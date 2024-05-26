using Chess.Core.Extensions;
using Chess.Core.SignalR;
using Chess.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
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

var app = builder.Build();
app.UseCors("CorsPolicy");
app.AddGameEndpoints();
app.AddAuthEndpoints();
app.MapHub<HubBase>("/hub");
app.Run();