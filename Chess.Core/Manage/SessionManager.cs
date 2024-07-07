namespace Chess.Core.Manage;

public class SessionManager
{
    private readonly Dictionary<Guid, Session> _sessions = new();

    public void CreateSession(Guid gameId)
    {
        if (!_sessions.ContainsKey(gameId))
        {
            _sessions[gameId] = new Session(gameId);
        }
    }

    public void AddPlayerToGame(Guid gameId, Guid userId, string connectionId)
    {
        if (_sessions.TryGetValue(gameId, out var session))
        {
            session.AddPlayer(userId, connectionId);
        }
    }

    public void RemovePlayerFromGame(Guid gameId, Guid userId)
    {
        if (_sessions.TryGetValue(gameId, out var gameSession))
        {
            gameSession.RemovePlayer(userId);
        }
    }

    public void RemoveGameSession(Guid gameId)
    {
        if (_sessions.ContainsKey(gameId))
        {
            _sessions.Remove(gameId);
        }
    }

    public Session? GetSession(Guid gameId)
        => _sessions.TryGetValue(gameId, out var session) ? session : null;
}