namespace Chess.Core.Logic.Pieces;

public class King : Piece
{
    private bool _hasMoved = false;

    public King(Color pieceColor) : base(pieceColor)
    {
        FenChar = Color == Color.White ? Logic.FenChar.WhiteKing : Logic.FenChar.NiggaKing;
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