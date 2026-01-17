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

        public int FullMoveNumber = 0;
        public int NoCaptureOrPawnMoves;
        private string stateString;
        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();

        public string FENString;

        public MovementInfo MovementInfo { get; private set; }

        private readonly CloudService _cloudService;
        private readonly string _mode;
        // Mode: "PvP", "PvE", "Online"

        // Tạm thời dùng ID mặc định để lưu trữ đám mây
        // Sau này: mỗi account người dùng sẽ có ID riêng
        private const string UserID = "Player_Default";
        #endregion

        #region Constructors
        public GameState(Player player, Board board, TimeSpan initialTime, string mode = "")
        {
            CurrentPlayer = player;
            Board = board;
            WhiteTime = initialTime;
            BlackTime = initialTime;

            NoCaptureOrPawnMoves = 0;

            stateString = new StateString(CurrentPlayer, Board).ToString();
            stateHistory[stateString] = 1;

            MovementInfo = new MovementInfo();

            _cloudService = new CloudService();
            _mode = mode;
        }
        // New GameState from existing GameState (for Undo/Redo or FEN)
        // ??? Có cần lưu thêm stateHistory ???
        public GameState(string fen, string mode = "")
        {
            string[] parts = fen.Split(' ');

            CurrentPlayer = (parts[1] == "w") ? Player.White : Player.Black;
            NoCaptureOrPawnMoves = int.Parse(parts[4]);
            FullMoveNumber = int.Parse(parts[5]);
            Board = new Board(parts[0], parts[2], parts[3], CurrentPlayer);

            string[] timers = parts[6].Split(':');
            WhiteTime = TimeSpan.FromSeconds(int.Parse(timers[0]));
            BlackTime = TimeSpan.FromSeconds(int.Parse(timers[1]));

            FENString = fen;
            MovementInfo = new MovementInfo();

            _cloudService = new CloudService();
            _mode = mode;
        }
        #endregion

        #region Make Move
        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPostion(CurrentPlayer, null);

            UpdateMovementInfo(move, Board.Copy());

            bool captureOrPawn = move.Execute(Board);

            if (CurrentPlayer == Player.Black)
            {
                FullMoveNumber++;
            }

            if (captureOrPawn)
            {
                NoCaptureOrPawnMoves = 0;
                stateHistory.Clear();
            }
            else
            {
                NoCaptureOrPawnMoves++;
            }

            UpdateStateString();

            CurrentPlayer = CurrentPlayer.Opponent();

            UpdateFENString();

            // Khởi tạo dịch vụ đám mây để save game
            // Gọi hàm lưu cloud. Cú pháp _ báo C# không cần chờ kết quả trả về
            if (_mode != "")
            {
                _ = _cloudService.SaveGameAsync(UserID, _mode, FENString);
            }

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
            else if (NoCaptureOrPawnMoves >= 100)
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
            else if (stateString != null && stateHistory[stateString] >= 3)
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

        #region FEN String
        private void UpdateFENString()
        {
            FENString = new FENString(this).ToString();
        }
        #endregion

        #region Movement Info
        private void UpdateMovementInfo(Move move, Board board)
        {
            string data = MovementInfo.SetData(move, board, CurrentPlayer);
        }

        public string GetMovementData(Player player)
        {
            if (player == Player.White)
            {
                return MovementInfo.whiteMoves.Last().ToString();
            }
            else
            {
                return MovementInfo.blackMoves.Last().ToString();
            }
        }

        public bool IsCheckMateMove(Player opponent, Board cpyBoard)
        {
            if (!cpyBoard.PiecePositionsFor(opponent).SelectMany(pos => LegalMovesForPiece(pos)).Any())
            {
                if (cpyBoard.IsInCheck(opponent))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        public void ClearCloudSave()
        {
            if (_mode == "")
            {
                return;
            }
            _ = _cloudService.DeleteGameAsync(UserID, _mode);
        }
    }
}