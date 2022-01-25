namespace ServerController.Extensions
{
    /// <summary>
    /// Contains extensions for the app
    /// </summary>
    internal static class WebApplicationExtensions
    {
        /// <summary>
        /// Add the used services.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddServices(this IServiceCollection serviceCollection)
        {

        }

        /// <summary>
        /// Configure the application logging.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ConfigureLogging(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddConsole();
            });
            
        }
    }
}
