using ChessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;
using ChessUI.Views.GameMenus;
namespace ChessUI.Views.BoardMenu
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        public event Action OnReturnToMenu;
        // Fen strings for Endgame Lessons
        private string _currentScenarioFen = null;
        // Cache FEN for Puzzle Mode Restart
        private string _cachedPuzzleFen;

        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();
        // Khi 1 quân cờ được chọn (thay đổi biến selectedPos), tất cả các vị trí có thể di chuyển kèm theo move tương ứng sẽ được lưu vào Dictionary moveCache
        // moveCache lưu key là <Position> tương ứng vị trí có thể di chuyển, với value là <Move>

        // AudioManager
        private readonly AudioManager _audioManager = new AudioManager();
        private readonly ChessAIService _aiService = new ChessAIService(); // Service AI chúng ta đã tạo
        public bool IsVsComputer { get; set; } = false; // Biến cờ để biết đang chơi với Người hay Máy

        private GameState gameState;
        private Position selectedPos = null;

        public InfoView _infoView;

        public int assetIndex = 1;   // Mặc định sử dụng Asset 1

        private Player clientSide = Player.White;
        // DispatcherTimer
        private DispatcherTimer timer;

        // check isLimits 
        private bool isUnlimitedTime = false;

        private bool loadFromFEN = false;   

        private CloudService _cloudService = new CloudService();
        public string userID = "Player_Default_00";

        public TimeSpan TimeLimit { get; set; }

        public event Action<int> OnPuzzleSolved;
        private List<string> _currentPuzzleSolution;

        private int _currentPuzzleMoveIndex;        
        private bool _isPuzzleMode = false;
        private bool _isDailyMode = false;


        // Properties
        public bool IsPuzzleMode => _isPuzzleMode;
        public bool IsEndgameMode => !string.IsNullOrEmpty(_currentScenarioFen);
        public BoardView()
        {
            InitializeComponent();
            InitialBoard();

            LoadInitialGameState();

            DrawBoard(gameState.Board);
            //SetCursor(gameState.CurrentPlayer);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            BoardGrid.IsHitTestVisible = false;
            UpdateTimerDisplay();
        }

        private void InitialBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = new Image();
                    pieceImages[i, j] = image;
                    PieceGrid.Children.Add(image);

                    Rectangle highlight = new Rectangle();
                    highlights[i, j] = highlight;
                    HighLightGrid.Children.Add(highlight);
                }
            }
        }

        private void LoadInitialGameState()
        {
            TimeSpan initialTime = TimeSpan.FromMinutes(10);
            gameState = new GameState(Player.White, Board.Initial(), initialTime, userID);
        }

        private void DrawBoard(Board board)
        {
            ImageBrush boardBackGround = BoardGrid.Background as ImageBrush;

                BitmapImage source = new BitmapImage();
                source.BeginInit();
                //source.UriSource = new Uri($"/Assets/Asset{assetIndex}/Board.png", UriKind.Relative);
                string packUri = $"pack://application:,,,/Assets/Asset{assetIndex}/Board.png";
                source.UriSource = new Uri(packUri, UriKind.Absolute);
                source.EndInit();

                //ImageSource source = new BitmapImage(new Uri($"Assets/Asset{assetIndex}/Board.png", UriKind.Relative));
                //MessageBox.Show(source.ToString() + "+" + path);
                boardBackGround.ImageSource = source;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int r = i;
                    int c = j;

                    if (clientSide == Player.Black)
                    {
                        r = 7 - i;
                        c = 7 - j;
                    }
                    Piece piece = board[r, c];
                    pieceImages[i, j].Source = Images.GetImage(piece, assetIndex);
                }
            }
        }

        //private void DrawBoard(Board board)
        //{
        //    for (int i = 0; i < 8; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            Piece piece = board[i, j];
        //            pieceImages[i, j].Source = Images.GetImage(piece);
        //        }
        //    }
        //}
        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);

            if (row < 0) row = 0;
            if (row > 7) row = 7;     
            if (column < 0) column = 0;
            if (column > 7) column = 7;

            if (clientSide == Player.Black)
            {
                return new Position (7-row, 7-column);
            }
            return new Position(row, column);
        }

        public bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        // moves truyền vào được lấy từ piece.GetMoves của gameState.Board[selectedPos]
        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();

            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighLights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);

            foreach (Position to in moveCache.Keys)
            {
                int r = to.Row;
                int c = to.Column;

                if (clientSide == Player.Black)
                {
                    r = 7 - to.Row;
                    c = 7 - to.Column;
                }
                highlights[r, c].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighLights()
        {
            foreach (Position to in moveCache.Keys)
            {
                int r = to.Row;
                int c = to.Column;

                if (clientSide == Player.Black)
                {
                    r = 7 - to.Row;
                    c = 7 - to.Column;
                }
                highlights[r, c].Fill = Brushes.Transparent;
            }
        }

        //private void SetCursor(Player player)
        //{
        //    if (player == Player.White)
        //    {
        //        Cursor = ChessCursors.WhiteCursor;
        //    }
        //    else
        //    {
        //        Cursor = ChessCursors.BlackCursor;
        //    }
        //}
        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen()) return;       // Nếu trên màn hình đang hiện 1 Menu nào đó (GameOverMenu) thì không nhận event MouseDown

            Point point = e.GetPosition(BoardGrid);     // Hàm này trả về point tính theo px, cần sử dụng ToSquarePosition để định Position
            Position pos = ToSquarePosition(point);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);    // Thao tác chọn quân để di chuyển
            }
            else
            {
                OnToPositionSelected(pos);      // Thao tác chọn ô để di chuyển quân đã chọn
            }

            // Hiện tại khi đã có selectedPos => luôn gọi OnToPositionSelected()
            // => Nếu người dùng đã chọn 1 quân cờ, sau đó muốn chọn quân cờ khác, buộc phải bấm ô không thể di chuyển để cancel, rồi mới chọn lại
            // Mở rộng: Nếu selected != null nhưng người dùng vẫn nhấp vào quân cờ khác của mình, lập tức chuyển selectedPos và cập nhật moveCache
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighLights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighLights();

            if (moveCache.TryGetValue(pos, out Move move))      // Nếu moveCache rỗng thì không chạy
            {
                if (move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotionMove(move);
                }
                else
                {
                    HandleMove(move);
                }
            }
        }
        private async void HandleMove(Move move)
        {
            if (_isPuzzleMode && _currentPuzzleSolution != null && gameState.CurrentPlayer == clientSide && _currentPuzzleMoveIndex < _currentPuzzleSolution.Count)
            {
                    string userMoveUci = GetUciString(move);
                    string expectedMove = _currentPuzzleSolution[_currentPuzzleMoveIndex];

                    if (userMoveUci != expectedMove)
                    {
                        _audioManager.PlayPuzzleWrong();

                        CustomMessageBox.Show("Nước đi sai! Hãy thử lại.", "Puzzle Failed");
                        selectedPos = null;
                        HideHighLights();
                        DrawBoard(gameState.Board);
                        return; 
                    }
                    _currentPuzzleMoveIndex++;
                
            }

            bool isCapture = gameState.Board[move.ToPos] != null || move.Type == MoveType.EnPassant;
            bool isCastle = move.Type == MoveType.CaslteKS || move.Type == MoveType.CaslteQS;
            bool isPromotion = move.Type == MoveType.PawnPromotion;

            gameState.MakeMove(move);

            string movementData = gameState.GetMovementData(gameState.CurrentPlayer.Opponent());
            if (_infoView != null) _infoView.ShowData(movementData, gameState.CurrentPlayer.Opponent());
            DrawBoard(gameState.Board);

            if (gameState.IsGameOver())
            {
                if (_isPuzzleMode)
                {
                    timer.Stop();
                    CustomMessageBox.Show("Puzzle Completed!", "Congratulations");
                    _audioManager.PlayPuzzleCorrect();
                    return;
                }
                else
                {
                    _audioManager.PlayGameEnd();       
                }
                ShowGameOverMenu();
                return;
            }


            if (isPromotion) _audioManager.PlayPromote();
            else if (isCastle) _audioManager.PlayCastle();
            else if (isCapture) _audioManager.PlayCapture();
            else _audioManager.PlayMove();

            if (_isPuzzleMode && !_isDailyMode && gameState.CurrentPlayer != clientSide)
            {
                if (_currentPuzzleMoveIndex < _currentPuzzleSolution.Count)
                {
                    string opponentUci = _currentPuzzleSolution[_currentPuzzleMoveIndex];

                    await Task.Delay(500);

                    Move opponentMove = FindMoveFromUci(opponentUci);
                    if (opponentMove != null)
                    {
                        _currentPuzzleMoveIndex++;
                        HandleMove(opponentMove); 
                    }
                }
                else
                {
                    _audioManager.PlayPuzzleCorrect();
                    OnPuzzleSolved?.Invoke(10);
                }
            }
            else if (IsVsComputer && gameState.CurrentPlayer != clientSide)
            {
                PlayAiTurn(); 
            }
        }
        private Move FindMoveFromUci(string uci)
        {
            try
            {
                string fromStr = uci.Substring(0, 2); 
                string toStr = uci.Substring(2, 2); 

                int fromCol = fromStr[0] - 'a';
                int fromRow = 8 - (fromStr[1] - '0');

                int toCol = toStr[0] - 'a';
                int toRow = 8 - (toStr[1] - '0');

                Position fromPos = new Position(fromRow, fromCol);
                Position toPos = new Position(toRow, toCol);

                var legalMoves = gameState.LegalMovesForPiece(fromPos);

                foreach (var move in legalMoves)
                {
                    if (move.ToPos.Row == toPos.Row && move.ToPos.Column == toPos.Column)
                    {
                        if (uci.Length == 5 && move.Type == MoveType.PawnPromotion && move is PawnPromotion promoMove)
                        {
                            char promoChar = uci[4];
                            PieceType targetType = PieceType.Queen; 

                            if (promoChar == 'r') targetType = PieceType.Rook;
                            else if (promoChar == 'b') targetType = PieceType.Bishop;
                            else if (promoChar == 'n') targetType = PieceType.Knight;
                            if (promoMove.newType == targetType)
                            {
                                return move;
                            }
                            continue; 
                        }
                        return move;
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi parse string nếu có
            }
            return null;
        }

        // --- HÀM XỬ LÝ AI ---
        private async void PlayAiTurn()
        {
            // 1. Kiểm tra game đã kết thúc chưa
            if (gameState.IsGameOver()) return;

            // 2. Khóa bàn cờ để người chơi không click lung tung khi máy đang nghĩ
            BoardGrid.IsHitTestVisible = false;
            Cursor = Cursors.Wait; // Đổi con trỏ chuột thành hình đồng hồ cát

            // 3. Gọi AI tính toán (chạy bất đồng bộ)
            // Lưu ý: Đảm bảo class ChessAIService đã được cập nhật như hướng dẫn trước
            Move bestMove = await _aiService.GetBestMoveAsync(gameState);

            // 4. Mở khóa bàn cờ
            BoardGrid.IsHitTestVisible = true;
            Cursor = Cursors.Arrow;

            // 5. Thực hiện nước đi của máy
            if (bestMove != null)
            {
                HandleMove(bestMove);
            }
        }

        public async void StartVsComputerGame(int aiDepth, Player playerSide)
        {
            _currentScenarioFen = null;
            timer.Stop();                  
            MenuContainer.Content = null;

            _isPuzzleMode = false;
            _currentPuzzleSolution = null;
            this.clientSide = playerSide;
            this.IsVsComputer = true;

            selectedPos = null;         
            moveCache.Clear();            
            HideHighLights(); 

            BoardGrid.IsHitTestVisible = true; 
            Cursor = Cursors.Arrow;

            // Set images for player and bot
            string botAvatar = GetBotAvatarPath(aiDepth);
            string playerAvatar = "/Assets/MenuAssets/player1.png"; 
            SetAvatarImages(playerAvatar, botAvatar);

            _aiService.SetDifficulty(aiDepth);
            // set up who first
            Player aiSide = (playerSide == Player.White) ? Player.Black : Player.White;
            _aiService.SetAIPlayer(aiSide);

            isUnlimitedTime = true;
            TimeSpan initialTime = TimeSpan.FromMinutes(10);

            string modeKey = "PvE";
            string fenToLoad = await CheckForSavedGame(modeKey);
            if (fenToLoad != null)
            {
                // Load saved game from cloud
                gameState = new GameState(fenToLoad, userID, modeKey);
            }
            else
            {
                // Load new game
                gameState = new GameState(Player.White, Board.Initial(), initialTime, userID, modeKey);
            }

            if (playerSide == Player.White)
            {
                PlayerNameText.Text = "You (White)";
                OpponentNameText.Text = "Computer (Black)";
            }
            else
            {
                PlayerNameText.Text = "You (Black)";
                OpponentNameText.Text = "Computer (White)";
            }

            DrawBoard(gameState.Board);
            await RunCountdown();
            StartGame();


            if (playerSide == Player.Black)
            {
                PlayAiTurn();
            }
        }
        public async void StartPvPGame(TwoPlayerSettings settings)
        {
            _currentScenarioFen = null;
            timer.Stop();
            MenuContainer.Content = null;
            selectedPos = null;
            moveCache.Clear();
            HideHighLights();

            BoardGrid.IsHitTestVisible = true;
            Cursor = Cursors.Arrow;

            // Set default avatar images for both players
            SetAvatarImages("/Assets/MenuAssets/player1.png", "/Assets/MenuAssets/player2.png");

            _isPuzzleMode = false;
            _currentPuzzleSolution = null;
            this.IsVsComputer = false;
            this.clientSide = Player.White;

            PlayerNameText.Text = string.IsNullOrEmpty(settings.WhiteName) ? "Player 1" : settings.WhiteName;
            OpponentNameText.Text = string.IsNullOrEmpty(settings.BlackName) ? "Player 2" : settings.BlackName;

            string modeKey = "PvP";
            string fenToLoad = await CheckForSavedGame(modeKey);

            if (settings.TimeLimit == TimeSpan.Zero)
            { 
                isUnlimitedTime = true;

                if (fenToLoad != null)
                {
                    // Load saved game from cloud
                    gameState = new GameState(fenToLoad, userID, modeKey);
                }
                else
                {
                    // Load new game
                    gameState = new GameState(Player.White, Board.Initial(), TimeSpan.FromDays(1), userID, modeKey);
                    gameState.ClearCloudSave();
                }
            }
            else
            {
                isUnlimitedTime = false;

                if (fenToLoad != null)
                {
                    // Load saved game from cloud
                    gameState = new GameState(fenToLoad, userID, modeKey);
                }
                else
                {
                    // Load new game
                    gameState = new GameState(Player.White, Board.Initial(), settings.TimeLimit, userID, modeKey);
                    gameState.ClearCloudSave();
                }
            }
            

            DrawBoard(gameState.Board);
            await RunCountdown();
            StartGame();
        }

        // transform Move to UCI string
        private string GetUciString(Move move)
        {
            string fromCol = ((char)('a' + move.FromPos.Column)).ToString();
            string fromRow = (8 - move.FromPos.Row).ToString();

            string toCol = ((char)('a' + move.ToPos.Column)).ToString();
            string toRow = (8 - move.ToPos.Row).ToString();

            string promotion = "";
            if (move.Type == MoveType.PawnPromotion && move is PawnPromotion promoMove)
            {
                switch (promoMove.newType)
                {
                    case PieceType.Rook: promotion = "r"; break;
                    case PieceType.Bishop: promotion = "b"; break;
                    case PieceType.Knight: promotion = "n"; break;
                    default: promotion = "q"; break;
                }
            }

            return $"{fromCol}{fromRow}{toCol}{toRow}{promotion}";
        }
        public void StartPuzzle(string fen, List<string> solution, bool isDaily = false)
        {
            timer.Stop();
            MenuContainer.Content = null;
            selectedPos = null;
            moveCache.Clear();
            HideHighLights();

            _isPuzzleMode = true;
            _isDailyMode = isDaily; 
            _currentPuzzleSolution = solution;
            _cachedPuzzleFen = fen;
            _currentPuzzleMoveIndex = 0;

            this.IsVsComputer = false;

            gameState = new GameState(fen, userID);
            this.clientSide = gameState.CurrentPlayer;

            PlayerNameText.Text = "You (Solver)";
            OpponentNameText.Text = "Puzzle Challenge";

            DrawBoard(gameState.Board);
            BoardGrid.IsHitTestVisible = true;
            Cursor = Cursors.Arrow;

            UpdateTimerDisplay();
            timer.Start();
        }
        // --- Thêm đoạn này vào BoardView.xaml.cs ---

        public async void ShowHint()
        {
            if (gameState == null || gameState.IsGameOver()) return;

            Move hintMove = null;
            if (_isPuzzleMode && _currentPuzzleSolution != null && _currentPuzzleMoveIndex < _currentPuzzleSolution.Count)
            {
                try
                {
                    string expectedUci = _currentPuzzleSolution[_currentPuzzleMoveIndex];
                    hintMove = FindMoveFromUci(expectedUci);
                }
                catch { }
            }
            else if (!IsVsComputer || (IsVsComputer && gameState.CurrentPlayer == clientSide))
            {
                Cursor = Cursors.Wait;
                try
                {
                    _aiService.SetDifficulty(3); 
                    _aiService.SetAIPlayer(gameState.CurrentPlayer);

                    hintMove = await _aiService.GetBestMoveAsync(gameState);
                }
                finally
                {
                    if (IsVsComputer)
                    {
                        Player aiSide = (clientSide == Player.White) ? Player.Black : Player.White;
                        _aiService.SetAIPlayer(aiSide);
                    }
                    Cursor = Cursors.Arrow;
                }
            }
            if (hintMove != null)
            {
                await FlashHint(hintMove);
            }
            else
            {
                MessageBox.Show("Không tìm thấy gợi ý khả thi!");
            }
        }
        private async Task FlashHint(Move move)
        {
            HideHighLights();

            Position from = move.FromPos;
            Position to = move.ToPos;

            int rFrom = from.Row;
            int cFrom = from.Column;
            int rTo = to.Row;
            int cTo = to.Column;

            if (clientSide == Player.Black)
            {
                rFrom = 7 - rFrom;
                cFrom = 7 - cFrom;
                rTo = 7 - rTo;
                cTo = 7 - cTo;
            }
            // Change Highlight color to hint color
            var hintBrush = new SolidColorBrush(Color.FromArgb(180, 255, 215, 0));

            if (highlights[rFrom, cFrom] != null) highlights[rFrom, cFrom].Fill = hintBrush;
            if (highlights[rTo, cTo] != null) highlights[rTo, cTo].Fill = hintBrush;

            await Task.Delay(1500); // Chờ 1.5 giây
            if (highlights[rFrom, cFrom] != null) highlights[rFrom, cFrom].Fill = Brushes.Transparent;
            if (highlights[rTo, cTo] != null) highlights[rTo, cTo].Fill = Brushes.Transparent;
        }
        public void ShowGameOverMenu()
        {
            timer.Stop();
            GameOverMenu gameOverMenu = new GameOverMenu(gameState);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                else
                {
                    gameState.ClearCloudSave();
                    gameOverMenu.Content = null;
                    OnReturnToMenu?.Invoke();
                }
            };
        }

        public void RestartGame()
        {
            selectedPos = null;
            HideHighLights();
            moveCache.Clear();

            BoardGrid.IsHitTestVisible = true;
            Cursor = Cursors.Arrow;
            if (_isPuzzleMode)
            {
                StartPuzzle(_cachedPuzzleFen, _currentPuzzleSolution, _isDailyMode);
            }
            else
            {
                // Nếu là Game thường (PvP hoặc PvE) -> Logic cũ
                TimeSpan initialTime;

                if (isUnlimitedTime)
                {
                    initialTime = TimeSpan.FromDays(1);
                }
                else
                {
                    initialTime = TimeLimit;
                }

                gameState.ClearCloudSave();
                if (!string.IsNullOrEmpty(_currentScenarioFen))
                {
                    gameState = new GameState(_currentScenarioFen, userID);
                }
                else
                {
                    gameState = new GameState(Player.White, Board.Initial(), initialTime, userID);
                }

                DrawBoard(gameState.Board);
                StartGame();
                if (IsVsComputer && clientSide == Player.Black)
                {
                    PlayAiTurn();
                }
            }
        }

        private void HandlePromotionMove(Move move)
        {
            Position from = move.FromPos;
            Position to = move.ToPos;

            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

            int rTo = move.ToPos.Row;
            int cTo = move.ToPos.Column;

            if (clientSide == Player.Black)
            {
                rTo = 7 - rTo;
                cTo = 7 - cTo;
            }

            // B2: Show PromotionMenu => Nhận event click để xem người dùng chọn quân nào
            PromotionMenu promMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promMenu;

            promMenu.PieceSelected += type =>
            {
                // B3: Thực thi PromotionMove
                MenuContainer.Content = null;
                HandleMove(new PawnPromotion(from, to, type));

                //switch (type)
                //{
                //    case PieceType.Queen:
                //    case PieceType.Rook:
                //    case PieceType.Knight:
                //    case PieceType.Bishop:
                //    default:
                //} 
            };
        }
        public void ShowPauseMenu()
        {
            timer.Stop();
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu;

            //MessageBox.Show(gameState.FENString);
            CustomMessageBox.Show("Current FEN", gameState.FENString);

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null;

                if (option == Option.Restart)
                {
                    RestartGame();
                }
                else
                {
                    if (!gameState.IsGameOver())
                    {
                        timer.Start();
                    }
                }
            };
        }

        #region TimerSetUp
        private void StartGame()
        {
            _audioManager.PlayGameStart();
            UpdateTimerDisplay();
            if (_infoView != null)
            {
                _infoView.ClearData();
            }
            PlayerTimerBorder.Visibility = Visibility.Visible;
            OpponentTimerBorder.Visibility = Visibility.Visible;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            gameState.Tick();
            if (gameState.CurrentPlayer == clientSide)
            {
                PlayerTimerBorder.Background = Brushes.White;
                PlayerTimerText.Foreground = Brushes.Black;

                OpponentTimerBorder.Background = null;                           
                OpponentTimerText.Foreground = Brushes.White;
            }
            else
            {
                PlayerTimerBorder.Background = null;
                PlayerTimerText.Foreground = Brushes.White;

                OpponentTimerBorder.Background = Brushes.White;
                OpponentTimerText.Foreground = Brushes.Black;
            }

            UpdateTimerDisplay();
            if (gameState.IsGameOver())
            {
                ShowGameOverMenu();
                timer.Stop();
            }
        }
        private void UpdateTimerDisplay()
        {
            if (isUnlimitedTime == true)
            {
                PlayerTimerText.Text = "∞";
                OpponentTimerText.Text = "∞";
                return;
            }
            if (clientSide == Player.White)
            {
                PlayerTimerText.Text = gameState.WhiteTime.ToString(@"mm\:ss");
                OpponentTimerText.Text = gameState.BlackTime.ToString(@"mm\:ss");
                return;
            }
            else
            {
                PlayerTimerText.Text = gameState.BlackTime.ToString(@"mm\:ss");
                OpponentTimerText.Text = gameState.WhiteTime.ToString(@"mm\:ss");
            }
        }
        #endregion

        //public void ChangeAsset()
        //{
        //    if (assetIndex == 1)
        //    {
        //        assetIndex = 2;
        //    }
        //    else
        //    {
        //        assetIndex = 1;
        //    }
        //    DrawBoard(gameState.Board);
        //}
        //function to run countdown animation
        private async Task RunCountdown()
        {
            CountDownView countDown = new CountDownView();
            MenuContainer.Content = countDown;
            BoardGrid.IsHitTestVisible = false;
            await countDown.StartCountDownAsync();
            MenuContainer.Content = null;
            BoardGrid.IsHitTestVisible = true;
        }

        public void StartEndgameLesson(string fenString)
        {
            timer.Stop();
            MenuContainer.Content = null;
            selectedPos = null;
            moveCache.Clear();
            HideHighLights();

            _isPuzzleMode = false;
            _currentPuzzleSolution = null;

            _currentScenarioFen = fenString;
            this.IsVsComputer = true; 
            this.clientSide = Player.White; 

            _aiService.SetDifficulty(3); 
            _aiService.SetAIPlayer(Player.Black);

            gameState = new GameState(fenString, userID);

            PlayerNameText.Text = "You (Training)";
            OpponentNameText.Text = "Computer (Defender)";

            DrawBoard(gameState.Board);

            BoardGrid.IsHitTestVisible = true;
            Cursor = Cursors.Arrow;

            // Start audio 
            _audioManager.PlayGameStart();
            UpdateTimerDisplay();
            PlayerTimerBorder.Visibility = Visibility.Visible;
            OpponentTimerBorder.Visibility = Visibility.Visible;
            timer.Start();
        }
     
        public void UpdateTheme(int AssetId)
        {
            this.assetIndex = AssetId;
            if (gameState != null)
            {
                DrawBoard(gameState.Board);
            }
        }

        private async Task<string> CheckForSavedGame(string mode)
        {
            try
            {
                if (IsVsComputer)
                {
                    mode = "PvE";
                }
                else
                {
                    mode = "PvP";
                }
                string savedFen = await _cloudService.LoadGameAsync(userID, mode);

                if (!string.IsNullOrEmpty(savedFen))
                {
                    bool result = CustomMessageBox.Show(
                        $"A saved game in {mode} was found in the cloud. Do you want to load it?",
                        "Load Saved Game",
                        true); 

                    if (result) 
                    {
                        return savedFen;
                    }
                }
            }
            catch (Exception ex)
            {
                // Báo lỗi ở debug output phía coder, không hiện MessageBox làm phiền với user, user vẫn có thể chơi bình thường
                System.Diagnostics.Debug.WriteLine("Lỗi load game: " + ex.Message);
            }

            return null;
        }
        #region Set Avatar Images
        private void SetAvatarImages(string playerPath, string opponentPath)
        {
                var playerUri = new Uri($"pack://application:,,,{playerPath}", UriKind.Absolute);
                PlayerAvatar.ImageSource = new BitmapImage(playerUri);

                var opponentUri = new Uri($"pack://application:,,,{opponentPath}", UriKind.Absolute);
                OpponentAvatar.ImageSource = new BitmapImage(opponentUri);
        }
        private string GetBotAvatarPath(int depth)
        {
            switch (depth)
            {
                case 1: return "/Assets/MenuAssets/admin_avartar.png"; 
                case 2: return "/Assets/MenuAssets/LeQuangLiem_avartar.jpeg"; 
                case 3: return "/Assets/MenuAssets/Hikaru_Nakamura_avartar.jpeg"; 
                case 4: return "/Assets/MenuAssets/Garry_Kasparov_avartar.jpeg";
                case 5: return "/Assets/MenuAssets/Magnus_Carlsen_avartar.jpeg"; 
                default: return "/Assets/MenuAssets/admin_avartar.png";
            }
        }
        #endregion
        #region Resign and Draw Offer
        public void HandleResign()
        {
            if (gameState == null || gameState.IsGameOver()) return;
            bool result = CustomMessageBox.Show("Are you sure you want to resign?", "Confirm", true);

            if (result) 
            {
                timer.Stop();
                Player winner = clientSide.Opponent();
                Result resignResult = Result.Win(winner, EndReason.Resignation);
                gameState.EndGame(resignResult);
                gameState.ClearCloudSave();

                ShowGameOverMenu();
            }
        }
        public void HandleDrawOffer()
        {
            if (gameState == null || gameState.IsGameOver()) return;
            if (IsVsComputer) return;

            Player offerPlayer = gameState.CurrentPlayer;
            Player opponent = offerPlayer.Opponent();
            bool result = CustomMessageBox.Show(
                $"{offerPlayer} offers a draw.\n{opponent}, do you accept?",
                "Draw Offer",
                true); 

            if (result)
            {
                timer.Stop();
                Result drawResult = Result.Draw(EndReason.DrawAgreement);
                gameState.EndGame(drawResult);
                gameState.ClearCloudSave();

                ShowGameOverMenu();
            }
        }
        #endregion
    }
}
