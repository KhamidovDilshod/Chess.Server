using Chess.Core.Logic.Constants;

namespace Chess.Core.Logic.Pieces;

public class Knight : Piece
{
    public Knight(Color pieceColor) : base(pieceColor)
    {
    }

    protected override char FenChar() =>
        PieceColor == Color.White ? Constants.FenChar.WhiteKnight : Constants.FenChar.NiggaKnight;

    protected override Coords[] Directions()
        => new[]
        {
            new Coords(1, 2),
            new Coords(1, -2),
            new Coords(-1, 2),
            new Coords(-1, -2),
            new Coords(2, 1),
            new Coords(2, -1),
            new Coords(-2, 1),
            new Coords(-2, -1)
        };
}