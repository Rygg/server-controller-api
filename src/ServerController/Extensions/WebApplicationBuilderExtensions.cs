using Serilog;

namespace ServerController.Extensions
{
    /// <summary>
    /// Contains extensions for the app
    /// </summary>
    internal static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the used services.
        /// </summary>
        /// <param name="builder"></param>
        public static void AddServices(this WebApplicationBuilder builder)
        {

        }

        /// <summary>
        /// Configure the application logging.
        /// </summary>
        /// <param name="builder"></param>
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddConsole();
                logBuilder.AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Configuration, "Logging:Serilog")
                        .CreateLogger());
            });
            
        }
    }
}
