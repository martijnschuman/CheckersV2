using CheckersTest;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckersAPI.Models {
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
            Lost,
        }

        // Unique identifier for the game
        [Key]
        [Required]
        public int GameId { get; set; }

        // Unique infinte code
        [Required]
        public required int GameCode { get; set; }

        // Current state of the game
        [Required]
        [EnumDataType(typeof(GameStateOptions))]
        public required GameStateOptions GameState { get; set; }

        // Winner of the game
        [Required]
        public Player? Winner { get; set; }

        // Start date and time of the game
        [Required]
        [Column(TypeName = "Date")]
        public required DateTime StartDateTime { get; set; }

        // End date and time of the game
        [Column(TypeName = "Date")]
        public DateTime? EndDateTime { get; set; } // Nullable, as the game might not be over

        // Players in the game
        [Required]
        [ForeignKey("PlayerOne")]
        public int PlayerOneId { get; set; }
        public Player PlayerOne { get; set; }

        [Required]
        [ForeignKey("PlayerTwo")]
        public int PlayerTwoId { get; set; }
        public Player PlayerTwo { get; set; }

        // Size of the board
        [Required]
        public int BoardSize { get; set; } = 8;

        // List of pieces on the board
        [Required]
        public List<CheckerPiece> Board { get; set; }
        // Current player's turn
        [Required]
        public Player CurrentTurn { get; set; }

        /// <summary>
        /// Constructor for the CheckerGame class
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        public CheckerGame(Player playerOne, Player playerTwo) {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            CurrentTurn = playerOne;

            // Initialize the board and other properties
            InitializeGame();
        }

        /// <summary>
        /// Creates the initial state of the game
        /// </summary>
        public void InitializeGame() {
            // Clear the board and initialize the pieces
            Board = new List<CheckerPiece>();

            // Set up the pieces for player one (lower rows)
            // Loop through the rows of the board
            for (int row = 0; row < 3; row++) {
                // Loop through the columns of the board
                for (int col = 0; col < BoardSize; col += 2) {
                    // Create non-king pieces for player one
                    var piece = new CheckerPiece {
                        RowIndex = row,
                        ColIndex = (row % 2 == 0) ? col : col + 1,
                        IsKing = false,
                        IsTaken = false,
                        Owner = PlayerOne,
                        GameId = GameId,
                        Game = this // Set the reference back to the game
                    };

                    // Add the piece to the board
                    Board.Add(piece);
                }
            }

            // Set up the pieces for player two (upper rows)
            // Loop through the rows of the board
            for (int row = BoardSize - 3; row < BoardSize; row++) {
                // Loop through the columns of the board
                for (int col = 0; col < BoardSize; col += 2) {
                    // Create non-king pieces for player two
                    var piece = new CheckerPiece {
                        RowIndex = row,
                        ColIndex = (row % 2 == 0) ? col : col + 1,
                        IsKing = false,
                        IsTaken = false,
                        Owner = PlayerTwo,
                        GameId = GameId,
                        Game = this // Set the reference back to the game
                    };

                    // Add the piece to the board
                    Board.Add(piece);
                }
            }
        }

        /// <summary>
        /// Prints the current state of the game to the console
        /// </summary>
        public void PrintGame() {
            Console.WriteLine("Current Checkers Board:");

            for (int row = 0; row < BoardSize; row++) {
                for (int col = 0; col < BoardSize; col++) {
                    var piece = GetPieceAtPosition(row, col);

                    if (piece != null && !piece.IsTaken) {
                        Console.Write($"{(piece.IsKing ? "K" : "P")}{(piece.Owner == PlayerOne ? "1" : "2")} ");
                    }
                    else {
                        Console.Write(" - ");
                    }
                }

                Console.WriteLine(); // Move to the next row
            }

            Console.WriteLine("Player 1: " + PlayerOne.UserName);
            Console.WriteLine("Player 2: " + PlayerTwo.UserName);
            Console.WriteLine("Player 1 pieces: " + Board.Count(piece => piece.Owner == PlayerOne && !piece.IsTaken));
            Console.WriteLine("Player 2 pieces: " + Board.Count(piece => piece.Owner == PlayerTwo && !piece.IsTaken));
            Console.WriteLine("Current Turn: " + (CurrentTurn == PlayerOne ? "Player 1" : "Player 2"));
            Console.WriteLine("Gamestate: " + GameState.ToString());

            //Console.WriteLine("\nDetailed Piece Information:");
            //foreach (var piece in Board) {
            //    Console.WriteLine($"Piece at ({piece.RowIndex}, {piece.ColIndex}) - Owner: {piece.Owner?.UserName}, IsKing: {piece.IsKing}, IsTaken {piece.IsTaken}");
            //}

            Console.WriteLine();
            Console.WriteLine();
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
        private bool TryCapturePiece(CheckerPiece piece, int startRow, int startCol, int endRow, int endCol) {
            // Calculate the position of the captured piece
            int capturedRow = (startRow + endRow) / 2;
            int capturedCol = (startCol + endCol) / 2;

            // Get the piece at the captured position
            var capturedPiece = GetPieceAtPosition(capturedRow, capturedCol);

            // Check if the captured piece exists and belongs to the opponent
            if (capturedPiece != null && capturedPiece.Owner != piece.Owner) {
                // Capture the piece
                capturedPiece.IsTaken = true;
                Console.WriteLine("Capture successful!");
                return true;
            }

            // No capture available
            return false;
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
                if (targetPiece == null && capturedPiece != null && capturedPiece.Owner != CurrentTurn) {
                    captures.Add(capturedPiece);
                }
            }
        }

        /// <summary>
        /// Gets a list of mandatory captures for the specified player's pieces.
        /// </summary>
        /// <param name="player">The current player who is makeing a move.</param>
        /// <returns>A list of CheckerPieces representing mandatory captures for the specified player.</returns>
        private List<CheckerPiece> GetMandatoryCaptures(Player player) {
            List<CheckerPiece> mandatoryCaptures = new List<CheckerPiece>();

            foreach (var piece in Board.Where(p => p.Owner == player && !p.IsTaken)) {
                int startRow = piece.RowIndex;
                int startCol = piece.ColIndex;

                // Check for mandatory captures in the forward diagonal directions
                CheckForCaptureOptions(startRow + 2, startCol + 2, startRow + 1, startCol + 1, mandatoryCaptures);
                CheckForCaptureOptions(startRow + 2, startCol - 2, startRow + 1, startCol - 1, mandatoryCaptures);

                // If the piece is a king, also check the backward diagonal directions
                if (piece.IsKing) {
                    CheckForCaptureOptions(startRow - 2, startCol + 2, startRow - 1, startCol + 1, mandatoryCaptures);
                    CheckForCaptureOptions(startRow - 2, startCol - 2, startRow - 1, startCol - 1, mandatoryCaptures);
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
        public bool IsMoveValid(Player player, CheckerPiece currentPiece, int startRow, int startCol, int endRow, int endCol) {
            if (player != CurrentTurn) {
                Console.WriteLine($"Invalid move: It is player {CurrentTurn.UserName}s turn.");
                return false;
            }

            // Check if the piece exists and belongs to the current player
            if (currentPiece == null || currentPiece.Owner != player) {
                Console.WriteLine("Invalid move: The selected piece does not exist or does not belong to the current player.");
                return false;
            }

            // Werkt nog niet in combinatie met verplicht slaan
            //// Check for moving up and back for Player 1
            //if (player == PlayerOne && endRow <= startRow && !currentPiece.IsKing) {
            //    Console.WriteLine("Invalid move: Player 1 pieces can only move up.");
            //    return false;
            //}

            //// Check for moving down and back for Player 2
            //if (player == PlayerTwo && endRow >= startRow && !currentPiece.IsKing) {
            //    Console.WriteLine("Invalid move: Player 2 pieces can only move down.");
            //    return false;
            //}

            // Check for mandatory captures
            List<CheckerPiece> mandatoryCaptures = GetMandatoryCaptures(player);
            if (mandatoryCaptures.Count > 0) {
                // Check if the move includes a mandatory capture
                CheckerPiece capturedPiece = GetPieceAtPosition((startRow + endRow) / 2, (startCol + endCol) / 2);

                if (!mandatoryCaptures.Contains(capturedPiece)) {
                    Console.WriteLine("Invalid move: Mandatory captures must be performed.");
                    return false;
                }
            }

            //Check for not diagonal moves
            if ((startCol == endCol && Math.Abs(endRow - startRow) > 0) || (startRow == endRow && Math.Abs(endCol - startCol) > 0)) {
                Console.WriteLine($"Invalid move: Pieces can only me moved diagonally.");
                return false;
            }

            // Check for move to same location
            if (startRow == endRow || startCol == endCol) {
                Console.WriteLine("Invalid move: Pieces can not move to the same location.");
                return false;
            }

            // Check if the destination is a valid empty square
            if (IsSquareOccupied(endRow, endCol)) {
                Console.WriteLine("Invalid move: The destination square is already occupied.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a checker piece is eligible for king status based on its position.
        /// </summary>
        /// <param name="piece">The checker piece to be checked.</param>
        /// <returns>True if the piece is eligible for king status; otherwise, false.</returns>
        private bool CheckForKingMove(CheckerPiece piece) {
            return CurrentTurn == PlayerOne && (piece.RowIndex == BoardSize - 1) || CurrentTurn == PlayerTwo && (piece.RowIndex == 0);
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
        public bool MakeMove(Player player, int startRow, int startCol, int endRow, int endCol) {
            Console.WriteLine("Moving " + player.UserName);

            GameState = GameStateOptions.InProgress;

            if (IsGameOver()) {
                Console.WriteLine("Invlid move: Game is finnished, winner: " + Winner.UserName);
                return true;
            };

            var currentPiece = GetPieceAtPosition(startRow, startCol);

            if (!IsMoveValid(player, currentPiece, startRow, startCol, endRow, endCol)) {
                return false;
            }

            // Check if the piece is a king
            if (currentPiece.IsKing) {
                if (Math.Abs(endRow - startRow) != Math.Abs(endCol - startCol)) {
                    Console.WriteLine("Invalid move: The king can not move more than 2 steps diagonally and horizontally at the same time.");
                    return false;
                }

                int rowDirection = (endRow - startRow) / Math.Abs(endRow - startRow);
                int colDirection = (endCol - startCol) / Math.Abs(endCol - startCol);

                // Check for captures along the path
                while (startRow != endRow || startCol != endCol) {
                    // Move to the next square
                    startRow += rowDirection;
                    startCol += colDirection;

                    // Try to capture a piece along the way
                    if (!TryCapturePiece(currentPiece, startRow - rowDirection, startCol - colDirection, endRow, endCol)) {
                        // If no capture, check if the move is still valid
                        if (IsSquareOccupied(startRow, startCol)) {
                            Console.WriteLine("Invalid move: The king cannot jump over more than one piece at a time.");
                            return false;
                        }
                    }
                }
            }
            else {
                // For regular pieces, perform the regular capture logic
                // Check for a possible capture, capture if true
                if (!TryCapturePiece(currentPiece, startRow, startCol, endRow, endCol)) {
                    // If no capture check if the move is still valid
                    if (!currentPiece.IsKing && (Math.Abs(endRow - startRow) > 1 || Math.Abs(endCol - startCol) > 1)) {
                        Console.WriteLine("Invalid move: The piece can not move more than 2 steps diagonally.");
                        return false;
                    }
                }
            }

            // Move the piece to the destination
            currentPiece.RowIndex = endRow;
            currentPiece.ColIndex = endCol;

            // Check for king
            if (CheckForKingMove(currentPiece)) {
                currentPiece.IsKing = true;
                Console.WriteLine("King Piece");
            }

            // Update the current turn
            CurrentTurn = (player == PlayerOne) ? PlayerTwo : PlayerOne;

            if (IsGameOver()) {
                return true;
            };

            // Return true if the move is valid, false otherwise
            return true;
        }

        /// <summary>
        /// Checks if the game is over by determining if one of the players has no more pieces left on the board.
        /// Updates the game state and declares a winner if applicable.
        /// </summary>
        /// <returns>True if the game is over; otherwise, false.</returns>
        public bool IsGameOver() {
            // Check if one of the players has no more pieces
            if (Board.Where(piece => piece.Owner == PlayerOne).All(piece => piece.IsTaken == true)) {
                // PlayerTwo wins
                GameState = GameStateOptions.P2_Won;
                Winner = PlayerTwo;
                EndDateTime = DateTime.Now;
                return true;
            }
            else if (Board.Where(piece => piece.Owner == PlayerTwo).All(piece => piece.IsTaken == true)) {
                // PlayerOne wins
                GameState = GameStateOptions.P1_Won;
                Winner = PlayerOne;
                EndDateTime = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}
