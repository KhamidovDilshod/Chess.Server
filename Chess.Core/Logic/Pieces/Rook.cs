namespace Chess.Core.Logic.Pieces;

public class Rook : Piece
{
    public bool HasMoved = false;

    public Rook(Color pieceColor) : base(pieceColor)
    {
        FenChar = pieceColor == Color.White ? Logic.FenChar.WhiteRook : Logic.FenChar.NiggaRook;
    }

    public sealed override char FenChar { get; set; }

    public override Coords[] Directions { get; set; } =
    [
        new Coords(1, 0),
        new Coords(-1, 0),
        new Coords(0, 1),
        new Coords(0, -1)
    ];
}