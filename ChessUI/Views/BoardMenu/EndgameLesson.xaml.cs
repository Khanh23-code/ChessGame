using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class EndgameLesson : UserControl
    {
        public event EventHandler<string> OptionSelected;
        public EndgameLesson()
        {
            InitializeComponent();
        }

        private void BtnOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                OptionSelected?.Invoke(this, btn.Tag.ToString());
            }
        }
    }
}
