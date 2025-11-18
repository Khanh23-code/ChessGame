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
        #region Properties
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();   
        // Khi 1 quân cờ được chọn (thay đổi biến selectedPos), tất cả các vị trí có thể di chuyển kèm theo move tương ứng sẽ được lưu vào Dictionary moveCache
        // moveCache lưu key là <Position> tương ứng vị trí có thể di chuyển, với value là <Move>

        private GameState gameState;
        private Position selectedPos = null;

        private int assetIndex = 1;
        #endregion

        #region Constructors / Initial Functions
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
        #endregion

        #region Supportive Functions
        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);
            return new Position(row, column);
        }

        private bool IsMenuOnScreen()
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
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // check BoardView loaded
            // call public function
            if (BoardViewControl != null && !BoardViewControl.IsMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPauseMenu();
                MessageBox.Show(gameState.StateString);
                return;
            }

            if (!IsMenuOnScreen() && e.Key == Key.Space)
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
                BoardViewControl.ShowPauseMenu();
            }
        }
    }
}