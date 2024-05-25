namespace Chess.Core.Logic.Pieces;

public class Pawn : Piece
{
    public Pawn(Color color) : base(color)
    {
        FenChar = Color == Color.White ? Logic.FenChar.WhitePawn : Logic.FenChar.NiggaPawn;
    }

    public sealed override char FenChar { get; set; }

    public override Coords[] Directions { get; set; } =
    {
        new(1, 0),
        new(2, 0),
        new(1, 1),
        new(1, -1)
    };

    private void SetBlackPawnDirection()
    {
        Directions = Directions.Select(coord => coord with { X = -1 * coord.X }).ToArray();
    }
}