namespace Chess.Core.Constants;

public static class HubMethods
{
    public const string CreateGame = nameof(CreateGame);

    #region Listening methods

    public const string JoinGame = nameof(JoinGame);
    public const string LeaveGame = nameof(LeaveGame);
    public const string Move = nameof(Move);

    #endregion

    #region Push methods

    public const string Notification = nameof(Notification);
    public const string Joined = nameof(Joined);
    public const string Left = nameof(Left);
    public const string Moved = nameof(Moved);

    #endregion
}