using ChessLogic;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChessUI.Views.UsersLogin
{
    public partial class ForgotPassword : Window
    {
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            //    string email = EmailTextBox.Text.Trim();
            //    ErrorText.Visibility = Visibility.Collapsed;

            //    if (string.IsNullOrEmpty(email))
            //    {
            //        ErrorText.Text = "Vui lòng nhập email.";
            //        ErrorText.Visibility = Visibility.Visible;
            //        EmailTextBox.Focus();
            //        return;
            //    }
            //    BtnSend.IsEnabled = false;
            //    BtnCancel.IsEnabled = false;
            //    this.Cursor = Cursors.Wait;

            //    try
            //    {
            //        CloudService cloudService = new CloudService();
            //        bool isSent = await cloudService.ResetPasswordAsync(email);

            //        if (isSent)
            //        {
            //            MessageBox.Show($"Đã gửi link đặt lại mật khẩu tới {email}.\nVui lòng kiểm tra email.",
            //                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            //            this.Close(); 
            //        }
            //        else
            //        {
            //            ErrorText.Text = "Có lỗi xảy ra hoặc email không tồn tại.";
            //            ErrorText.Visibility = Visibility.Visible;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ErrorText.Text = "Lỗi: " + ex.Message;
            //        ErrorText.Visibility = Visibility.Visible;
            //    }
            //    finally
            //    {
            //        // Mở lại nút
            //        BtnSend.IsEnabled = true;
            //        BtnCancel.IsEnabled = true;
            //        this.Cursor = Cursors.Arrow;
            //    }
        }
    }
}