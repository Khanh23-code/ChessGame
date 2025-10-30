namespace ChessLogic
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        public Knight(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Knight copy = new(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static IEnumerable<Position> PotentialToPositions(Position from)        // Hàm này chỉ trả về các vị trí mà Mã có thể đi (chưa kiểm tra hợp lệ)
        {
            foreach (Direction vDir in new Direction[] { Direction.North, Direction.South }) {
                foreach (Direction hDir in new Direction[] { Direction.West, Direction.East })
                {
                    yield return from + vDir + 2 * hDir;
                    yield return from + hDir + 2 * vDir;
                }
            }
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            foreach (Position pos in PotentialToPositions(from))
            {
                if (!board.IsInside(pos)) continue;

                if (board.IsEmpty(pos) || board[pos].Color != Color) yield return pos;
            }

            // Src code:
            // return PotentialToPositions(from).Where(to => board.IsInside(to) && board.IsEmpty(to) || board[pos].Color != Color);
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositions(from, board).Select(to => new NormalMove(from, to));
        }
    }
}
