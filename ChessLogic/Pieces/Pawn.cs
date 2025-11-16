namespace ChessLogic
{
    public class Pawn : Piece
    {
        public override PieceType Type => PieceType.Pawn; // public override PieceType Type { get { return PieceType.Pawn; } }
        public override Player Color { get; }
        private Direction forward;

        public Pawn(Player color)
        {
            Color = color;

            // Thiết lập hướng di chuyển mặc định cho tốt dựa vào màu
            if (Color == Player.White)
            {
                forward = Direction.North;
            }
            else
            {
                forward = Direction.South;
            }
        }

        public override Piece Copy()
        {
            Pawn copy = new(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        #region Supportive Functions
        private static bool CanMoveTo(Position pos, Board board)        // Kiểm tra tính hợp lệ đẻ di chuyển: ô có nằm trong phạm vị board và có quân khác ở đó không
        {
            return board.IsInside(pos) && board.IsEmpty(pos);
        }

        private bool CanCaptureAt(Position pos, Board board)
        {
            if (!board.IsInside(pos) || board.IsEmpty(pos)) return false;

            Piece piece = board[pos];
            if (piece.Color != Color) return true;
            return false;
        }
        #endregion

        #region Get Forward/Diagonal Moves
        private IEnumerable<Move> ForwardMoves(Position from, Board board)      // Bao gồm di chuyển 1 và 2 bước đầu (note: Bên MoveType có type khác cho 2 bước đầu?)
        {
            Position oneMovePos = from + forward;

            if (CanMoveTo(oneMovePos, board))
            {
                // Kiểm tra PawnPromotion
                if (oneMovePos.Row == 0 || oneMovePos.Row == 7)
                {
                    foreach (Move move in PromotionMoves(from, oneMovePos))
                    {
                        yield return move;
                    }
                }
                else
                {
                    yield return new NormalMove(from, oneMovePos);
                }

                Position twoMovePos = oneMovePos + forward;
                if (!HasMoved && CanMoveTo(twoMovePos, board)) yield return new DoublePawn(from, twoMovePos);
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from, Board board)     // Ăn quân chéo
        {
            Position leftPos = from + (forward + Direction.West);
            Position rightPos = from + (forward + Direction.East);

            // Kiem tra EnPassant
            if (leftPos == board.GetPawnSkipPosition(Color.Opponent()))
            {
                yield return new EnPassant(from, leftPos);
            }

            else if (CanCaptureAt(leftPos, board))
            {
                // Kiểm tra PawnPromotion
                if (leftPos.Row == 0 || leftPos.Row == 7)
                {
                    foreach (Move move in PromotionMoves(from, leftPos))
                    {
                        yield return move;
                    }
                }
                else
                {
                    yield return new NormalMove(from, leftPos);
                }
            }

            // Kiem tra EnPassant
            if (rightPos == board.GetPawnSkipPosition(Color.Opponent()))
            {
                yield return new EnPassant(from, rightPos);
            }

            else if (CanCaptureAt(rightPos, board))
            {
                // Kiểm tra PawnPromotion
                if (rightPos.Row == 0 || rightPos.Row == 7)
                {
                    foreach (Move move in PromotionMoves(from, rightPos))
                    {
                        yield return move;
                    }
                }
                else
                {
                    yield return new NormalMove(from, rightPos);
                }
            }
        }
        #endregion

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMoves(from, board).Concat(DiagonalMoves(from, board));
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return DiagonalMoves(from, board).Any(move =>
            {
                Piece piece = board[move.ToPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }

        private IEnumerable<Move> PromotionMoves(Position from, Position to)
        {
            yield return new PawnPromotion(from, to, PieceType.Queen);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
        }
    }
}
