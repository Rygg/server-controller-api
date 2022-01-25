using Microsoft.AspNetCore.Mvc;

namespace ServerController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;

        public TodoController(ILogger<TodoController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetIntegers")]
        public IEnumerable<int> Get()
        {
            _logger.LogDebug("Returning integers.");
            return Enumerable.Range(1, 5).ToArray();
        }
    }
}