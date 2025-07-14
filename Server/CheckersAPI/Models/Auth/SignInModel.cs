using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models.Auth {
    public class SignInModel {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Invalid or no MultiFactorKey")]
        public int? MultiFactorKey { get; set; }
    }
}
