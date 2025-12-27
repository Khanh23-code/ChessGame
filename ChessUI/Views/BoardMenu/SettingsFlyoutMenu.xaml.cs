using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{    
    public partial class SettingsFlyoutMenu : UserControl
    {
        public SettingsFlyoutMenu()
        {
            InitializeComponent();
        }

        private void LogOutRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Application.Current.MainWindow.Close();
        }

        private void CustomRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var customRadioButton = sender as RadioButton;
            CustomBoardPieces customWindow = new CustomBoardPieces();
            if (customWindow.ShowDialog() == true)
            {
                int AssetId = customWindow.SelectedTheme.AssetId;
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null && mainWindow.BoardViewControl != null)
                {
                    mainWindow.BoardViewControl.UpdateTheme(AssetId);
                }
            }
            if (customRadioButton != null)
            {
                customRadioButton.IsChecked = false;
            }
        }
    }
}
