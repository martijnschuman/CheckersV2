using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models.Checkers;
using CheckersAPI.Models.Checkers.Game;
using CheckersAPI.Models.Checkers.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Security.Claims;

namespace CheckersAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CheckersController : BaseController {
        public CheckersController(DataContext context, IHubContext<CheckersHub> hubContext) : base(context, hubContext) {
        }

        /// <summary>
        /// Create a new game and adds the user to the game
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/createGame")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 401)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public async Task<ActionResult> CreateGame() {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("CheckersController - CreateGame: Can't find user in Database or token invalid");
                return Unauthorized(new CheckerResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            // Check if user already has an open game
            var getOpenGames = _context.CheckerGames.Where(game =>
                (game.PlayerOneId == user.Id || game.PlayerTwoId == user.Id) &&
                (game.GameState == CheckerGame.GameStateOptions.Open || game.GameState == CheckerGame.GameStateOptions.InProgress) &&
                (game.EndDateTime == null)
                ).Count();

            // If user already has an open game, return error
            if (getOpenGames > 0) {
                Log.Warning($"User {user.Id} tried to create a game while he has an open game!");
                // Return error
                return BadRequest(new CheckerResponseModel() { Status = "ERROR", Message = "You can only play one game at the time!" });
            }
            else {
                // Create new game and adds the current user as PlayerOne
                var game = new CheckerGame(user.Id);

                // Save game to database
                _context.CheckerGames.Add(game);
                _context.SaveChanges();

                // Return success with the game code
                return Ok(new CheckerResponseModel() { Status = "Success", Message = "Chechekers game created!", GameCode = game.GameCode });
            }
        }

        /// <summary>
        /// Joins a game
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/joinGame")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public async Task<ActionResult> JoinGame([FromBody] CheckerJoinModel joinModel) {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("CheckersController - JoinGame: Can't find user in Database or token invalid");
                return Unauthorized(new CheckerResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            // Checks if the model is empty
            if (joinModel == null) {
                Log.Warning("CheckersController - JoinGame: Model == null" + ModelState);
                return BadRequest(ModelState);
            }

            // Checks if the model is invalid
            if (!ModelState.IsValid) {
                Log.Warning("CheckersController - JoinGame: Invalid ModelState " + ModelState);
                return BadRequest(ModelState);
            }

            // Get the game based on the game code and the current user and the game state is open
            var game = _context.CheckerGames.Where(game =>
                game.GameState == CheckerGame.GameStateOptions.Open &&
                game.PlayerTwoId == null &&
                game.EndDateTime == null &&
                game.GameCode == joinModel.GameCode
            ).FirstOrDefault();

            // If no game found, return error
            if (game == null) {
                Log.Warning($"CheckersController - JoinGame: Invalid GameCode {joinModel.GameCode}");
                return BadRequest(new CheckerResponseModel() { Status = "ERORR", Message = "Invalid GameCode", GameCode = joinModel.GameCode });
            }

            // If the user is trying to join his own game, return error
            if (game.PlayerOneId == user.Id) {
                Log.Warning($"CheckersController - JoinGame: Player {user.Id} tried to join his own game!");
                return BadRequest(new CheckerResponseModel() { Status = "ERORR", Message = "You can't join your own game", GameCode = joinModel.GameCode });
            }

            // Check if 30 minutes has passed
            if ((DateTime.Now - game.CreatedAt).TotalMinutes > 30) {
                Log.Warning($"CheckersController - JoinGame: Player {user.Id} tried to join expired game {game.GameId}");

                // Cancell the game
                game.GameState = CheckerGame.GameStateOptions.Cancelled;
                // Update game in DB
                _context.Update(game);
                _context.SaveChanges();

                return BadRequest(new CheckerResponseModel() { Status = "ERROR", Message = "GameCode expired", GameCode = joinModel.GameCode });
            }

            // Set the second player to the game
            game.PlayerOneId = game.PlayerOneId;
            game.PlayerTwoId = user.Id;
            game.CurrentPlayerTurnId = game.PlayerOneId;
            game.StartDateTime = DateTime.Now;

            // Initialize the game for the second player
            game.InitializeGame();
            // Saves the CheckersPieces to the DB
            UpdateCheckersPieces(game.Board);

            // Save game to database
            _context.Update(user);
            _context.SaveChanges();

            // Sends a notify to the game creator that the game has started
            await _hubContext.Clients.Group(game.GameCode.ToString()).SendAsync("GameStarted", "Started");

            // Send game update to the players
            return Ok(new CheckerResponseModel() { Status = "SUCCESS", Message = "Game joined!", GameCode = game.GameCode });
        }

        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <param name="checkerInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/getGameInfo")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public async Task<ActionResult> GetSingleUserGame() {
            // Get user from database
            var user = await GetUserFromDatabase(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // If no user, return unauthorized
            if (user == null) {
                Log.Warning("CheckersController - GetDetaildGameInfo: Can't find user in Database or token invalid");
                return Unauthorized(new CheckerResponseModel() { Status = "ERROR", Message = "No user found or invalid token!" });
            }

            // Gets the game based on the gameId and the current user
            CheckerGame? game = _context.CheckerGames.Where(game =>
                (game.PlayerOneId == user.Id || game.PlayerTwoId == user.Id) &&
                (game.GameState == CheckerGame.GameStateOptions.Open || game.GameState == CheckerGame.GameStateOptions.InProgress)
            ).FirstOrDefault();

            // If a game has been found
            if (game != null) {
                // Get the CheckerPieces
                List<CheckerPiece> pieces = _context.CheckersPiece.Where(piece => piece.GameId == game.GameId).ToList();

                // If piecess have been found
                if (pieces.Count > 0) {
                    // Add them to the game
                    game.Board = pieces;

                    string PlayerOneUserName = _context.Users.Where(user => user.Id == game.PlayerOneId).First().UserName;
                    string PlayerTwoUserName = _context.Users.Where(user => user.Id == game.PlayerTwoId).First().UserName;
                    string CurrentPlayerUserName = _context.Users.Where(user => user.Id == game.CurrentPlayerTurnId).First().UserName;

                    // Return Ok and the game to the client
                    return Ok(new CheckerResponseModel() { Status = "SUCCESS", Message = "Game info fetched", CheckerGame = game, PlayerOneUserName = PlayerOneUserName, PlayerTwoUserName = PlayerTwoUserName, CurrentPlayerUserName = CurrentPlayerUserName });
                }
            }

            // Return error
            return BadRequest(new CheckerResponseModel() { Status = "ERROR", Message = "No checkers game found or you have no access to the game" });
        }

        /// <summary>
        /// Gets a specific game based on the game code and the player id
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="playerID"></param>
        /// <returns>The CheckerGame</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public CheckerGame GetGameFromCodeAndID(int gameCode, string playerID) {

            CheckerGame game = _context.CheckerGames.Where(game =>
               game.GameCode == gameCode &&
               (game.GameState == CheckerGame.GameStateOptions.InProgress || game.GameState == CheckerGame.GameStateOptions.Open) &&
               (game.PlayerOneId == playerID || game.PlayerTwoId == playerID)
            ).First();
            game.Board = _context.CheckersPiece.Where(piece => piece.GameId == game.GameId).ToList();

            return game;
        }

        /// <summary>
        /// Updates the game in the database
        /// </summary>
        /// <param name="game"></param>
        /// <returns>ActionResult based on the flow</returns>
        [HttpPut]
        [Route("/[controller]/updateGame")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public ActionResult UpdateCheckersGame(CheckerGame game) {
            // If game is null, return error
            if (game == null) {
                return BadRequest(new CheckerResponseModel() { Status = "ERROR", Message = "Can't update game, game == null", IsValid = false });
            }

            // Set the updated at to the current time
            game.UpdatedAt = DateTime.Now;

            // Update game in database
            _context.CheckerGames.Update(game);
            _context.SaveChanges();

            // Return success
            return Ok(new CheckerResponseModel() { Status = "SUCCESS", Message = "Game updated", IsValid = true });
        }

        /// <summary>
        /// Updates the pieces in the database
        /// </summary>
        /// <param name="pieces"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/[controller]/updatePieces")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        [ProducesResponseType(typeof(CheckerResponseModel), 400)]
        [ProducesResponseType(typeof(CheckerResponseModel), 500)]
        public ActionResult UpdateCheckersPieces(List<CheckerPiece> pieces) {
            // If pieces are null or empty, return error
            if (pieces == null || pieces.Count == 0) {
                return BadRequest(new CheckerResponseModel { Status = "ERROR", Message = "Can't update checkers pieces, pieces == null", IsValid = false });
            }

            // Foreach piece, set the updated at to the current time and update the piece in the database
            foreach (CheckerPiece piece in pieces) {
                piece.UpdatedAt = DateTime.Now;

                _context.CheckersPiece.Update(piece);
            }

            _context.SaveChanges();

            // Return success
            return Ok(new CheckerResponseModel() { Status = "SUCCESS", Message = "Pieces updted", IsValid = true });
        }
    }
}
