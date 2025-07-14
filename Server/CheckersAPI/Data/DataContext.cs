using CheckersAPI.Models;
using CheckersAPI.Models.Checkers.Game;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CheckersAPI.Data {
    public class DataContext : IdentityDbContext<UserModel> {
        public DbSet<CheckerGame> CheckerGames { get; set; }
        public DbSet<CheckerPiece> CheckersPiece { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // Apply a blanket mapping: all DateTime and DateTime? → timestamp without time zone
            foreach (var entity in builder.Model.GetEntityTypes()) {
                foreach (var property in entity.GetProperties()
                                              .Where(p => p.ClrType == typeof(DateTime)
                                                       || p.ClrType == typeof(DateTime?))) {
                    property.SetColumnType("timestamp without time zone");
                }
            }
        }
    }
}
