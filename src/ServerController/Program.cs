using ServerController.Extensions;

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

app.UseHttpLogging(); // Use the configured http logging.
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthorization();
app.MapControllers(); // Map the controllers.

app.Run(); // Run the application.