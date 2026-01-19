using ChessLogic; // Namespace chứa CloudService
using System;
using System.Windows;
using System.Windows.Input;

namespace ChessUI.Views.UsersLogin
{
    public partial class ForgotPassword : Window
    {
        // Biến lưu trạng thái hiện tại (1: Email, 2: OTP, 3: Pass mới)
        private int _currentStep = 1;

        // Biến lưu mã OTP hệ thống tạo ra để so sánh
        private string _serverOtp = "";

        // Biến lưu email người dùng nhập
        private string _userEmail = "";

        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = ""; // Xóa lỗi cũ

            // --- BƯỚC 1: XỬ LÝ GỬI EMAIL ---
            if (_currentStep == 1)
            {
                _userEmail = TxtEmail.Text.Trim();
                if (string.IsNullOrEmpty(_userEmail))
                {
                    TxtError.Text = "Vui lòng nhập email.";
                    return;
                }

                BtnMain.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                CloudService service = new CloudService();

                // Kiểm tra xem email có tồn tại không trước khi gửi
                bool exists = await service.CheckUserExistsAsync(_userEmail); // Bạn cần đảm bảo hàm này public
                if (!exists)
                {
                    TxtError.Text = "Email này chưa được đăng ký.";
                    BtnMain.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                    return;
                }

                // Tạo mã OTP ngẫu nhiên 6 số
                Random rand = new Random();
                _serverOtp = rand.Next(100000, 999999).ToString();

                // Gửi mail
                // Lưu ý: Cần chạy cái này trong Task.Run để tránh đơ UI nếu gửi lâu
                bool sent = await Task.Run(() => service.SendVerificationCode(_userEmail, _serverOtp));

                if (sent)
                {
                    // Chuyển sang Bước 2
                    _currentStep = 2;
                    Step1_Email.Visibility = Visibility.Collapsed;
                    Step2_Otp.Visibility = Visibility.Visible;

                    TitleText.Text = "Xác thực OTP";
                    BtnMain.Content = "Xác nhận";
                    TxtError.Text = "";
                }
                else
                {
                    TxtError.Text = "Không gửi được email. Vui lòng thử lại.";
                }

                BtnMain.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
            // --- BƯỚC 2: XỬ LÝ CHECK OTP ---
            else if (_currentStep == 2)
            {
                string inputOtp = TxtOtp.Text.Trim();
                if (inputOtp == _serverOtp)
                {
                    // OTP đúng -> Chuyển sang Bước 3
                    _currentStep = 3;
                    Step2_Otp.Visibility = Visibility.Collapsed;
                    Step3_NewPass.Visibility = Visibility.Visible;

                    TitleText.Text = "Đặt lại mật khẩu";
                    BtnMain.Content = "Đổi mật khẩu";
                    TxtError.Text = "";
                }
                else
                {
                    TxtError.Text = "Mã xác thực không đúng.";
                }
            }
            // --- BƯỚC 3: LƯU MẬT KHẨU MỚI ---
            else if (_currentStep == 3)
            {
                string newPass = PwdNew.Password;
                string confirmPass = PwdConfirm.Password;

                if (string.IsNullOrEmpty(newPass))
                {
                    TxtError.Text = "Vui lòng nhập mật khẩu mới.";
                    return;
                }
                if (newPass != confirmPass)
                {
                    TxtError.Text = "Mật khẩu xác nhận không khớp.";
                    return;
                }

                BtnMain.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                CloudService service = new CloudService();
                bool updated = await service.UpdateUserPasswordAsync(_userEmail, newPass);

                if (updated)
                {
                    MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành công");
                    this.Close();
                }
                else
                {
                    TxtError.Text = "Lỗi khi lưu dữ liệu. Vui lòng thử lại.";
                }

                BtnMain.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}