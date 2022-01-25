using ServerController.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure the builder.
builder.Services.AddControllers(); // Controllers.
builder.Services.AddServices(); // Services.
builder.Services.ConfigureLogging(); // Configure logging.
builder.Services.AddEndpointsApiExplorer(); // Api description.
builder.Services.AddSwaggerGen(); // Swagger generator.

var app = builder.Build(); // Build the app.

if (app.Environment.IsDevelopment()) // Use swagger in development.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthorization();
app.MapControllers(); // Map the controllers.

app.Run(); // Run.
