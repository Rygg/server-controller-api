using Microsoft.AspNetCore.Mvc;
using ServerController.Interfaces;

namespace ServerController.Controllers
{
    /// <summary>
    /// ApiController for AssettoCorsa related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AssettoController : ControllerBase
    {
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<AssettoController> _logger;
        /// <summary>
        /// Injected AssettoCorsaService
        /// </summary>
        private readonly IAssettoCorsaService _assettoCorsaService;

        /// <summary>
        /// Constructor to support dependency injections.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="assettoCorsaService">Injected AssettoCorsaService</param>
        public AssettoController(ILogger<AssettoController> logger, IAssettoCorsaService assettoCorsaService)
        {
            _logger = logger;
            _assettoCorsaService = assettoCorsaService;
        }

        /// <summary>
        /// Endpoint retrieves all available tracks and track configurations available on the Assetto Corsa server.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">All available track names and track combinations.</response>
        /// <response code="500">Internal server error</response>
        [HttpGet(Name = nameof(Tracks))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> Tracks()
        {
            try
            {
                _logger.LogDebug("Retrieving tracks from service");
                var tracks = _assettoCorsaService.GetTracksAsync();
                return new OkObjectResult(tracks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving tracks.");
                throw;
            }
        }
    }
}