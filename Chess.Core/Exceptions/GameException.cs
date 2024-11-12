namespace Chess.Core.Exceptions;

public class GameException(Codes code) : Exception(code.ToString());

[Flags]
public enum Codes
{
    PlayerNotInGame,
    PlayerDoesntHavePermissionToMove,
    BoardNotFound,
    
}