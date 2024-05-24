using Chess.Core.Logic.Constants;

namespace Chess.Core.Logic.Pieces;

public class Bishop : Piece
{
    public Bishop(Color pieceColor) : base(pieceColor)
    {
    }

    protected override char FenChar() =>
        PieceColor == Color.White ? Constants.FenChar.WhiteBishop : Constants.FenChar.NiggaBishop;

    protected override Coords[] Directions() => new[]
    {
        new Coords(1, 1),
        new Coords(1, -1),
        new Coords(-1, 1),
        new Coords(-1, -1)
    };
}