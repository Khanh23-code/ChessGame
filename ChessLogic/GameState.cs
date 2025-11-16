namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;

        // Variables for timer
        public TimeSpan WhiteTime { get; private set; }
        public TimeSpan BlackTime { get; private set; }

        private int noCaptureOrPawnMoves;      // Biến đếm hỗ trợ 50-move rule

        // Hien tai chi luu 1 stateString tai 1 thoi diem va dem so lan lap lai cua stateString do => phuc vu Threefold rule
        // => Mo rong: Luu tat ca stateString de truy vet nuoc di
        private string stateString;
        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();

<<<<<<< Updated upstream
        public GameState(Player player, Board board)
=======
        #region Constructors
        public GameState(Player player, Board board, TimeSpan initialTime)
>>>>>>> Stashed changes
        {
            CurrentPlayer = player;
            Board = board;

            WhiteTime = initialTime;
            BlackTime = initialTime;

            noCaptureOrPawnMoves = 0;

            stateString = new StateString(CurrentPlayer, Board).ToString();
            stateHistory[stateString] = 1;
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

            if (captureOrPawn)      // Cap nhat bien dem => 50-move Rule
            {
                noCaptureOrPawnMoves = 0;

                stateHistory.Clear();   
                // Voi nhung move thuc hien capture hoac pawn's move thi trang thai ca ban co se khong the lap lai trang thai truoc do
                // => Co the xoa stateHistory de lam trong bo nho
            }
            else
            {
                noCaptureOrPawnMoves++;
            }
            
            UpdateStateString();    // Update stateString => Threefold Rule

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
                    Result = Result.Draw(EndReason.Stalemate);
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
            else if (stateHistory[stateString] >= 3)
            {
                Result = Result.Draw(EndReason.ThreefoldRepetition);
            }
        }
<<<<<<< Updated upstream
=======
        public void EndGame(Result result)
        { 
            Result = result;
        }
        #endregion
>>>>>>> Stashed changes

        public bool IsGameOver()
        {
            return Result != null;
        }

        private void UpdateStateString()
        {
            stateString = new StateString(CurrentPlayer, Board).ToString();

            if (!stateHistory.ContainsKey(stateString))
            {
                stateHistory[stateString] = 1;
            }
            else
            {
                stateHistory[stateString]++;
            }
        }
<<<<<<< Updated upstream
=======
        #endregion

        #region Timer
        public void Tick()
        {
            if (IsGameOver())
            {
                return;
            }
            if (CurrentPlayer == Player.White)
            {
                WhiteTime = WhiteTime.Subtract(TimeSpan.FromSeconds(1));
            }
            else
            {
                BlackTime = BlackTime.Subtract(TimeSpan.FromSeconds(1));
            }
            if (WhiteTime == TimeSpan.Zero || BlackTime == TimeSpan.Zero)
            {
                Result timeoutResult = (WhiteTime == TimeSpan.Zero)
                                        ? new Result(Player.Black, EndReason.Timeout)
                                        : new Result(Player.White, EndReason.Timeout);

                EndGame(timeoutResult); 
            }
        }
        #endregion
>>>>>>> Stashed changes
    }
}
