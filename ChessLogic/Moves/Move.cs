namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }

        public abstract void Execute(Board board);

        public virtual bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;

            Board copyBoard = board.Copy();
            Execute(copyBoard);

            return !copyBoard.IsInCheck(player);
        }
    }
}
