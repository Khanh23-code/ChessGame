using System.Reflection.Metadata.Ecma335;

namespace ChessLogic
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King;
        public override Player Color { get; }

        private Direction[] dirs =
        [
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West,
            Direction.NorthEast,
            Direction.SouthEast,
            Direction.NorthWest,
            Direction.SouthWest
        ];

        public King(Player color)
        {
            Color = color;
        }

        private static bool IsUnmovedRook(Position pos, Board board)        // Hô trợ - kiểm tra nhập thành: Kiểm tra quân xe đã di chuyển (thông qua lớp King)
        {
            if (board.IsEmpty(pos)) return false;

            return board[pos].Type == PieceType.Rook && !board[pos].HasMoved;
        }

        private static bool AllEmpty(IEnumerable<Position> positions, Board board)      // Hỗ trợ - kiểm tra nhập thành: Kiểm tra nhiều vị trí có trống không? Dùng để kiểm tra các positions giữa vua và xe
        {
            return positions.All(pos => board.IsEmpty(pos));
        }

        private bool CanCastleKingSide(Position from, Board board)      // Kiểm tra nhập thành cánh vua (phải)
        {
            if (HasMoved) return false;

            Position rookPos = new Position(from.Row, 7);
            // Vì nhập thành phải nên xe ở cột 7 (cố định cho cả player black và white)
            // => Khi mở rộng để xoay bàn cờ: PHẢI truyền thêm tham số để xác định column

            Position[] betweenPositions = new Position[] { new Position(from.Row, 5), new Position(from.Row, 6) };
            // Cần mở rộng không được truyền giá trị cố định (5, 6)

            return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPositions, board);
        }

        private bool CanCastleQueenSide(Position from, Board board)      // Kiểm tra nhập thành cánh hậu (trái)
        {
            if (HasMoved) return false;

            Position rookPos = new Position(from.Row, 0);
            Position[] betweenPositions = new Position[] { new Position(from.Row, 1), new Position(from.Row, 2), new Position(from.Row, 3) };
            // Cần mở rộng tương tự trên

            return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPositions, board);
        }

        public override Piece Copy()
        {
            King copy = new(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)     // Lấy các vị trí di chuyển hợp lệ (NormalMove)
        {
            foreach (Direction dir in dirs)
            {
                Position to = from + dir;
                if (!board.IsInside(to)) continue;
                    
                if (board.IsEmpty(to) || board[to].Color != Color) yield return to;
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            foreach (Position to in MovePositions(from, board))
            {
                yield return new NormalMove(from, to);
            }
            // Ở đây không dùng Select vì sẽ cần thêm các nước đi khác

            if (CanCastleKingSide(from, board))
            {
                yield return new Castle(MoveType.CaslteKS, from);
            }

            if (CanCastleQueenSide(from, board))
            {
                yield return new Castle(MoveType.CaslteQS, from);
            }
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)     // Cần override bởi vì các nước nhập thành không thể ăn quân (chỉ nước bình thường mới chiếu được)
        {
            return MovePositions(from, board).Any(toPos =>
            {
                Piece piece = board[toPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
