using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();   
        // Khi 1 quân cờ được chọn (thay đổi biến selectedPos), tất cả các vị trí có thể di chuyển kèm theo move tương ứng sẽ được lưu vào Dictionary moveCache
        // moveCache lưu key là <Position> tương ứng vị trí có thể di chuyển, với value là <Move>

        private GameState gameState;
        private Position selectedPos = null;

        public MainWindow()
        {
            InitializeComponent();
            InitialBoard();

            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
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
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    pieceImages[i, j].Source = Images.GetImage(piece);
                }
            }
        }

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

        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
            // Sau mỗi lần thực thi di chuyển, vẽ lại bàn cờ tương ứng với gameState.Board
            // Mở rộng: Để dễ dàng cho người dùng theo dõi, thiết lập vị trí của CurrentPlayer nằm dưới (hiện tại quân trắng là row = 6, 7; đen là 0, 1)
            
            SetCursor(gameState.CurrentPlayer);

            // gameState.Result được tự động cập nhật khi gọi hàm MakeMove
            if (gameState.IsGameOver())     // if (gameState.Result != null)
            {
                ShowGameOverMenu();
            }
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);
            return new Position(row, column);
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

        private void SetCursor(Player player)
        {
            if (player == Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }

        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        private void ShowGameOverMenu()
        {
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
            HideHighLights();
            moveCache.Clear();

            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }
    }
}