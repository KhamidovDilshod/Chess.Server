using Chess.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

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

app.MapGet("/", () => "Hello World!");
app.MapHub<HubBase>("/hub");
app.Run();