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
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllOrigins",
        configurePolicy: policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddGoogleAuth(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.AddGameEndpoints();
app.AddAuthEndpoints();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.MapHub<HubBase>("/hub");
app.MapHub<GameHub>("/game");
app.Run();