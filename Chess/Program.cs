using Chess;
using Chess.Core.Extensions;
using Chess.Core.SignalR;
using Chess.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Host.UseSerilog(ServiceRegistrationExt.ConfigureLogging);
builder.Services
    .AddDatabase(builder.Configuration)
    .AddManagers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
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
builder.Services.AddAuthentication();

builder.Services.AddGoogleAuth(builder.Configuration);

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.AddGameEndpoints();
app.AddAuthEndpoints();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.MapHub<HubBase>("/hub");
app.MapHub<GameHub>("/hub/game");
app.Run();