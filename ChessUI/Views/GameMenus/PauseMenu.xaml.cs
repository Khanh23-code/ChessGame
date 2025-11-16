using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for PauseMenu.xaml
    /// </summary>
    public partial class PauseMenu : UserControl
    {
        #region Properties / Constructors
        public event Action<Option> OptionSelected;

        public PauseMenu()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Continue);
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);

        }
        #endregion
    }
}
