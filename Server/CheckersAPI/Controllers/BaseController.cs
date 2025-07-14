using CheckersAPI.Data;
using CheckersAPI.Hubs;
using CheckersAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CheckersAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase {
        public readonly DataContext _context;
        public readonly IHubContext<CheckersHub> _hubContext;

        public BaseController(DataContext context, IHubContext<CheckersHub> hubContext) {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get user from database
        /// </summary>
        /// <returns></returns>
        protected async Task<UserModel> GetUserFromDatabase(string userID) {
            // If no user ID in header, return null
            if (string.IsNullOrEmpty(userID)) {
                return null;
            }

            // Get user from database
            var user = await _context.Users.FindAsync(userID);
            return user;
        }
    }
}
