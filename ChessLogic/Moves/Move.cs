namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }

        // Kiểu trả về của Execute là bool để hỗ trợ việc đếm cho 50 move rule: Nếu việc move thực hiện Execute() có ăn quân cờ hoặc là move của Tốt sẽ return true
        public abstract bool Execute(Board board);

        public virtual bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;

            Board copyBoard = board.Copy();
            Execute(copyBoard);

            return !copyBoard.IsInCheck(player);
        }
    }
}
