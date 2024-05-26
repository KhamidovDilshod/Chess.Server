using Chess.Core.Logic;
using Microsoft.Extensions.Logging;

namespace Chess.Core.SignalR;

public class GameConnectionHandler : HubBase
{
    public GameConnectionHandler(ILogger<HubBase> logger) : base(logger)
    {
    }

    public async ValueTask CreateGameAsync(long userId,Color? playerColor)
    {
        
    }

    public async ValueTask JoinGameAsync()
    {
        
    }

    public async ValueTask LeaveGameAsync()
    {
        
    }
}