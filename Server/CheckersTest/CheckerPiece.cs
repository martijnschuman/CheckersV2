using CheckersTest;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckersAPI.Models {
    public class CheckerPiece {
        [Key]
        [Required]
        public int CheckerPieceId { get; set; }

        [Required]
        public bool IsKing { get; set; }

        public Player Owner { get; set; }

        [Required]
        public bool IsTaken { get; set; }

        [Required]
        public int RowIndex { get; set; }

        [Required]
        public int ColIndex { get; set; }

        [Required]
        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public CheckerGame Game { get; set; }
    }
}
