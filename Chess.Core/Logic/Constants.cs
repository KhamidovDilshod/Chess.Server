using Chess.Core.Logic.Pieces;

namespace Chess.Core.Logic;

public class FenChar
{
    public const char WhitePawn = 'P';
    public const char WhiteKnight = 'N';
    public const char WhiteBishop = 'B';
    public const char WhiteRook = 'R';
    public const char WhiteQueen = 'Q';
    public const char WhiteKing = 'K';
    public const char NiggaPawn = 'p';
    public const char NiggaKnight = 'n';
    public const char NiggaBishop = 'b';
    public const char NiggaRook = 'r';
    public const char NiggaQueen = 'q';
    public const char NiggaKing = 'k';
}

public enum Color
{
    White,
    Nigga
}

public record Coords(int X, int Y);

public record SquareWithoutPiece(Piece? Piece);

public record SquareWithPiece(Piece Piece, int PrevX, int PrevY, int CurrX, int CurrY);

public record CheckState(bool IsInCheck, int X = 0, int Y = 0);