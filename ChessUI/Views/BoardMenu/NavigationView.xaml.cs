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

namespace ChessUI.Views.BoardMenu
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
        public enum MenuPage
        {
            Play,
            Puzzle,
            Learn,
            More,
            Settings
        }
        public class NavigationEventArgs : EventArgs
        {
            public MenuPage Page { get; set; }
        }

        public partial class NavigationView : UserControl
        {
            // Sự kiện bắn ra ngoài
            public event EventHandler<NavigationEventArgs> SelectionChanged;

            public NavigationView()
            {
                InitializeComponent();
            }

            private void OnMenuButtonClick(object sender, RoutedEventArgs e)
            {
                if (sender is RadioButton btn && btn.CommandParameter != null)
                {
                    string pageName = btn.CommandParameter.ToString();
                    if (Enum.TryParse(pageName, out MenuPage selectedPage))
                    {
                        SelectionChanged?.Invoke(this, new NavigationEventArgs { Page = selectedPage });
                    }
                }
            }

        private void LogoButton_Click(object sender, RoutedEventArgs e)
        {
            // refresh main window by close and open again
            var newMainWindow = new MainWindow();
            newMainWindow.Show();
        }
    }
}
