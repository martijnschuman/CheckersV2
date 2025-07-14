namespace CheckersAPI.Models.Auth {
    public class AuthResponseModel {
        public string? Status { get; set; }
        public string? Message { get; set; }

        public string? MultiFactorKey { get; set; }
        public string? MultiFactoryKeyQR { get; set; }
    }
}
