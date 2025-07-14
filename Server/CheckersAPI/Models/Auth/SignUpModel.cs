using System.ComponentModel.DataAnnotations;

namespace CheckersAPI.Models.Auth {
    public class SignUpModel {
        [Required(ErrorMessage = "User Name is required")]
        [MinLength(1)]
        [MaxLength(60)]
        public string? Username { get; set; }

        [EmailAddress]
        [MinLength(1)]
        [MaxLength(60)]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [MinLength(12)]
        [MaxLength(128)]
        [DataType(DataType.Password)]
        [Password(
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
            RequiredUniqueChars = 1
        )]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Custom validation attribute for password
    /// </summary>
    public class PasswordAttribute : ValidationAttribute {
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public int RequiredUniqueChars { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var password = value as string;

            if (password == null) {
                return new ValidationResult("Password is required.");
            }

            if (RequireDigit && !password.Any(char.IsDigit)) {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (RequireLowercase && !password.Any(char.IsLower)) {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (RequireNonAlphanumeric && !password.Any(ch => !char.IsLetterOrDigit(ch))) {
                return new ValidationResult("Password must contain at least one non alphanumeric character.");
            }

            if (RequireUppercase && !password.Any(char.IsUpper)) {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (RequiredUniqueChars > 1 && password.Distinct().Count() < RequiredUniqueChars) {
                return new ValidationResult($"Password must contain at least {RequiredUniqueChars} unique characters.");
            }

            return ValidationResult.Success;
        }
    }
}
