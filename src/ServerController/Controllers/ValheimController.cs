using System.Net;
using Microsoft.AspNetCore.Mvc;
using ServerController.Interfaces;
using ServerController.Models;

namespace ServerController.Controllers
{
    /// <summary>
    /// ApiController for Valheim related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ValheimController : ControllerBase
    {
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<ValheimController> _logger;
        /// <summary>
        /// Injected ValheimService
        /// </summary>
        private readonly IValheimService _valheimService;

        /// <summary>
        /// Constructor to support dependency injections.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="valheimService">Injected valheim service</param>
        public ValheimController(ILogger<ValheimController> logger, IValheimService valheimService)
        {
            _logger = logger;
            _valheimService = valheimService;
        }

        /// <summary>
        /// Endpoint starts the Valheim server if it is not already running
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Server was successfully started.</response>
        /// <response code="401">Client was unauthorized</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "ValheimStart")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public IActionResult Start()
        {
            try
            {
                _valheimService.StartServer();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while starting the Valheim server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message }) 
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Endpoint stops the Valheim server if it is running.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Server was successfully stopped.</response>
        /// <response code="401">Client was unauthorized</response>
        /// <response code="405">Method not allowed</response>
        /// <response code="500">Internal server error</response>
        [HttpPost(Name = "ValheimStop")]
        [ProducesResponseType(typeof(InternalErrorResult), 500)]
        public async Task<IActionResult> Stop()
        {
            try
            {
                await _valheimService.StopServerAsync();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while stopping the Valheim server");
                return new ObjectResult(new InternalErrorResult { Message = ex.Message, Reason = ex.InnerException?.Message })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}