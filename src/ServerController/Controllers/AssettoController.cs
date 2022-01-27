using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ServerController.Interfaces;
using ServerController.Models;
using ServerController.Models.AssettoController;

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
        [HttpGet(Name = "AssettoCorsaTracks")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Tracks()
        {
            try
            {
                _logger.LogDebug("Retrieving tracks from service");
                var tracks = _assettoCorsaService.GetTracks();
                return new OkObjectResult(tracks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while retrieving tracks.");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message})
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Endpoint starts the Assetto Corsa server if it is not already running. Optional TrackConfiguration supported in the request body.
        /// </summary>
        /// <param name="trackConfiguration">Optional track configuration request body.</param>
        /// <returns></returns>
        /// <response code="200">Server was successfully started.</response>
        /// <response code="400">Bad request</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "AssettoCorsaStart")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Start([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TrackConfiguration? trackConfiguration)
        {
            try
            {
                _assettoCorsaService.StartServer(trackConfiguration);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while starting the Assetto Corsa server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message }) 
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Endpoint stops the Assetto Corsa server if it is running. After this, the server is started with the optional parameters parsed from the request body.
        /// </summary>
        /// <param name="trackConfiguration">Optional track configuration request body.</param>
        /// <returns></returns>
        /// <response code="200">Server was successfully restarted.</response>
        /// <response code="400">Bad request</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "AssettoCorsaRestart")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Restart([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TrackConfiguration? trackConfiguration)
        {
            try
            {
                _assettoCorsaService.RestartServer(trackConfiguration);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restarting the Assetto Corsa server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Endpoint stops the Assetto Corsa server if it is running.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Server was successfully stopped.</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "AssettoCorsaStop")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Stop()
        {
            try
            {
                _assettoCorsaService.StopServer();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while stopping the Assetto Corsa server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}