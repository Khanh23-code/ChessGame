using ChessLogic;

namespace ChessLogic
{
    public class GameState
    {
        #region Properties
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;

        private int noCaptureOrPawnMoves;      // Biến đếm hỗ trợ 50-move rule

        // Hien tai chi luu 1 stateString tai 1 thoi diem va dem so lan lap lai cua stateString do => phuc vu Threefold rule
        // => Mo rong: Luu tat ca stateString de truy vet nuoc di
        private string stateString;
        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();
        #endregion

        #region Constructors
        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;

            noCaptureOrPawnMoves = 0;

            stateString = new StateString(CurrentPlayer, Board).ToString();
            stateHistory[stateString] = 1;
        }
        #endregion

        #region Make Move
        public void MakeMove(Move move)        // Hàm dùng để thực thi <move>
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
        #endregion

        #region Check for Game Over
        public bool IsGameOver()
        {
            return Result != null;
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
