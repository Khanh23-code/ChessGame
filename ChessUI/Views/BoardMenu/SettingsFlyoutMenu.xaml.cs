using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{    
    public partial class SettingsFlyoutMenu : UserControl
    {
        //private bool isDarkMode = true;
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
        private void ToggleButtonChangeMode_Loaded(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            bool isDark = app.Resources.MergedDictionaries.Any(d =>
                d.Source != null && d.Source.OriginalString.Contains("DarkMode.xaml"));

            ToggleButtonChangeMode.IsChecked = !isDark;
        }

        private void ToggleButtonChangeMode_Toggled(object sender, RoutedEventArgs e)
        {
            bool isDark = ToggleButtonChangeMode.IsChecked == false;
            ApplyTheme(isDark);
        }

        private void ApplyTheme(bool isDark)
        {
            var app = (App)Application.Current;
            var mergedDicts = app.Resources.MergedDictionaries;
            var oldTheme = mergedDicts.FirstOrDefault(d => d.Source != null &&
                (d.Source.OriginalString.Contains("DarkMode.xaml") ||
                 d.Source.OriginalString.Contains("LightMode.xaml")));

            if (oldTheme != null)
            {
                mergedDicts.Remove(oldTheme);
            }

            string newThemeSource = isDark ? "Themes/DarkMode.xaml" : "Themes/LightMode.xaml";
            ResourceDictionary newTheme = new ResourceDictionary()
            {
                Source = new Uri(newThemeSource, UriKind.RelativeOrAbsolute)
            };

            mergedDicts.Insert(0, newTheme);
        }
    }
}
