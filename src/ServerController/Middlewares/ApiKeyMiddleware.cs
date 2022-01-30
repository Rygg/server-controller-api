using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using ServerController.Configuration;

namespace ServerController.Middlewares
{
    /// <summary>
    /// Middleware handling API key authentication for all requests.
    /// </summary>
    public class ApiKeyMiddleware
    {
        /// <summary>
        /// Header name.
        /// </summary>
        private const string ApiKeyHeaderName = "ApiKey";
        /// <summary>
        /// Next action.
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<ApiKeyMiddleware> _logger;
        /// <summary>
        /// Dictionary of allowed applications.
        /// </summary>
        private readonly Dictionary<string, string> _allowedApplications;

        /// <summary>
        /// Constructor for the ApiKeyMiddleware.
        /// </summary>
        /// <param name="next">The next request delegate</param>
        /// <param name="configuration">Injected configuration</param>
        /// <param name="logger">Injected logger</param>
        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger, IOptions<List<AllowedClientsConfigurationSection>> configuration)
        {
            _next = next;
            _logger = logger;
            _allowedApplications = new Dictionary<string, string>();
            
            // Add allowed client applications.
            foreach (var apiKey in configuration.Value.Where(apiKey => apiKey.ClientAllowed))
            {
                _allowedApplications.Add(apiKey.ClientName, apiKey.ClientSecret);
            }
        }

        /// <summary>
        /// InvokeAsync is called for each request done to the API.
        /// </summary>
        /// <param name="context">Context for the request.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if api key existed:
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogError("ApiKey missing from request");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (!ValidateApiKey(extractedApiKey))
            {
                _logger.LogError("Received ApiKey was invalid.");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(context); // Continue to the request.
        }

        /// <summary>
        /// Method validates the received API key.
        /// </summary>
        /// <param name="extractedApiKey">The hashed api key received in the request.</param>
        /// <returns></returns>
        private bool ValidateApiKey(string extractedApiKey)
        {
            foreach (var (appName, appSecret) in _allowedApplications)
            {
                var hashedApiSecret = HashApiKey(appSecret);
                if (extractedApiKey == hashedApiSecret)
                {
                    _logger.LogInformation("Valid api key in request. Api key owner: {appName}", appName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Hash the stored api keys to provide a value for comparison. This step could be removed if the api keys were stored as hashed values.
        /// </summary>
        /// <param name="appSecret">Stored un-hashed secret</param>
        /// <returns></returns>
        private static string HashApiKey(string appSecret)
        {
            // Hashing is currently done without salting.
            var hashedAuthKeyBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(appSecret), new byte[16], 15000);
            var hashedAuthKey = Convert.ToBase64String(hashedAuthKeyBytes.GetBytes(32));
            return hashedAuthKey;
        }
    }
}
