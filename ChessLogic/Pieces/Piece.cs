namespace ChessLogic
{
    public abstract class Piece     // Lớp cơ sở trừu tượng để xây dựng các lớp quân cờ
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;     // Theo như hiểu biết hiện tại: HashMoved để kiếm tra 1 quân cờ đã di chuyển trong CẢ VÁN CỜ chưa (phục vụ cho tốt đi 2 bước hoặc vua nhập thành)

        public abstract Piece Copy();
        public abstract IEnumerable<Move> GetMoves(Position from, Board board);     // Trả về các Move mà quân <Piece> có thể di chuyển từ <from>

        protected IEnumerable<Position> MovePositionsInDir(Position from, Board board, Direction dir)       // Trả về tất cả các ô theo MỘT hướng <dir> tính từ <from> (hỗ trợ các quân Rook, Queen, Bishop có thể di chuyển tất cả ô theo <dir>)
        {
            for (Position pos = from + dir; board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    yield return pos;
                    continue;
                }

                Piece piece = board[pos];

                if (piece.Color != Color) yield return pos;

                break;
            }
        }

        protected IEnumerable<Position> MovePositionsInDir(Position from, Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => MovePositionsInDir(from, board, dir));

            // Dưới là code nếu không sử dụng SelectMany
            //foreach (Direction dir in dirs)
            //{
            //    IEnumerable<Position> positionsInCurrentDir = MovePositionsInDir(from, board, dir);

            //    foreach (Position pos in positionsInCurrentDir)
            //    {
            //        yield return pos;
            //    }
            //}
        }

        public virtual bool CanCaptureOpponentKing(Position from, Board board)
        {
            return GetMoves(from, board).Any(move =>
            {
                Piece piece = board[move.ToPos];
                if (piece != null && piece.Type == PieceType.King) return true;
                return false;
            });
        }













    }
}
