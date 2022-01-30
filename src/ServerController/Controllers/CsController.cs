using System.Net;
using Microsoft.AspNetCore.Mvc;
using ServerController.Interfaces;
using ServerController.Models;

namespace ServerController.Controllers
{
    /// <summary>
    /// ApiController for Counter-Strike related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CsController : ControllerBase
    {
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<CsController> _logger;
        /// <summary>
        /// Injected CounterStrikeService
        /// </summary>
        private readonly ICounterStrikeService _counterStrikeService;

        /// <summary>
        /// Constructor to support dependency injections.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="counterStrikeService">Injected counter strike service</param>
        public CsController(ILogger<CsController> logger, ICounterStrikeService counterStrikeService)
        {
            _logger = logger;
            _counterStrikeService = counterStrikeService;
        }

        /// <summary>
        /// Endpoint starts the CS:GO server if it is not already running
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Server was successfully started.</response>
        /// <response code="401">Client was unauthorized</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "CounterStrikeStart")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Start()
        {
            try
            {
                _counterStrikeService.StartServer();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while starting the Counter-Strike server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Endpoint stops the CS:GO server if it is running.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Server was successfully stopped.</response>
        /// <response code="401">Client was unauthorized</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "CounterStrikeStop")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public async Task<IActionResult> Stop()
        {
            try
            {
                await _counterStrikeService.StopServerAsync();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while stopping the Counter-Strike server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}