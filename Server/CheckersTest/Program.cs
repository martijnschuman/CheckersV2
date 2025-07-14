using CheckersAPI.Models;
using CheckersTest;

Player playerOne = new Player() { Id = 1, UserName = "Player1" };
Player playerTwo = new Player() { Id = 2, UserName = "Player2" };
CheckerGame checkerGame = new CheckerGame(playerOne, playerTwo) {
    GameCode = 1,
    GameState = CheckerGame.GameStateOptions.Open,
    StartDateTime = DateTime.Now,
};

checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 0, 3, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 3, 4, 2);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 1, 5, 3); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 2, 4, 4); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 4, 3, 3);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 5, 4, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 1, 2, 0); // illegaal
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 3, 5, 5); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 6, 4, 4); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 6, 3, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 4, 4, 2, 7); // p1 gaat maar p2 is aan de beurt
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 4, 2, 6); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 5, 3, 7); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 7, 7, 6, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 7, 5, 5); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 6, 4, 4); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 7, 3, 5); // p1 illegaal twee stappen
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 1, 3, 3); // p1 illegaal twee stappen
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 1, 2, 0);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 0, 4, 3); // p2 illegaal twee stappen
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 3, 5); // p2 illegaal twee stappen
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 5, 7); // p2 illegaal zelfde plek
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 6, 7); // p2 illegaal horizontaal
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 5, 6); // p2 illegaal verticaal
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 4, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 7, 2, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 4, 3, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 6, 3, 7); // p1 illegaal moet slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 6, 4, 4); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 6, 3, 7);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 0, 6, 1, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 7, 5, 6, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 5, 2, 4);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 3, 7, 2, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 4, 3, 3);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 2, 6, 1, 7);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 0, 3, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 1, 7, 0, 6); // p2 make king move
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 0, 0, 1, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 0, 6, 4, 3); // p2 king move illegaal
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 0, 6, 4, 2); // p2 king move, na p1 king slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 1, 5, 3); // p1 slaat king
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 4, 4, 2); // p2 slaat
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 3, 2, 4);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 1, 4, 0);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 0, 2, 1, 3);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 6, 6, 5, 7);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 4, 3, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 7, 3, 6, 4);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 3, 2, 4);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 2, 3, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 5, 4, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 3, 1, 1, 3); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 1, 2, 0); // p1 illegaal, moet slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 4, 0, 2); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 7, 3, 5); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 1, 2, 0);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 0, 3, 1); // p2 illegaal moet achterui slaan; bug
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 3, 5, 5, 3); // p2 moet illegaal slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 0, 2, 1, 3);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 3, 4, 2);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 0, 3, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 0, 2, 2); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 3, 3, 1); // p1 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 4, 2, 2, 0); // p2 slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 0, 4, 1, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 2, 0, 1, 1);
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 1, 5, 2, 6);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 1, 1, 0, 0); // p2 king move
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 2, 6, 3, 5);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 0, 0, 5, 5); // p2 king move
checkerGame.PrintGame();
checkerGame.MakeMove(playerOne, 3, 5, 4, 4);
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 5, 4, 6); // p2 king move, illegaal moet slaan
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 5, 5, 3, 3); // p2 king slaan - game finnished
checkerGame.PrintGame();
checkerGame.MakeMove(playerTwo, 3, 3, 2, 2); // p2 illegaal, game klaar
checkerGame.PrintGame();