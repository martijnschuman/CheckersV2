using CheckersTest;
using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models {
    public class CheckerMove {
        [Key]
        [Required]
        public int CheckerMoveId { get; set; }
        public Player Player { get; set; }

        [Required]
        public int StartRow { get; set; }
        [Required]
        public int StartCol { get; set; }
        [Required]
        public int EndRow { get; set; }
        [Required]
        public int EndCol { get; set; }

        // Foreign key relationship
        [Required]
        public CheckerGame CheckerGame { get; set; }

        public CheckerMove(Player player, int startRow, int startCol, int endRow, int endCol, CheckerGame checkerGame) {
            Player = player;
            StartRow = startRow;
            StartCol = startCol;
            EndRow = endRow;
            EndCol = endCol;
            CheckerGame = checkerGame;
        }
    }
}
