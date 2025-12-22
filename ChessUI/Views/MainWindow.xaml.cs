using ChessLogic;
using ChessUI.Views.BoardMenu;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AIController aiController; 
        private readonly Views.BoardMenu.InfoView DefaultInfoView = new Views.BoardMenu.InfoView();
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            aiController = new AIController(Player.Black, AIController.Difficulty.Medium); 
            RightPanelContentHost.Content = DefaultInfoView; 

            if (NavigationViewControl != null)
            {
                NavigationViewControl.Loaded += NavigationView_Loaded;
            }

            BoardViewControl._infoView = DefaultInfoView;
        }
        #region Taskbar Button
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
        }
        // Change content of btnMaximize
        private void Window_StateChanged(object sender, EventArgs e)
        {
            // check if Window in Maximized state change btn Maximize content
            if (this.WindowState == WindowState.Maximized)
            {
                btnMaximize.Content = "\uE923";
            }
            // otherwise
            else
            {
                btnMaximize.Content = "\uE922";
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // check BoardView loaded
            // call public function
            if (BoardViewControl != null && !BoardViewControl.IsMenuOnScreen() && e.Key == Key.Escape)
            {
                //ShowPauseMenu();
                //return;

                BoardViewControl.ShowPauseMenu();
                return;
            }

            //if (BoardViewControl != null && !BoardViewControl.IsMenuOnScreen() && e.Key == Key.Space)
            //{
            //    BoardViewControl.ChangeAsset();
            //}
        }

        #region Navigation Menu
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView navView = (NavigationView)sender;
            navView.SelectionChanged += OnMainNavigationChanged;
        }

        private void NavigateToPage(MenuPage page)
        {
            OverlayContainer.Content = null;
            OverlayContainer.Visibility = Visibility.Collapsed;
            GameContainer.Visibility = Visibility.Visible;

            switch (page)
            {
                case MenuPage.Play:
                    break;

                case MenuPage.Learn:
                    OpenOverlay(new Views.BoardMenu.LearnFlyoutMenu());
                    break;

                case MenuPage.Settings: 
                    break;

                case MenuPage.More:
                    break;
            }
        }
        #endregion
        private void OpenOverlay(UserControl content)
        {
            GameContainer.Visibility = Visibility.Collapsed;
            OverlayContainer.Content = content;
            OverlayContainer.Visibility = Visibility.Visible;

            if (content is LearnFlyoutMenu learnMenu)
            {
                learnMenu.LessonSelected -= LearnMenu_OnLessonSelected;
                learnMenu.LessonSelected += LearnMenu_OnLessonSelected;
            }
        }
        private void LearnMenu_OnLessonSelected(object sender, int lessonId)
        {
            switch (lessonId)
            {
                case 1:
                    OpenOverlay(new Views.BoardMenu.ChessMovesLesson());
                    break;

                case 2:
                    OpenOverlay(new Views.BoardMenu.ChessOpeningsLesson());
                    break;

                case 3:
                    OverlayContainer.Visibility = Visibility.Collapsed;
                    OverlayContainer.Content = null;
                    GameContainer.Visibility = Visibility.Visible;

                    var endgameView = new Views.BoardMenu.EndgameLesson();
                    endgameView.OptionSelected += Endgame_OptionSelected;

                    RightPanelContentHost.Content = endgameView;
                    break;

                default:
                    break;
            }
        }
        private void OnMainNavigationChanged(object sender, ChessUI.Views.BoardMenu.NavigationEventArgs e)
        {
            NavigateToPage(e.Page);
            NavigationView navView = (NavigationView)sender;

            if (e.Page == MenuPage.Play)
            {
                Popup playSubMenuPopup = (Popup)navView.FindName("PlaySubMenu");
                if (playSubMenuPopup != null && playSubMenuPopup.Child is PlayFlyoutMenu playFlyoutMenu)
                {
                    playFlyoutMenu.PlayComputerClicked -= PlayFlyoutMenu_PlayComputerClicked;
                    playFlyoutMenu.PlayComputerClicked += PlayFlyoutMenu_PlayComputerClicked;

                    playFlyoutMenu.PlayTwoPlayerClicked -= PlayFlyoutMenu_PlayTwoPlayerClicked;
                    playFlyoutMenu.PlayTwoPlayerClicked += PlayFlyoutMenu_PlayTwoPlayerClicked;
                }
            }
            else if (e.Page == MenuPage.Puzzle)
            {
                Popup puzzlePopup = (Popup)navView.FindName("PuzzleSubMenu");

                if (puzzlePopup != null && puzzlePopup.Child is PuzzleFlyoutMenu puzzleMenu)
                {
                    puzzleMenu.NormalPuzzleClicked -= PuzzleMenu_NormalClicked;
                    puzzleMenu.DailyPuzzleClicked -= PuzzleMenu_DailyClicked;

                    puzzleMenu.NormalPuzzleClicked += PuzzleMenu_NormalClicked;
                    puzzleMenu.DailyPuzzleClicked += PuzzleMenu_DailyClicked;
                }
            }
        }
        #region Play Menu UX
        private void PlayFlyoutMenu_PlayComputerClicked(object sender, EventArgs e)
        {
            var setupControl = new Views.BoardMenu.ComputerPlaySetup();
            RightPanelContentHost.Content = setupControl;
            setupControl.OnStartGameClicked += (s, settings) =>
            {
                RightPanelContentHost.Content = DefaultInfoView;
                if (BoardViewControl != null)
                {
                    BoardViewControl.StartVsComputerGame(settings.AiDepth, settings.PlayerSide);
                }
            };
        }
        private void PlayFlyoutMenu_PlayTwoPlayerClicked(object sender, EventArgs e)
        {
            var setupControl = new Views.BoardMenu.TwoPlayerSetup();
            RightPanelContentHost.Content = setupControl;
            setupControl.OnStartGameClicked += (s, settings) =>
            {
                RightPanelContentHost.Content = DefaultInfoView;
                if (BoardViewControl != null)
                {
                    BoardViewControl.StartPvPGame(settings);
                }
            };
        }
        #endregion
        #region Puzzle Menu UX
        private void PuzzleMenu_NormalClicked(object sender, EventArgs e)
        {
            var puzzleView = new Views.BoardMenu.PuzzleInfoView();
            puzzleView.IsDailyMode = false; 
            RightPanelContentHost.Content = puzzleView;
        }
        private void PuzzleMenu_DailyClicked(object sender, EventArgs e)
        {
            var puzzleView = new Views.BoardMenu.PuzzleInfoView();
            puzzleView.IsDailyMode = true;
            RightPanelContentHost.Content = puzzleView;
        }
        #endregion
        private void Endgame_OptionSelected(object sender, string tag)
        {
            // 1. Lấy chuỗi FEN
            string fen = GetFenEndgameLesson(tag);

            // 2. Khởi động bài tập trên bàn cờ
            if (BoardViewControl != null)
            {
                BoardViewControl.StartEndgameLesson(fen);
            }
            RightPanelContentHost.Content = DefaultInfoView;
        }
        private string GetFenEndgameLesson(string tag)
        {
            switch (tag)
            {
                case "TwoRooks":
                    return "8/8/8/8/8/4k3/2R5/R3K3 w - - 0 1 600:600";

                case "Queen":
                    return "8/8/8/8/8/4k3/8/4Q1K1 w - - 0 1 600:600";

                case "Rook":
                    return "8/8/8/8/8/4k3/8/4R1K1 w - - 0 1 600:600";

                case "TwoBishops":
                    return "8/8/8/8/8/4k3/8/2B1KB2 w - - 0 1 600:600";

                case "BishopKnight":
                    return "8/8/8/8/8/4k3/8/2N1KB2 w - - 0 1 600:600";

                case "QueenRook":
                    return "2r5/8/8/8/8/4k3/8/4Q1K1 w - - 0 1 600:600";

                default:
                    return "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 600:600";
            }
        }
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            RightPanelContentHost.Content = DefaultInfoView; 

            // start logic game
        }
        private void OpenCustomBoard_Click(object sender, RoutedEventArgs e)
        {
            CustomBoardPieces customWindow = new CustomBoardPieces();

            if (customWindow.ShowDialog() == true)
            {
                int AssetId = customWindow.SelectedTheme.AssetId;
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.BoardViewControl.UpdateTheme(AssetId);
                }
            }
        }
    }
}