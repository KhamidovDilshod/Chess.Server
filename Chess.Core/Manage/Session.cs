namespace Chess.Core.Manage;

public sealed class Session(Guid gameId)
{
    public Guid GameId { get; set; } = gameId;
    private readonly Dictionary<Guid, string> _players = new(); //Key: UserId, value: ConnectionId

    public void AddPlayer(Guid userId, string connectionId) => _players.TryAdd(userId, connectionId);

    public void RemovePlayer(Guid userId)
    {
        if (IsPlayerInGame(userId))
        {
            _players.Remove(userId);
        }
    }

    public bool IsPlayerInGame(Guid userId) => _players.ContainsKey(userId);

    public string? ConnectionId(Guid userId) =>
        _players.TryGetValue(userId, out var connectionId) ? connectionId : null;
}