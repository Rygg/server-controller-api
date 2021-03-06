using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Serilog;
using ServerController.Configuration;
using ServerController.Interfaces;
using ServerController.Services;

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
        internal static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(); // Add controllers.
            builder.Services.AddEndpointsApiExplorer(); // Api explorer. 
            builder.Services.AddRouting(options => options.LowercaseUrls = true); // Use lowercase urls because I like them.

            builder.Services.AddSingleton<IAssettoCorsaService, AssettoCorsaService>(); // Add assetto corsa service.
            builder.Services.AddSingleton<ICounterStrikeService, CounterStrikeService>(); // Add counter-strike service.
            builder.Services.AddSingleton<IValheimService, ValheimService>(); // Add valheim service.

            builder.Services.AddOptions(); // Add options functionality and read configuration sections.
            builder.Services.Configure<AssettoCorsaConfigurationSection>(builder.Configuration.GetSection(AssettoCorsaConfigurationSection.DefaultConfigurationSectionName));
            builder.Services.Configure<CounterStrikeConfigurationSection>(builder.Configuration.GetSection(CounterStrikeConfigurationSection.DefaultConfigurationSectionName));
            builder.Services.Configure<ValheimConfigurationSection>(builder.Configuration.GetSection(ValheimConfigurationSection.DefaultConfigurationSectionName));
            builder.Services.Configure<List<ApplicationClientsConfigurationSection>>(builder.Configuration.GetSection(ApplicationClientsConfigurationSection.DefaultConfigurationSectionName));
        }

        /// <summary>
        /// Configure the application logging.
        /// </summary>
        /// <param name="builder"></param>
        internal static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            // Add loggers.
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Configuration, "Logging:Serilog")
                        .CreateLogger());
            });
            // Configure http logging.
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestBody | HttpLoggingFields.RequestMethod | 
                                        HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.ResponseBody;
            });
        }

        /// <summary>
        /// Configure swagger document.
        /// </summary>
        /// <param name="builder"></param>
        internal static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Server Controller API",
                    Description = "Web API to control multiple game servers located on the server. Implemented using ASP.NET Core 6.",
                });
                
                // Using System.Reflection: Get generated xml documentation and include it in the swagger generation options.
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }
    }
}
