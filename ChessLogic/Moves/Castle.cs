namespace ChessLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; }
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public Castle(MoveType type, Position kingPos)
        {
            Type = type;
            FromPos = kingPos;

            if (type == MoveType.CaslteKS)      // Nhập thành phải
            {
                kingMoveDir = Direction.East;
                rookFromPos = kingPos + 3 * kingMoveDir;
            }
            else                               // Nhập thành trái
            {
                kingMoveDir = Direction.West;
                rookFromPos = kingPos + 4 * kingMoveDir;
            }

            ToPos = kingPos + 2 * kingMoveDir;
            rookToPos = kingPos + kingMoveDir;
        }

        public override void Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            new NormalMove(rookFromPos, rookToPos).Execute(board);

            //Piece king = board[FromPos];
            //board[ToPos] = king;
            //board[FromPos] = null;
            //king.HasMoved = true;

            //Piece rook = board[rookFromPos];
            //board[rookToPos] = rook;
            //board[rookFromPos] = null;
            //rook.HasMoved = true;
        }

        public override bool IsLegal(Board board)
        {
            // Luật: Bước nhập thành sẽ chỉ hợp lệ nếu:
            // + Vua và xe đều chưa di chuyển, mọi vị trí giữa đều trống (đã kiểm tra trong King class)
            // + Vua đang không bị chiếu (trước khi nhập)
            // + 2 vị trí vua đi qua (vị trí vua sau nhập thành và vị trí giữa nó với vị trí ban đầu) đều không trong tình trạng chiếu

            Player player = board[FromPos].Color;

            if (board.IsInCheck(player)) return false;
            
            Board copyBoard = board.Copy();
            Position kingPosInCopy = FromPos;

            for (int i = 0; i < 2; i ++)
            {
                new NormalMove(kingPosInCopy, kingPosInCopy + kingMoveDir).Execute(copyBoard);
                kingPosInCopy += kingMoveDir;

                if (copyBoard.IsInCheck(player)) return false;
            }

            return true;
        }
    }
}
