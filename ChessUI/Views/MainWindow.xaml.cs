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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // check BoardView loaded
            // call public function
            if (BoardViewControl != null && !BoardViewControl.IsMenuOnScreen() && e.Key == Key.Escape)
            {
                BoardViewControl.ShowPauseMenu();
            }
        }
    }
}