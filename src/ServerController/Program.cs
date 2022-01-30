using ServerController.Extensions;
using ServerController.Middlewares;

var builder = WebApplication.CreateBuilder(args); // Create the builder.
builder.ConfigureServices(); // Configure services.
builder.ConfigureLogging(); // Configure logging.
builder.ConfigureSwagger(); // Configure swagger.

var app = builder.Build(); // Build the app.

if (app.Environment.IsDevelopment()) // Set development environment specific options.
{
    app.UseSwagger(); // Use swagger in development.
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<ApiKeyMiddleware>(); // API keys are not required in development.
}

app.UseHttpLogging(); // Use the configured http logging.
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthorization();
app.MapControllers(); // Map the controllers.

app.Run(); // Run the application.