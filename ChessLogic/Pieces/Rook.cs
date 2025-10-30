
namespace ChessLogic
{
    public class Rook : Piece
    {
        public override PieceType Type => PieceType.Rook;
        public override Player Color { get; }
        private Direction[] dirs =
        [
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West
        ];

        public Rook(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Rook copy = new(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDir(from, board, dirs).Select(to => new NormalMove(from, to));
        }
    }
}
