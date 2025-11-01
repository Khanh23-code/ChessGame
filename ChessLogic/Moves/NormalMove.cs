namespace ChessLogic
{
    public class NormalMove : Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        public NormalMove(Position fromPos, Position toPos)
        {
            this.FromPos = fromPos;
            this.ToPos = toPos;
        }

        public override bool Execute(Board board)
        {
            Piece piece = board[FromPos];
            board[ToPos] = piece;
            board[FromPos] = null;
            piece.HasMoved = true;

            return !board.IsEmpty(ToPos) || piece.Type == PieceType.Pawn;
        }
    }
}
