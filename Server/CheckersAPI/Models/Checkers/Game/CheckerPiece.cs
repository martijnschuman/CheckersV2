using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckersAPI.Models.Checkers.Game {
    public class CheckerPiece {
        [Key]
        [Required]
        public int CheckerPieceId { get; set; }

        [Required]
        public bool IsKing { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public bool IsTaken { get; set; }

        [Required]
        public int RowIndex { get; set; }

        [Required]
        public int ColIndex { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int PlayerNumber { get; set; }

        // Saves creation date in db
        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public CheckerPiece() {
            UpdatedAt = DateTime.Now;
        }
    }
}
