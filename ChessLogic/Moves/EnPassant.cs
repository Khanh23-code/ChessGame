namespace ChessLogic
{
    public class EnPassant : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position capturePosition;          // Đây là vị trí quân tốt sẽ bị bắt (ko phải vị trí skip)

        public EnPassant(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            capturePosition = new Position(from.Row, to.Column);
        }

        public override void Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);

            board[capturePosition] = null;
        }
    }
}
