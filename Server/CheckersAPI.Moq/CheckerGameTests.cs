using CheckersAPI.Models.Checkers.Game;

namespace CheckersAPI.Moq {
    public class CheckerGameTests {
        // Default method which can be used to quickly create an instance of the game
        private CheckerGame CreateGame() {
            var playerOneId = "Player1";
            var gameId = 1;

            var game = new CheckerGame(playerOneId) {
                GameId = gameId,
                CurrentPlayerTurnId = playerOneId,
                StartDateTime = DateTime.Now,
            };

            return game;
        }

        // Test if the game gets setup correctly
        [Fact]
        public void InitializeGame_SetsUpBoardCorrectly() {
            // Arrange
            var game = CreateGame();
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;

            // Act
            game.InitializeGame();

            // Assert
            // Assert that the board has been initialized with the correct number of pieces
            Assert.Equal(40, game.Board.Count);


            // Assert that each player has the correct number of pieces
            Assert.Equal(20, game.Board.FindAll(piece => piece.OwnerId == game.PlayerOneId).Count);
            Assert.Equal(20, game.Board.FindAll(piece => piece.OwnerId == game.PlayerTwoId).Count);
        }


        // Test if a player can join an already existing game
        [Fact]
        public void JoinGame_JoinValid() {
            // Arrange
            var game = CreateGame();

            // Act
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;

            // Assert that the second player joined correctly
            Assert.Equal(playerTwoId, game.PlayerTwoId);
        }

        // Tests if a player can make a valid move
        [Fact]
        public void MakeMove_MoveIsValid() {
            // Arrange
            var game = CreateGame();
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;
            game.InitializeGame();

            // Act
            var result = game.MakeMove(game.PlayerOneId, 3, 0, 4, 1);

            // Assert
            // Assert the move went succesful
            Assert.True(result.IsValid);

            // Assert that the game started
            Assert.Equal(CheckerGame.GameStateOptions.InProgress, game.GameState);

            // Assert that the current turn changed to playerTwo
            Assert.Equal(game.PlayerTwoId, game.CurrentPlayerTurnId);
        }

        // Test if a player can move a piece while it's not theire turn
        [Fact]
        public void MakeMove_MoveInValidNotCurrentPlayersTurn() {
            // Arrange
            var game = CreateGame();
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;
            game.InitializeGame();

            // Act
            var result = game.MakeMove(game.PlayerTwoId, 6, 1, 5, 0);

            // Assert
            // Assert the move went succesful
            Assert.False(result.IsValid);
            Assert.Equal("ERROR", result.Status);
            Assert.Equal($"Invalid move: It is player {game.PlayerOneId}s turn.", result.Message);

            // Assert that the game started
            Assert.Equal(CheckerGame.GameStateOptions.InProgress, game.GameState);

            // Assert that the current turn changed to playerTwo
            Assert.Equal(game.PlayerOneId, game.CurrentPlayerTurnId);
        }

        // Test if a player has to make a manditory capture
        [Fact]
        public void MakeMove_ManidotryCaptureMustBePerformed() {
            // Arrange
            var game = CreateGame();
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;
            game.InitializeGame();

            // Act
            // Make a couple of moves
            game.MakeMove(game.PlayerOneId, 3, 0, 4, 1);
            game.MakeMove(game.PlayerTwoId, 6, 1, 5, 0);
            game.MakeMove(game.PlayerOneId, 3, 2, 4, 3);

            // P2 has to hit but doesn't
            var result = game.MakeMove(game.PlayerTwoId, 6, 9, 5, 8);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("ERROR", result.Status);
            Assert.Equal($"Invalid move: Mandatory captures must be performed.", result.Message);
        }

        // Test if a player can make a manditory capture
        [Fact]
        public void MakeMove_ManidotryCaptureIsPerformed() {
            // Arrange
            var game = CreateGame();
            var playerTwoId = "Player2";
            game.PlayerTwoId = playerTwoId;
            game.InitializeGame();

            // Act
            // Make a couple of moves
            game.MakeMove(game.PlayerOneId, 3, 0, 4, 1);
            game.MakeMove(game.PlayerTwoId, 6, 1, 5, 0);
            game.MakeMove(game.PlayerOneId, 3, 2, 4, 3);

            // P2 has to hit and does so
            var result = game.MakeMove(game.PlayerTwoId, 5, 0, 3, 2);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal("SUCCESS", result.Status);
            Assert.Equal(19, game.Board.FindAll(piece => piece.OwnerId == game.PlayerOneId && piece.IsTaken == false).Count);
            Assert.Equal(20, game.Board.FindAll(piece => piece.OwnerId == game.PlayerTwoId && piece.IsTaken == false).Count);
            Assert.Equal(game.CurrentPlayerTurnId, game.PlayerOneId);
        }
    }
}
