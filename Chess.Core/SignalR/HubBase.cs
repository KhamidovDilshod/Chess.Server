using Chess.Core.Manage;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class HubBase : Hub
{
    private readonly ILogger<HubBase> _logger;
    private readonly ConnectionMapping<string> _connections = new();

    public HubBase(ILogger<HubBase> logger)
    {
        _logger = logger;
    }

    protected string ConnectionId => Context.ConnectionId;
    protected string Name => Context.User.Identity?.Name ?? string.Empty;

    public async ValueTask SendMessage(string who, string message)
    {
        string name = Name;
        foreach (var connection in _connections.GetConnections(who))
        {
            await Clients.Client(connection).SendAsync("notification",name + ": " + message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        string name = Context.User.Identity?.Name ?? string.Empty;
        _connections.Add(name, ConnectionId);
        _logger.LogInformation("Client with Id:'{clientId}' connected", ConnectionId);
        await SendMessage(Name, $"Connected");
        await Clients.All.SendAsync("all",$"Client with id:'{ConnectionId}' connected");
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        string name = Context.User.Identity?.Name ?? string.Empty;
        _connections.Remove(name, ConnectionId);
        _logger.LogInformation("Client wit Id: '{clientId}' disconnected'", ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}