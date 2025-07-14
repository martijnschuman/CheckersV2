using Microsoft.AspNetCore.Identity;

namespace CheckersAPI.Models {
    public class UserModel : IdentityUser {
        public string? MultiFactorPrivateKey { get; set; }
    }
}
