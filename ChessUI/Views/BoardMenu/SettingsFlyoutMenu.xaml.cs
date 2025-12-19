using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
    /// Interaction logic for SettingsFlyoutMenu.xaml
    /// </summary>
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
            // close the current main window
            Application.Current.MainWindow.Close();
        }
    }
}
