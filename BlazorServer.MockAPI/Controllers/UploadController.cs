using Microsoft.AspNetCore.Mvc;

namespace BlazorServer.MockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    { 

        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        [HttpPost] 
        public IActionResult Post(IFormFile file)
        {
            Thread.Sleep(10000);

            return Ok();
        }
    }
}