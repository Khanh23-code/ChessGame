namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;

        private int noCaptureOrPawnMoves;      // Biến đếm hỗ trợ 50-move rule

        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;

            noCaptureOrPawnMoves = 0;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board).Where(move => move.IsLegal(Board));
        }

        public void MakeMove (Move move)        // Hàm dùng để thực thi <move>
        {
            Board.SetPawnSkipPostion(CurrentPlayer, null);      // Reset pawnSkipPosition moi luot

            bool captureOrPawn = move.Execute(Board);

            {
                noCaptureOrPawnMoves = 0;
            }
            else
            {
                noCaptureOrPawnMoves++;
            }

            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            return Board.PiecePositionsFor(player).SelectMany(pos => LegalMovesForPiece(pos));

            //IEnumerable<Move> moves = Board.PiecePositionsFor(player).SelectMany(pos =>
            //{
            //    Piece piece = Board[pos];
            //    return piece.GetMoves(pos, Board);
            //});

            //return moves.Where(move => move.IsLegal(Board));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                if (Board.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.InsufficentMaterial);

                    //Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (Board.IsInsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficentMaterial);
            }
            else if (noCaptureOrPawnMoves >= 100)
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
        }

        public bool IsGameOver()
        {
            return Result != null;
        }
    }
}
