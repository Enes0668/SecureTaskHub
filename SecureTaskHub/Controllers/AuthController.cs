using Microsoft.AspNetCore.Mvc;

namespace SecureTaskHub.Controllers
{
    [ApiController]  
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase 
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Merhaba!");
        }
    }
}