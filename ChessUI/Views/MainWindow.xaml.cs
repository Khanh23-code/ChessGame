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

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        #region Taskbar Button
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
    }
}