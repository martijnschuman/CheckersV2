using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models.Checkers {
    public class CheckerJoinModel {
        [Required]
        public required int GameCode { get; set; }
    }
}
