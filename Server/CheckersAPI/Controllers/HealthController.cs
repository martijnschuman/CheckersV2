using CheckersAPI.Models.Checkers.Response;
using Microsoft.AspNetCore.Mvc;

namespace CheckersAPI.Controllers {
    public class HealthController : Controller {
        [HttpGet]
        [Route("/")]
        [ProducesResponseType(typeof(CheckerResponseModel), 200)]
        public IActionResult Index() {
            return Ok(new { status = "ok" });
        }
    }
}
