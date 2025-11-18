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

namespace ChessUI.Views.BoardMenu
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();
        // Khi 1 quân cờ được chọn (thay đổi biến selectedPos), tất cả các vị trí có thể di chuyển kèm theo move tương ứng sẽ được lưu vào Dictionary moveCache
        // moveCache lưu key là <Position> tương ứng vị trí có thể di chuyển, với value là <Move>

        private GameState gameState;
        private Position selectedPos = null;

        public int assetIndex = 1;   // Mặc định sử dụng Asset 1

        // DispatcherTimer
        private readonly DispatcherTimer timer;

        public BoardView()
        {
            InitializeComponent();
            InitialBoard();

            TimeSpan initialTime = TimeSpan.FromMinutes(10);
            gameState = new GameState(Player.White, Board.Initial(), initialTime);

            DrawBoard(gameState.Board);
            //SetCursor(gameState.CurrentPlayer);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            StartGame();
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

        private void DrawBoard(Board board)
        {
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
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
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighLights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
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
        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
            // Sau mỗi lần thực thi di chuyển, vẽ lại bàn cờ tương ứng với gameState.Board

            //SetCursor(gameState.CurrentPlayer);

            // gameState.Result được tự động cập nhật khi gọi hàm MakeMove
            if (gameState.IsGameOver())     // if (gameState.Result != null)
            {
                ShowGameOverMenu();
            }
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
                    Application.Current.Shutdown();
                }
            };
        }

        private void RestartGame()
        {
            selectedPos = null;
            HideHighLights();
            moveCache.Clear();

            // initial Timer after ResetGame is called
            TimeSpan initialTime = TimeSpan.FromMinutes(10);
            gameState = new GameState(Player.White, Board.Initial(), initialTime);

            DrawBoard(gameState.Board);
            StartGame();
        }

        private void HandlePromotionMove(Move move)
        {
            // B1: hiển thị quân tốt đã di chuyển cho người dùng (nhưng thực sự chưa thực thi PromotionMove)
            Position from = move.FromPos;
            Position to = move.ToPos;

            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

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
            UpdateTimerDisplay();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            gameState.Tick();
            UpdateTimerDisplay();
            if (gameState.IsGameOver())
            {
                ShowGameOverMenu();
                timer.Stop();
            }
        }
        private void UpdateTimerDisplay()
        {
            PlayerTimerText.Text = gameState.WhiteTime.ToString(@"mm\:ss");
            OpponentTimerText.Text = gameState.BlackTime.ToString(@"mm\:ss");
        }
        #endregion

        public void ChangeAsset()
        {
            if (assetIndex == 1)
            {
                assetIndex = 2;
            }
            else
            {
                assetIndex = 1;
            }
            DrawBoard(gameState.Board);
        }
    }
}
