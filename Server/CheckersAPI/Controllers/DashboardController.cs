using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models.Checkers.Game;
using CheckersAPI.Models.Checkers.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Security.Claims;

namespace CheckersAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController {
        public DashboardController(DataContext context, IHubContext<CheckersHub> hubContext) : base(context, hubContext) {
        }

        /// <summary>
        /// Returns every game the user has played or is playing
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/getAllGames")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public async Task<ActionResult> GetAllUserGames() {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("CheckersController - GetGlobalGameInfo: Can't find user in Database or token invalid");
                return BadRequest(new CheckerResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            // Get global game stats
            var stats = new {
                Wins = _context.CheckerGames.Count(game => game.WinnerId == user.Id),
                Draws = _context.CheckerGames.Count(game => game.WinnerId == null && (game.PlayerOneId == user.Id || game.PlayerTwoId == user.Id) && game.GameState == CheckerGame.GameStateOptions.Draw),
                Losses = _context.CheckerGames.Count(game => game.WinnerId != user.Id && ((game.PlayerOneId == user.Id && game.GameState == CheckerGame.GameStateOptions.P2_Won) || (game.PlayerTwoId == user.Id && game.GameState == CheckerGame.GameStateOptions.P1_Won)))
            };

            // Get all user games
            var allGames = _context.CheckerGames
                .Where(game => game.PlayerOneId == user.Id || game.PlayerTwoId == user.Id)
                .OrderByDescending(game => game.GameState == CheckerGame.GameStateOptions.Open || game.GameState == CheckerGame.GameStateOptions.InProgress)
                .ThenByDescending(game => game.GameId)
                .Select(game => new {
                    Game = game,
                    PlayerOneUserName = _context.Users.Where(u => u.Id == game.PlayerOneId).Select(u => u.UserName).FirstOrDefault(),
                    PlayerTwoUserName = _context.Users.Where(u => u.Id == game.PlayerTwoId).Select(u => u.UserName).FirstOrDefault(),
                    PlayerOneRemainingPieces = _context.CheckersPiece.Count(piece => piece.OwnerId == game.PlayerOneId && !piece.IsTaken && game.GameId == piece.GameId),
                    PlayerTwoRemainingPieces = _context.CheckersPiece.Count(piece => piece.OwnerId == game.PlayerTwoId && !piece.IsTaken && game.GameId == piece.GameId)
                })
                .ToList();

            // Return Ok and the games
            return Ok(new CheckerResponseModel() { Status = allGames.Count() > 0 ? "SUCCESS" : "ERROR", Message = $"{allGames.Count} game(s) found", CheckerGames = allGames, CheckerGameStats = stats });
        }
    }
}
