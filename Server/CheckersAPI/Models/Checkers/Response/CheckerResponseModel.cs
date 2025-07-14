using CheckersAPI.Models.Checkers.Game;

namespace CheckersAPI.Models.Checkers.Response {
    public class CheckerResponseModel {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public int? GameCode { get; set; }

        public bool IsValid { get; set; }

        public CheckerGame? CheckerGame { get; set; }
        public object? CheckerGames { get; set; }
        public object? CheckerGameStats { get; set; }

        public string? PlayerOneUserName { get; set; }
        public string? PlayerTwoUserName { get; set; }
        public string? CurrentPlayerUserName { get; set; }
    }
}
