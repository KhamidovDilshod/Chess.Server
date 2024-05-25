namespace Chess.Core.Logic.Pieces;

public class Queen : Piece
{
    public Queen(Color pieceColor) : base(pieceColor)
    {
        FenChar = Color == Color.White ? Logic.FenChar.WhiteQueen : Logic.FenChar.NiggaQueen;
    }

    public sealed override char FenChar { get; set; }

    public override Coords[] Directions { get; set; } =
    {
        new(0, 1),
        new(0, -1),
        new(1, 0),
        new(1, -1),
        new(1, 1),
        new(-1, 0),
        new(-1, 1),
        new(-1, -1)
    };
}