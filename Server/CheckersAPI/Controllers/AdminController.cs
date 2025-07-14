using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models.Admin;
using CheckersAPI.Models.Admin.Response;
using CheckersAPI.Models.Checkers.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Security.Claims;

namespace CheckersAPI.Controllers {
    [Authorize(Roles = "Beheerder")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : BaseController {
        public AdminController(DataContext context, IHubContext<CheckersHub> hubContext) : base(context, hubContext) {
        }

        /// <summary>
        /// Gets all Checkers games seperated by games which can be cancelled
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/[controller]/getAllGames")]
        [ProducesResponseType(typeof(AdminResponseModel), 200)]
        [ProducesResponseType(typeof(AdminResponseModel), 400)]
        [ProducesResponseType(typeof(AdminResponseModel), 401)]
        [ProducesResponseType(typeof(AdminResponseModel), 500)]
        public async Task<IActionResult> GetAllCheckerGames() {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("AdminController - GetAllCheckerGames: Can't find user in Database or token invalid");
                return Unauthorized(new AdminResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            Log.Information($"AdminController - GetAllCheckerGames: Method accessed by user {user.UserName}");

            // Calculate threshold date 7 days ago
            DateTime thresholdDate = DateTime.Now.AddDays(-7);

            // Get all games
            var allGames = _context.CheckerGames
                .OrderByDescending(game => game.CreatedAt)
                .Select(game => new {
                    Game = game,
                    PlayerOneUserName = _context.Users.Where(u => u.Id == game.PlayerOneId).Select(u => u.UserName).FirstOrDefault(),
                    PlayerTwoUserName = _context.Users.Where(u => u.Id == game.PlayerTwoId).Select(u => u.UserName).FirstOrDefault(),
                    PlayerOneRemainingPieces = _context.CheckersPiece.Count(piece => piece.OwnerId == game.PlayerOneId && !piece.IsTaken && game.GameId == piece.GameId),
                    PlayerTwoRemainingPieces = _context.CheckersPiece.Count(piece => piece.OwnerId == game.PlayerTwoId && !piece.IsTaken && game.GameId == piece.GameId)
                })
                .ToList();

            // Split games into cancelable and remaining
            var cancelableGames = allGames.Where(game =>
                game.Game.WinnerId == null &&
                (game.Game.GameState == CheckerGame.GameStateOptions.Open || game.Game.GameState == CheckerGame.GameStateOptions.InProgress) &&
                game.Game.EndDateTime == null &&
                game.Game.UpdatedAt <= thresholdDate
            ).ToList();

            var remainingGames = allGames.Except(cancelableGames).ToList();

            return Ok(new AdminResponseModel { Status = "Success", Message = $"{remainingGames.Count} games found, {cancelableGames.Count} can be cancelled.", CancelableGames = cancelableGames, RemainingGames = remainingGames });
        }

        /// <summary>
        /// Cancels a specific game based on the id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/cancelGame")]
        [ProducesResponseType(typeof(AdminResponseModel), 200)]
        [ProducesResponseType(typeof(AdminResponseModel), 400)]
        [ProducesResponseType(typeof(AdminResponseModel), 401)]
        [ProducesResponseType(typeof(AdminResponseModel), 500)]
        public async Task<IActionResult> CancelCheckersGame([FromBody] CheckersCancelModel model) {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("AdminController - GetAllCheckerGames: Can't find user in Database or token invalid");
                return Unauthorized(new AdminResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            Log.Information($"AdminController - CancelCheckersGame: Method accessed by user {user.UserName}");

            // Checks if the model is empty
            if (model == null) {
                Log.Warning("AdminController - CancelCheckersGame: Model == null" + ModelState);
                BadRequest(ModelState);
            }

            // Checks if the model is invalid
            if (!ModelState.IsValid) {
                Log.Warning("AdminController - CancelCheckersGame: Invalid ModelState " + ModelState);
                BadRequest(ModelState);
            }

            // Gets the games based on the ID
            CheckerGame game = _context.CheckerGames.Where(game => game.GameId == model.GameId).FirstOrDefault();
            // If a game has been found
            if (game != null) {
                // Cancel the game
                game.GameState = CheckerGame.GameStateOptions.Cancelled;
                game.EndDateTime = DateTime.Now;
                game.UpdatedAt = DateTime.Now;

                // Save the changes to the DB
                _context.Update(game);
                _context.SaveChanges();

                return Ok(new AdminResponseModel { Status = "Success", Message = $"CheckersGame with id {model.GameId} cancelled" });
            }

            return BadRequest(new AdminResponseModel { Status = "ERROR", Message = $"Something went wrong while trying to cancel CheckersGame with id {model.GameId}" });
        }
    }
}
