using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models;
using CheckersAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwoFactorAuthNet;

namespace CheckersAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, DataContext context, IHubContext<CheckersHub> hubContext) : base(context, hubContext) {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Signs in a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/signIn")]
        [ProducesResponseType(200)] // Success
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Error
        public async Task<IActionResult> SignIn([FromBody] SignInModel model) {
            // Check if the model is null
            if (model == null) {
                Log.Error("AuthenticateController - SignIn - Model == null");
                return BadRequest(ModelState);
            }

            // Check if the model is valid
            if (!ModelState.IsValid) {
                Log.Error("AuthenticateController - SignIn - Model == invalid " + ModelState);
                return BadRequest(ModelState);
            }

            // Check if the user exists
            var user = await _userManager.FindByNameAsync(model.Username);

            // Add this null check to prevent NullReferenceException
            if (user == null) {
                Log.Warning("AuthenticateController - SignIn: User not found for username " + model.Username);
                return Unauthorized(new AuthResponseModel {
                    Status = "ERROR",
                    Message = "Combination of user credentials invalid, user not found or the account has been blocked."
                });
            }

            // If the user exists and the account hasn't been blocked
            if (user.AccessFailedCount < 4) {
                // If the password is correct
                if (await _userManager.CheckPasswordAsync(user, model.Password)) {
                    // If the user has MFA enabled
                    if (user.TwoFactorEnabled) {
                        // Verify the MFA code
                        if (!VerifyMFACode((int)model.MultiFactorKey, user.MultiFactorPrivateKey)) {
                            // Return 401 of code is wrong
                            return Unauthorized();
                        }
                    }

                    // Reset AccessFailedCount
                    if (user.AccessFailedCount > 0) {
                        user.AccessFailedCount = 0;
                        _context.Update(user);
                        _context.SaveChanges();
                    }

                    // Get the user roles
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Creates a List of Claims
                    var authClaims = new List<Claim>{
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    // Add the user roles to the JWT token
                    foreach (var userRole in userRoles) {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    // Create the JWT token
                    var token = GetToken(authClaims);

                    // Return the JWT token
                    return Ok(new {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                    });
                }
                else {
                    Log.Warning($"Login of user {user.UserName} failed.");
                    user.AccessFailedCount++;
                    _context.Update(user);
                    _context.SaveChanges();
                }
            }
            else {
                Log.Warning($"Login of user {user.UserName} failed.");
            }

            return Unauthorized(new AuthResponseModel {
                Status = "ERROR",
                Message = "Combination of user credentials invalid, user not found or the account has been blocked."
            });
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/signUp")]
        [ProducesResponseType(200)] // Success
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Error
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model) {
            // Check if the model is null
            if (model == null) {
                Log.Error("AuthenticateController - SignUp - Model == null");
                return BadRequest(ModelState);
            }

            // Check if the model is valid
            if (!ModelState.IsValid) {
                Log.Error("AuthenticateController - SignUp - Model == invalid " + ModelState);
                return BadRequest(ModelState);
            }

            // Check if the user already exists
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null) {
                Log.Error("AuthenticateController - SignUp - User already exists!");
                return BadRequest(new AuthResponseModel { Status = "Error", Message = "User already exists!" });
            }

            // Creates a new UserModel object
            UserModel user = new() {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            // Inserts the new user in the DB
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                return BadRequest(new AuthResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            // Assigns the user to the Player role
            IActionResult assignUserRoles = await AssignUserRoles(user);
            // If the user role assignment failed
            if (!(assignUserRoles is OkObjectResult)) {
                Log.Error("AuthenticateController - SignUp - AssignUserRoles failed");
                return BadRequest(assignUserRoles);
            }

            else {
                // User created successfully
                Log.Information("AuthenticateController - SignUp - User Created");
                return Ok(new AuthResponseModel { Status = "Success", Message = "User created successfully!" });
            }
        }

        /// <summary>
        /// Gets the user's MFA key
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/[controller]/getMFA")]
        [ProducesResponseType(200)] // Success
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Error
        public async Task<IActionResult> GetMultiFactorKey() {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user has been found
            if (user == null) {
                Log.Error("AuthenticateController - GetMultiFactorKey - No user found");
                return BadRequest(new AuthResponseModel { Status = "ERROR", Message = "No user found!" });
            }

            // If the user has no 2FA enabled
            if (!user.TwoFactorEnabled) {
                // Generate a new 2FA secret
                TwoFactorAuth tfa = new TwoFactorAuth(Environment.GetEnvironmentVariable("MultiFactorIssuer"));
                var secret = tfa.CreateSecret(80);

                // Store the secret
                user.MultiFactorPrivateKey = secret;
                _context.Update(user);
                _context.SaveChanges();

                // Generates a QR code string
                var qr = $"otpauth://totp/{Environment.GetEnvironmentVariable("MultiFactorIssuer")}:{user.Email}?secret={secret}&issuer={Environment.GetEnvironmentVariable("MultiFactorIssuer")}";

                // Return the secret to the user
                return Ok(new AuthResponseModel { Status = "Success", Message = "User token created", MultiFactorKey = secret, MultiFactoryKeyQR = qr });
            }
            else {
                return Ok(new AuthResponseModel { Status = "ERROR", Message = "2FA already setup", MultiFactorKey = null });
            }
        }

        /// <summary>
        /// Enables the user's MFA
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/enableMFA")]
        [ProducesResponseType(200)] // Success
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Error
        public async Task<IActionResult> EnableMultiFactor([FromBody] MFASetupModel model) {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user has been found
            if (user == null) {
                Log.Error("AuthenticateController - EnableMultiFactor - No user found");
                return BadRequest(new AuthResponseModel { Status = "ERROR", Message = "No user found!" });
            }

            // Check if the model is null
            if (model == null) {
                Log.Error("AuthenticateController - EnableMultiFactor - Model == null");
                return BadRequest(ModelState);
            }

            // Check if the model is valid
            if (!ModelState.IsValid) {
                Log.Error("AuthenticateController - EnableMultiFactor - Model == invalid " + ModelState);
                return BadRequest(ModelState);
            }

            // If the MFA checks out
            if (VerifyMFACode((int)model.MultiFactorKey, user.MultiFactorPrivateKey)) {
                // Enable MFA
                user.TwoFactorEnabled = true;
                _context.Update(user);
                _context.SaveChanges();

                return Ok(new AuthResponseModel { Status = "Success", Message = "MFA enabled!" });
            }

            return BadRequest(new AuthResponseModel { Status = "ERROR", Message = "Something went wrong while enableing MFA." });
        }

        /// <summary>
        /// Disables the user's MFA
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/disableMFA")]
        [ProducesResponseType(200)] // Success
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Error
        public async Task<IActionResult> DisableMultiFactor([FromBody] MFASetupModel model) {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user has been found
            if (user == null) {
                Log.Error("AuthenticateController - DisableMultiFactor - No user found");
                return BadRequest(new AuthResponseModel { Status = "ERROR", Message = "No user found!" });
            }

            // Check if the model is null
            if (model == null) {
                Log.Error("AuthenticateController - DisableMultiFactor - Model == null");
                return BadRequest(ModelState);
            }

            // Check if the model is valid
            if (!ModelState.IsValid) {
                Log.Error("AuthenticateController - DisableMultiFactor - Model == invalid " + ModelState);
                return BadRequest(ModelState);
            }

            // If the MFA checks out
            if (VerifyMFACode((int)model.MultiFactorKey, user.MultiFactorPrivateKey)) {
                // Disable MFA
                user.TwoFactorEnabled = false;
                user.MultiFactorPrivateKey = "";
                _context.Update(user);
                _context.SaveChanges();

                return Ok(new AuthResponseModel { Status = "Success", Message = "MFA disabled!" });
            }

            return BadRequest(new AuthResponseModel { Status = "ERROR", Message = "Something went wrong while disabling MFA." });
        }

        /// <summary>
        /// Assigns the user to the Player role
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<IActionResult> AssignUserRoles(UserModel user) {
            IActionResult result;

            // If the Player user role does not exist
            if (!await _roleManager.RoleExistsAsync(UserRolesModel.Player)) {
                // Create the role
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(UserRolesModel.Player));
                if (!roleResult.Succeeded) {
                    // Role creation failed
                    Log.Error("AuthenticateController - AssignUserRoles - Role creation failed!");
                    result = BadRequest(new AuthResponseModel { Status = "Error", Message = "Role creation failed!" });
                    return result;
                }
            }

            // If the Player user role exists
            if (await _roleManager.RoleExistsAsync(UserRolesModel.Player)) {
                var addRoleResult = await _userManager.AddToRoleAsync(user, UserRolesModel.Player);
                if (!addRoleResult.Succeeded) {
                    // Adding user to role failed
                    Log.Error("AuthenticateController - AssignUserRoles - Failed to assign user role!");
                    result = BadRequest(new AuthResponseModel { Status = "Error", Message = "Failed to assign user role!" });
                    return result;
                }
            }

            // User role assigned successfully
            return Ok(new AuthResponseModel { Status = "Success", Message = "User role assigned successfully!" }); ;
        }

        /// <summary>
        /// Creates a new JWT token with the given claims and returns it
        /// </summary>
        /// <param name="authClaims"></param>
        /// <returns></returns>
        private JwtSecurityToken GetToken(List<Claim> authClaims) {
            // Get's the JWT secret from the appsettings.json file
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Creates the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        /// <summary>
        /// Verifies the MFA code
        /// </summary>
        /// <param name="multiFactorKey"></param>
        /// <param name="multiFactorPrivateKey"></param>
        /// <returns></returns>
        private bool VerifyMFACode(int multiFactorKey, string userSecret) {
            TwoFactorAuth tfa = new TwoFactorAuth(Environment.GetEnvironmentVariable("MultiFactorKeyName"));

            bool mfa = tfa.VerifyCode(userSecret, multiFactorKey.ToString());

            return mfa;
        }
    }
}
