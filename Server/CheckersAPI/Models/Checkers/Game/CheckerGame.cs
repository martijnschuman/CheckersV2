using CheckersAPI.Models.Checkers.Response;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckersAPI.Models.Checkers.Game {
    /// <summary>
    /// Class representing a game of checkers
    /// </summary>
    public class CheckerGame {
        // Enum representing the possible states of a game
        public enum GameStateOptions {
            Open,
            InProgress,
            P1_Won,
            P2_Won,
            Draw,
            Cancelled,
        }

        // Unique identifier for the game
        [Key]
        [Required]
        public int GameId { get; set; }

        // Unique infinte code
        [Required]
        public int? GameCode { get; set; }

        // Current state of the game
        [Required]
        [EnumDataType(typeof(GameStateOptions))]
        public GameStateOptions GameState { get; set; }

        // Winner of the game
        public string? WinnerId { get; set; }

        // Start date and time of the game
        [Column(TypeName = "DateTime")]
        public DateTime? StartDateTime { get; set; }

        // End date and time of the game
        [Column(TypeName = "DateTime")]
        public DateTime? EndDateTime { get; set; }

        // First player
        [Required]
        [ForeignKey("PlayerOne")]
        public string PlayerOneId { get; set; }

        // Second player
        [ForeignKey("PlayerTwo")]
        public string? PlayerTwoId { get; set; }

        // List of pieces on the board
        public List<CheckerPiece> Board { get; set; }

        // Current player's turn
        public string? CurrentPlayerTurnId { get; set; }

        // Saves creation date in db
        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Size of the board
        private int BoardSize = 10;


        public CheckerGame(string playerOneId) {
            Random random = new Random();

            PlayerOneId = playerOneId;
            GameCode = random.Next(100000, 999999);
            GameState = GameStateOptions.Open;
        }

        /// <summary>
        /// Creates the initial state of the game
        /// </summary>
        public void InitializeGame() {
            // Clear the board and initialize the pieces
            Board = new List<CheckerPiece>();

            // Set up the pieces for player one (lower rows)
            // Loop through the rows of the board
            for (int row = 0; row < 4; row++) {
                // Loop through the columns of the board
                for (int col = 0; col < BoardSize; col += 2) {
                    // Create non-king pieces for player one
                    var piece = new CheckerPiece {
                        RowIndex = row,
                        ColIndex = (row % 2 == 0 ? col + 1 : col),
                        IsKing = false,
                        IsTaken = false,
                        OwnerId = PlayerOneId,
                        GameId = GameId,
                        PlayerNumber = 1,
                    };

                    // Add the piece to the board
                    Board.Add(piece);
                }
            }

            // Set up the pieces for player two (upper rows)
            // Loop through the rows of the board
            for (int row = BoardSize - 4; row < BoardSize; row++) {
                // Loop through the columns of the board
                for (int col = 0; col < BoardSize; col += 2) {
                    // Create non-king pieces for player two
                    var piece = new CheckerPiece {
                        RowIndex = row,
                        ColIndex = (row % 2 == 0 ? col + 1 : col),
                        IsKing = false,
                        IsTaken = false,
                        OwnerId = PlayerTwoId,
                        GameId = GameId,
                        PlayerNumber = 2,
                    };

                    // Add the piece to the board
                    Board.Add(piece);
                }
            }
        }

        /// <summary>
        /// Get the piece at the specified position on the board
        /// </summary>
        /// <param name="row">Row position</param>
        /// <param name="col">Column position</param>
        /// <returns>A CheckerPiece if a piece has been found or null</returns>
        private CheckerPiece GetPieceAtPosition(int row, int col) {
            return Board.FirstOrDefault(piece => piece.RowIndex == row && piece.ColIndex == col && !piece.IsTaken);
        }

        /// <summary>
        /// Check if the specified square on the board is occupied
        /// </summary>
        /// <param name="row">Row position</param>
        /// <param name="col">Column position</param>
        /// <returns>True if a piece has been found, null if no piece has been found</returns>
        private bool IsSquareOccupied(int row, int col) {
            return GetPieceAtPosition(row, col) != null;
        }

        /// <summary>
        /// Check if the specified position is within the bounds of the board
        /// </summary>
        /// <param name="row">Row position</param>
        /// <param name="col">Column position</param>
        /// <returns></returns>
        private bool IsValidPosition(int row, int col) {
            return row >= 0 && col >= 0 && row <= BoardSize - 1 && col <= BoardSize - 1;
        }

        /// <summary>
        /// Try to capture a piece at the specified position
        /// </summary>
        /// <param name="piece">Represents the piece that is attacking</param>
        /// <param name="startRow">The current row of the piece</param>
        /// <param name="startCol">The current column of the piece</param>
        /// <param name="endRow">The destition row of the piece</param>
        /// <param name="endCol">The destition column of the piece</param>
        /// <returns></returns>
        private CheckerResponseModel TryCapturePiece(CheckerPiece piece, int startRow, int startCol, int endRow, int endCol) {
            // Calculate the position of the captured piece
            int capturedRow = (startRow + endRow) / 2;
            int capturedCol = (startCol + endCol) / 2;

            // Get the piece at the captured position
            var capturedPiece = GetPieceAtPosition(capturedRow, capturedCol);

            // Check if the captured piece exists and belongs to the opponent
            if (capturedPiece != null && capturedPiece.OwnerId != piece.OwnerId) {
                // Capture the piece
                capturedPiece.IsTaken = true;
                return new CheckerResponseModel { Status = "SUCCESS", Message = "Capture succesful", IsValid = true };
            }

            // No capture available
            return new CheckerResponseModel { Status = "ERROR", IsValid = false };
        }

        /// <summary>
        /// Check for capture options in the specified direction
        /// </summary>
        /// <param name="targetRow">The destition row of the piece</param>
        /// <param name="targetCol">The destition column of the piece</param>
        /// <param name="capturedRow">The row of the potential captured piece.</param>
        /// <param name="capturedCol">The column of the potential captured piece.</param>
        /// <param name="captures">A list to store captured pieces if capture is available.</param>
        private void CheckForCaptureOptions(int targetRow, int targetCol, int capturedRow, int capturedCol, List<CheckerPiece> captures) {
            // Check if the target position is within the bounds of the board
            if (IsValidPosition(targetRow, targetCol)) {
                var targetPiece = GetPieceAtPosition(targetRow, targetCol);
                var capturedPiece = GetPieceAtPosition(capturedRow, capturedCol);

                // Check if the target position is empty and the captured position has an opponent's piece
                if (targetPiece == null && capturedPiece != null && capturedPiece.OwnerId != CurrentPlayerTurnId) {
                    captures.Add(capturedPiece);
                }
            }
        }

        /// <summary>
        /// Gets a list of mandatory captures for the specified player's pieces.
        /// </summary>
        /// <param name="player">The current player who is makeing a move.</param>
        /// <returns>A list of CheckerPieces representing mandatory captures for the specified player.</returns>
        private List<CheckerPiece> GetMandatoryCaptures(string playerId) {
            List<CheckerPiece> mandatoryCaptures = new List<CheckerPiece>();

            foreach (var piece in Board.Where(p => p.OwnerId == playerId && !p.IsTaken)) {
                int startRow = piece.RowIndex;
                int startCol = piece.ColIndex;

                // Determine the direction of movement based on the player
                int forwardDirection = playerId == PlayerOneId ? 1 : -1;

                // Check for mandatory captures in the forward diagonal directions
                CheckForCaptureOptions(startRow + 2 * forwardDirection, startCol + 2, startRow + forwardDirection, startCol + 1, mandatoryCaptures);
                CheckForCaptureOptions(startRow + 2 * forwardDirection, startCol - 2, startRow + forwardDirection, startCol - 1, mandatoryCaptures);

                // If the piece is a king, also check the backward diagonal directions
                if (piece.IsKing) {
                    // Check for mandatory captures in the backward diagonal directions
                    CheckForCaptureOptions(startRow - 2 * forwardDirection, startCol + 2, startRow - forwardDirection, startCol + 1, mandatoryCaptures);
                    CheckForCaptureOptions(startRow - 2 * forwardDirection, startCol - 2, startRow - forwardDirection, startCol - 1, mandatoryCaptures);
                }
            }

            return mandatoryCaptures;
        }

        /// <summary>
        /// Validates the move for a player's checker piece.
        /// </summary>
        /// <param name="player">The player making the move.</param>
        /// <param name="currentPiece">The checker piece being moved.</param>
        /// <param name="startRow">The starting row of the piece.</param>
        /// <param name="startCol">The starting column of the piece.</param>
        /// <param name="endRow">The destination row of the move.</param>
        /// <param name="endCol">The destination column of the move.</param>
        /// <returns>True if the move is valid; otherwise, false.</returns>
        private CheckerResponseModel IsMoveValid(string player, CheckerPiece currentPiece, int startRow, int startCol, int endRow, int endCol) {
            if (player != CurrentPlayerTurnId) {
                return new CheckerResponseModel { Status = "ERROR", Message = $"Invalid move: It is player {CurrentPlayerTurnId}s turn.", IsValid = false };
            }

            // Check if the piece exists and belongs to the current player
            if (currentPiece == null || currentPiece.OwnerId != player) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: The selected piece does not exist or does not belong to the current player.", IsValid = false };
            }

            // Check for mandatory captures
            List<CheckerPiece> mandatoryCaptures = GetMandatoryCaptures(player);
            if (mandatoryCaptures.Count > 0) {
                // Check if the move includes a mandatory capture
                CheckerPiece capturedPiece = GetPieceAtPosition((startRow + endRow) / 2, (startCol + endCol) / 2);

                if (!mandatoryCaptures.Contains(capturedPiece)) {
                    return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: Mandatory captures must be performed.", IsValid = false };
                }
            }

            // Check if player one moves a piece backwards
            if (player == PlayerOneId && endRow <= startRow && !currentPiece.IsKing) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: Pieces can only be moved forward", IsValid = false };
            }

            // Check if player two moves a piece backwards
            if (player == PlayerTwoId && endRow >= startRow && !currentPiece.IsKing) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: Pieces can only be moved forward", IsValid = false };
            }

            // Check for not diagonal moves
            if (startCol == endCol && Math.Abs(endRow - startRow) > 0 || startRow == endRow && Math.Abs(endCol - startCol) > 0) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: Pieces can only me moved diagonally", IsValid = false };
            }

            // Check for move to same location
            if (startRow == endRow || startCol == endCol) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: Pieces can not move to the same location.", IsValid = false };
            }

            // Check if the destination is a valid empty square
            if (IsSquareOccupied(endRow, endCol)) {
                return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: The destination square is already occupied.", IsValid = false };
            }

            return new CheckerResponseModel { Status = "SUCCESS", IsValid = true };
        }

        /// <summary>
        /// Checks if a checker piece is eligible for king status based on its position.
        /// </summary>
        /// <param name="piece">The checker piece to be checked.</param>
        /// <returns>True if the piece is eligible for king status; otherwise, false.</returns>
        private bool CheckForKingMove(CheckerPiece piece) {
            return CurrentPlayerTurnId == PlayerOneId && piece.RowIndex == BoardSize - 1 || CurrentPlayerTurnId == PlayerTwoId && piece.RowIndex == 0;
        }

        /// <summary>
        /// Checks if the game is over by determining if one of the players has no more pieces left on the board.
        /// Updates the game state and declares a winner if applicable.
        /// </summary>
        /// <returns>True if the game is over; otherwise, false.</returns>
        private bool IsGameOver() {
            // Check if one of the players has no more pieces
            if (Board.Where(piece => piece.OwnerId == PlayerOneId).All(piece => piece.IsTaken == true)) {
                // PlayerTwo wins
                GameState = GameStateOptions.P2_Won;
                WinnerId = PlayerTwoId;
                EndDateTime = DateTime.Now;

                return true;
            }
            else if (Board.Where(piece => piece.OwnerId == PlayerTwoId).All(piece => piece.IsTaken == true)) {
                // PlayerOne wins
                GameState = GameStateOptions.P1_Won;
                WinnerId = PlayerOneId;
                EndDateTime = DateTime.Now;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to make a move on the checkerboard based on the provided parameters.
        /// </summary>
        /// <param name="player">The player making the move.</param>
        /// <param name="startRow">The starting row index of the piece to be moved.</param>
        /// <param name="startCol">The starting column index of the piece to be moved.</param>
        /// <param name="endRow">The target row index for the move.</param>
        /// <param name="endCol">The target column index for the move.</param>
        /// <returns>True if the move is successful; otherwise, false.</returns>
        public CheckerResponseModel MakeMove(string playerId, int startRow, int startCol, int endRow, int endCol) {
            // Sets the game state to in progress
            GameState = GameStateOptions.InProgress;

            // Check if the game is over
            if (IsGameOver()) {
                Log.Information("Invlid move: Game is finnished, winner: " + WinnerId);
                return new CheckerResponseModel { Status = "ERROR", Message = "Invlid move: Game is finnished, winner: " + WinnerId, IsValid = false };
            };

            // Get the piece at the starting position
            var currentPiece = GetPieceAtPosition(startRow, startCol);

            // Check if the move is valid
            var IsValid = IsMoveValid(playerId, currentPiece, startRow, startCol, endRow, endCol);
            // If the move is invalid, return the ResponseModel with the error message
            if (!IsValid.IsValid) {
                return IsValid;
            }

            // Check if the piece is a king
            if (currentPiece.IsKing) {
                if (Math.Abs(endRow - startRow) != Math.Abs(endCol - startCol)) {
                    return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: The king can not move more than 2 steps diagonally and horizontally at the same time.", IsValid = false };
                }

                // Determine the direction of movement
                int rowDirection = (endRow - startRow) / Math.Abs(endRow - startRow);
                int colDirection = (endCol - startCol) / Math.Abs(endCol - startCol);

                // Check for captures along the path
                while (startRow != endRow || startCol != endCol) {
                    // Move to the next square
                    startRow += rowDirection;
                    startCol += colDirection;

                    var tryCapture = TryCapturePiece(currentPiece, startRow - rowDirection, startCol - colDirection, endRow, endCol);
                    // Try to capture a piece along the way
                    if (!tryCapture.IsValid) {
                        // If no capture, check if the move is still valid
                        if (IsSquareOccupied(startRow, startCol)) {
                            return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: The king cannot jump over more than one piece at a time or over it's own pieces.", IsValid = false };
                        }
                    }
                }
            }
            else {
                // For regular pieces, perform the regular capture logic
                // Check for a possible capture, capture if true
                var tryCapture = TryCapturePiece(currentPiece, startRow, startCol, endRow, endCol);
                if (!tryCapture.IsValid) {
                    // If no capture check if the move is still valid
                    if (!currentPiece.IsKing && (Math.Abs(endRow - startRow) > 1 || Math.Abs(endCol - startCol) > 1)) {
                        return new CheckerResponseModel { Status = "ERROR", Message = "Invalid move: The piece can not move more than 2 steps diagonally.", IsValid = false };
                    }
                }
            }

            // Move the piece to the destination
            currentPiece.RowIndex = endRow;
            currentPiece.ColIndex = endCol;

            // Check for king
            if (CheckForKingMove(currentPiece)) {
                currentPiece.IsKing = true;
                Log.Information("King Piece");
            }

            // Update the current turn
            CurrentPlayerTurnId = playerId == PlayerOneId ? PlayerTwoId : PlayerOneId;

            // If the game is over, return the winner
            if (IsGameOver()) {
                return new CheckerResponseModel { Status = "SUCCESS", Message = $"Game over, player {WinnerId} won", IsValid = true };
            };

            // Return true if the move is valid, false otherwise
            return new CheckerResponseModel { Status = "SUCCESS", IsValid = true };
        }
    }
}
