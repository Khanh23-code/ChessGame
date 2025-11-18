using ChessLogic;
using System; 

namespace ChessLogic
{
    public class GameState
    {
        #region Properties
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;

        public TimeSpan WhiteTime { get; private set; }
        public TimeSpan BlackTime { get; private set; }

        private int noCaptureOrPawnMoves;
        private string stateString;
        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();

        public string FENString;
        #endregion

        #region Constructors
        public GameState(Player player, Board board, TimeSpan initialTime)
        {
            CurrentPlayer = player;
            Board = board;
            WhiteTime = initialTime;
            BlackTime = initialTime;

            noCaptureOrPawnMoves = 0;

            stateString = new StateString(CurrentPlayer, Board).ToString();
            stateHistory[stateString] = 1;
        }
        #endregion

        #region Make Move
        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPostion(CurrentPlayer, null);

            bool captureOrPawn = move.Execute(Board);

            if (captureOrPawn)
            {
                noCaptureOrPawnMoves = 0;
                stateHistory.Clear();
            }
            else
            {
                noCaptureOrPawnMoves++;
            }

            UpdateStateString();

            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }
        #endregion

        #region Timer Tick 
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

            CheckForGameOver();
        }
        #endregion

        #region Check for Game Over
        public bool IsGameOver()
        {
            return Result != null;
        }

        private Player CheckTimeout()
        {
            if (WhiteTime == TimeSpan.Zero || BlackTime == TimeSpan.Zero)
            {
                //Result timeoutResult = (WhiteTime == TimeSpan.Zero)
                //                        ? new Result(Player.Black, EndReason.Timeout)
                //                        : new Result(Player.White, EndReason.Timeout);

                //EndGame(timeoutResult);

                return (WhiteTime == TimeSpan.Zero) ? Player.White : Player.Black;      // trả về người thua cuộc
            }

            return Player.None;
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

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            return Board.PiecePositionsFor(player).SelectMany(pos => LegalMovesForPiece(pos));
        }

        private void CheckForGameOver()
        {
            Player timeoutPlayer = CheckTimeout();
            if (timeoutPlayer != Player.None)
            {
                Result = Result.Win(timeoutPlayer.Opponent(), EndReason.Timeout);
                return;
            }

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

        public void EndGame(Result result)
        {
            Result = result;
        }
        #endregion

        #region Update StateString
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
        #endregion
    }
}