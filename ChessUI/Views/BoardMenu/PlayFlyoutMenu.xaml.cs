using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace ChessUI.Views.BoardMenu
{
    /// <summary>
    /// Interaction logic for PlayFlyoutMenu.xaml
    /// </summary>
    public partial class PlayFlyoutMenu : UserControl
    {
        public event EventHandler PlayComputerClicked;
        public event EventHandler PlayTwoPlayerClicked;
        public PlayFlyoutMenu()
        {
            InitializeComponent();
        }
        private void PlayComputerButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn && btn.IsChecked == true)
            {
                PlayComputerClicked?.Invoke(this, EventArgs.Empty);
            }
        }
        private void TwoPlayersButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn && btn.IsChecked == true)
            {
                PlayTwoPlayerClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}