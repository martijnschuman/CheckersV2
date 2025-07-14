using CheckersAPI.Controllers;
using CheckersAPI.Models.Checkers.Game;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace CheckersAPI.Hubs {
    public class CheckersHub : Hub {
        private readonly CheckersController _controller;

        public CheckersHub(CheckersController controller) {
            _controller = controller;
        }

        /// <summary>
        /// Allows players to join the specific game connection group
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public async Task JoinGameGroup(int gameCode) {
            Log.Information($"User joined the group gameCode: {gameCode}");
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode.ToString());
        }

        /// <summary>
        /// Allows players to leave the specific game connection group
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public async Task LeaveGameGroup(int gameCode) {
            Log.Information($"User left the group gameCode: {gameCode}");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode.ToString());
        }

        /// <summary>
        /// Allows players to make a move in the game
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="playerID"></param>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        /// <param name="endRow"></param>
        /// <param name="endCol"></param>
        /// <returns></returns>
        public async Task MovePiece(int gameCode, string playerID, int startRow, int startCol, int endRow, int endCol) {
            // Get the game from the database
            CheckerGame game = _controller.GetGameFromCodeAndID(gameCode, playerID);
            // Makes the move
            var result = game.MakeMove(playerID, startRow, startCol, endRow, endCol);

            // If the move is valid, update the game state in the database
            if (result.IsValid) {
                _controller.UpdateCheckersGame(game);
                _controller.UpdateCheckersPieces(game.Board);
                //_controller.UpdateCheckersPieces(game.Board.Where(piece => piece.IsTaken == false).ToList());

                // Notify all clients in the game group about the updated game state
                await Clients.Group(gameCode.ToString()).SendAsync("CheckersBoardUpdated", game.Board, game.GameState, result.Message, game.CurrentPlayerTurnId);
            }
            // If the move is invalid, notify the caller about the error
            else {
                Log.Warning(result.Message);

                // Notify caller about the error
                await Clients.Caller.SendAsync("CheckersBoardError", result.Message);
            }
        }
    }
}
