using Chess.Core.Extensions;
using Chess.Core.SignalR;
using Chess.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(p => p.ListenAnyIP(5000));
builder.Services.AddSignalR();
builder.Host.UseSerilog(ServiceRegistrationExt.Configure);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddManagers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .WithOrigins("http://localhost:4200")
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
app.Run();