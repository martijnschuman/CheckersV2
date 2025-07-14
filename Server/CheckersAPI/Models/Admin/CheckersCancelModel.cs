using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models.Admin {
    public class CheckersCancelModel {
        [Required]
        public int? GameId { get; set; }
    }
}
