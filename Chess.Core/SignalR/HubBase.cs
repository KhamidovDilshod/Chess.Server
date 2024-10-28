using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class HubBase(ILogger<HubBase> logger) : Hub
{
    protected readonly ILogger<HubBase> Logger = logger;
    private readonly ConnectionMapping<string> _connections = new();

    protected string ConnectionId => Context.ConnectionId;
    private string Name => Context.User?.Identity?.Name ?? string.Empty;

    public async ValueTask SendMessage(string who, string message)
    {
        foreach (var connection in _connections.GetConnections(who))
        {
            await Clients.Client(connection).SendAsync("notification", $"{ConnectionId} : {message}");
        }
    }

    public override async Task OnConnectedAsync()
    {
        string name = Context.User.Identity?.Name ?? string.Empty;
        _connections.Add(name, ConnectionId);
        Logger.LogInformation("Client with Id:'{clientId}' connected", ConnectionId);
        await SendMessage(name, "Connected");
        await Clients.All.SendAsync("all", $"Client with id:'{ConnectionId}' connected");
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        string name = Context.User.Identity?.Name ?? string.Empty;
        _connections.Remove(name, ConnectionId);
        Logger.LogInformation("Client wit Id: '{clientId}' disconnected'", ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}