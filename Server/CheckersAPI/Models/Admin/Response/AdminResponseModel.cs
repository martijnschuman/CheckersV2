namespace CheckersAPI.Models.Admin.Response {
    public class AdminResponseModel {
        public string? Status { get; set; }
        public string? Message { get; set; }

        public object? CancelableGames { get; set; }
        public object? RemainingGames { get; set; }
    }
}
