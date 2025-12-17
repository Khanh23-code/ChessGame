namespace ChessLogic
{
    public class PawnPromotion : Move
    {
        public override MoveType Type => MoveType.PawnPromotion;
        public override Position FromPos { get; }
        public override Position ToPos { get; }
        public readonly PieceType newType;

        public PawnPromotion(Position fromPos, Position toPos, PieceType newType)
        {
            FromPos = fromPos;
            ToPos = toPos;
            this.newType = newType;
        }

        private Piece CreatePromotionPiece(Player color)
        {
            return newType switch
            {
                PieceType.Queen => new Queen(color),
                PieceType.Rook => new Rook(color),
                PieceType.Knight => new Knight(color),
                _ => new Bishop(color),
            };
        } 

        public override bool Execute(Board board)
        {
            Piece pawn = board[FromPos];
            board[FromPos] = null;

            Piece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true;

            board[ToPos] = promotionPiece;

            return true;
        }
    }
}
