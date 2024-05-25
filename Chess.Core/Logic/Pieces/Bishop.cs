namespace Chess.Core.Logic.Pieces;

public class Bishop : Piece
{
    public Bishop(Color pieceColor) : base(pieceColor)
    {
        FenChar = Color == Color.White ? Logic.FenChar.WhiteBishop : Logic.FenChar.NiggaBishop;
    }


    public sealed override char FenChar { get; set; }

    public override Coords[] Directions { get; set; } = new[]
    {
        new Coords(1, 1),
        new Coords(1, -1),
        new Coords(-1, 1),
        new Coords(-1, -1)
    };
}