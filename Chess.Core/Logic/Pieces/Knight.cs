namespace Chess.Core.Logic.Pieces;

public class Knight : Piece
{
    public Knight(Color pieceColor) : base(pieceColor)
    {
        FenChar = Color == Color.White ? Logic.FenChar.WhiteKnight : Logic.FenChar.NiggaKnight;
    }

    public sealed override char FenChar { get; set; }

    public override Coords[] Directions { get; set; }
        = {
            new(1, 2),
            new(1, -2),
            new(-1, 2),
            new(-1, -2),
            new(2, 1),
            new(2, -1),
            new(-2, 1),
            new(-2, -1)
        };
}