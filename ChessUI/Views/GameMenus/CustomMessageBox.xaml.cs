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

namespace ChessUI.Views.GameMenus
{
    public partial class CustomMessageBox : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public CustomMessageBox(string message, string title = "Thông báo", bool showCancel = false)
        {
            InitializeComponent();
            txtMessage.Text = message;
            txtTitle.Text = title;
            if (!showCancel)
            {
                btnCancel.Visibility = Visibility.Collapsed;
            }
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.DialogResult = true; 
            this.Close();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.DialogResult = false; 
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public static bool Show(string message, string title = "Thông báo", bool showCancel = false)
        {
            var msgBox = new CustomMessageBox(message, title, showCancel);
            var result = msgBox.ShowDialog();
            return result ?? false;
        }
    }
}
