using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models.Auth {
    public class MFASetupModel {
        [Required(ErrorMessage = "Invalid or no MultiFactorKey")]
        public int? MultiFactorKey { get; set; }
    }
}
