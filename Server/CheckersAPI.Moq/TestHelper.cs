using CheckersAPI.Controllers;
using CheckersAPI.Data;
using CheckersAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CheckersAPI.Tests {
    public class TestHelper {
        public readonly DataContext dataContext;
        private readonly UserManager<UserModel> userManager;

        public TestHelper(AuthenticateController authenticateController) {
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase(databaseName: "CheckersInMemory");

            var dbContextOptions = builder.Options;
            dataContext = new DataContext(dbContextOptions);

            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();
        }

        public void InserUser() {
            // Create the default roles
            string[] roles = new string[] { "Beheerder", "Speler" };
            foreach (string role in roles) {
                var roleStore = new RoleStore<IdentityRole>(dataContext);

                if (!dataContext.Roles.Any(r => r.Name == role)) {
                    roleStore.CreateAsync(new IdentityRole(role));
                }
            }

            UserModel user = new() {
                UserName = "TestUser",
                Email = "TestUser@email.nl",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!dataContext.Users.Any(u => u.UserName == user.UserName)) {
                var password = new PasswordHasher<UserModel>();
                var hashed = password.HashPassword(user, "Ontwikkeling123@");
                user.PasswordHash = hashed;

                var result = userManager.CreateAsync(user);

                var insertedUser = userManager.FindByEmailAsync(user.Email).Result;
                userManager.AddToRoleAsync(insertedUser, roles[0]);
            }

            dataContext.SaveChangesAsync();
        }


    }
}
