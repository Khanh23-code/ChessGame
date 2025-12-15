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
        private readonly Views.BoardMenu.InfoView DefaultInfoView = new Views.BoardMenu.InfoView();
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight; 
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

            if (BoardViewControl != null && !BoardViewControl.IsMenuOnScreen() && e.Key == Key.Space)
            {
                BoardViewControl.ChangeAsset();
            }
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
                learnMenu.ChessMovesLessonClicked += LearnMenu_ChessMovesLessonClicked;
            }
        }
        private void LearnMenu_ChessMovesLessonClicked(object sender, EventArgs e)
        {
            OpenOverlay(new Views.BoardMenu.ChessMovesLesson());
        }
        private void OnMainNavigationChanged(object sender, ChessUI.Views.BoardMenu.NavigationEventArgs e)
        {
            NavigateToPage(e.Page);

            if (e.Page == MenuPage.Play)
            {
                NavigationView navView = (NavigationView)sender;
                Popup playSubMenuPopup = (Popup)navView.FindName("PlaySubMenu");
                if (playSubMenuPopup != null && playSubMenuPopup.Child is PlayFlyoutMenu playFlyoutMenu)
                {
                    playFlyoutMenu.PlayComputerClicked -= PlayFlyoutMenu_PlayComputerClicked;
                    playFlyoutMenu.PlayComputerClicked += PlayFlyoutMenu_PlayComputerClicked;

                    playFlyoutMenu.PlayTwoPlayerClicked -= PlayFlyoutMenu_PlayTwoPlayerClicked;
                    playFlyoutMenu.PlayTwoPlayerClicked += PlayFlyoutMenu_PlayTwoPlayerClicked;
                }
            }
        }
        private void PlayFlyoutMenu_PlayComputerClicked(object sender, EventArgs e)
        {
            var setupControl = new Views.BoardMenu.ComputerPlaySetup();
            // change InfoView to ComputerPlaySetup View
            RightPanelContentHost.Content = setupControl; 
            setupControl.StartGameButton.Click += StartGameButton_Click;
        }
        private void PlayFlyoutMenu_PlayTwoPlayerClicked(object sender, EventArgs e)
        {
            var setupControl = new Views.BoardMenu.TwoPlayerSetup();
            // change InfoView to TwoPlayerSetup View
            RightPanelContentHost.Content = setupControl; 
            setupControl.StartTwoPlayerButton.Click += StartGameButton_Click;
        }
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            RightPanelContentHost.Content = DefaultInfoView; 

            // start logic game
        }
    }
}