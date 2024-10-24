using Chess.Core.Extensions;
using Chess.Core.SignalR;
using Chess.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Host.UseSerilog(ServiceRegistrationExt.ConfigureLogging);
string connectionString = builder.Configuration.GetConnectionString("AppConfig")!; 

// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(connectionString);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddManagers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .WithOrigins(
            "http://localhost:4200",
            "http://localhost:5200",
            "https://white-dune-0f6592410.5.azurestaticapps.net"
        )
        .WithOrigins()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddGoogleAuth(builder.Configuration);

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.AddGameEndpoints();
app.AddAuthEndpoints();
app.MapHub<HubBase>("/hub");
app.MapHub<GameHub>("/game");
app.Run();
