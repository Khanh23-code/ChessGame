using ChessLogic; // Namespace chứa CloudService
using ChessUI.Views.GameMenus;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChessUI.Views.UsersLogin
{
    public partial class ForgotPassword : Window
    {
        private int _currentStep = 1;
        private string _serverOtp = "";
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
            TxtError.Text = ""; 
            
            // step 1 enter the mail
            if (_currentStep == 1)
            {
                _userEmail = TxtEmail.Text.Trim();
                if (string.IsNullOrEmpty(_userEmail))
                {
                    TxtError.Text = "Please enter your email.";
                    return;
                }

                BtnMain.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                CloudService service = new CloudService();

                // check email existed 
                bool exists = await service.CheckUserExistsAsync(_userEmail); 
                if (!exists)
                {
                    TxtError.Text = "This email is not registered.";
                    BtnMain.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                    return;
                }
                Random rand = new Random();
                _serverOtp = rand.Next(100000, 999999).ToString();

                bool sent = await Task.Run(() => service.SendVerificationCode(_userEmail, _serverOtp));

                if (sent)
                {
                    // step 2
                    _currentStep = 2;
                    Step1_Email.Visibility = Visibility.Collapsed;
                    Step2_Otp.Visibility = Visibility.Visible;

                    TitleText.Text = "OTP verification";
                    BtnMain.Content = "Confirm";
                    TxtError.Text = "";
                }
                else
                {
                    TxtError.Text = "Unable to send email. Please try again.";
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

                    TitleText.Text = "Reset password";
                    BtnMain.Content = "Change password";
                    TxtError.Text = "";
                }
                else
                {
                    TxtError.Text = "Invalid verification code.";
                }
            }
            // Save new password
            else if (_currentStep == 3)
            {
                string newPass = PwdNew.Password;
                string confirmPass = PwdConfirm.Password;

                if (string.IsNullOrEmpty(newPass))
                {
                    TxtError.Text = "Please enter a new password.";
                    return;
                }
                if (newPass != confirmPass)
                {
                    TxtError.Text = "The confirmation password does not match.";
                    return;
                }

                BtnMain.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                CloudService service = new CloudService();
                bool updated = await service.UpdateUserPasswordAsync(_userEmail, newPass);

                if (updated)
                {
                    CustomMessageBox.Show("Password changed successfully! Please log in again.");
                    this.Close();
                }
                else
                {
                    TxtError.Text = "Error saving data. Please try again.";
                }

                BtnMain.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}