// remember settings
using ChessUI.Properties;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// remember settings
using ChessLogic;


namespace ChessUI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadRememberedUser();
        }
        private void LoadRememberedUser()
        {
            if (Settings.Default.RememberMe)
            {
                RememberMeCheckBox.IsChecked = true;
                EmailTextBox.Text = Settings.Default.RememberedUsername;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        #region Show Errors
        private void ShowError(Control inputControl, TextBlock errorTextBlock, string message)
        {
            inputControl.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5252"));

            errorTextBlock.Text = $"❗ {message}";
            errorTextBlock.Visibility = Visibility.Visible;
        }
        private void ClearErrors()
        {
            EmailTextBox.ClearValue(Control.BorderBrushProperty);
            PasswordBox.ClearValue(Control.BorderBrushProperty);
            PasswordShowBox.ClearValue(Control.BorderBrushProperty);

            ErrorText.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region Taskbar Button
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
        }
        // Change content of btnMaximize
        private void Window_StateChanged(object sender, EventArgs e)
        {
            // check if Window in Maximized state change btn Maximize content
            if (this.WindowState == WindowState.Maximized)
            {
                btnMaximize.Content = "\uE923";
            }
            // otherwise
            else
            {
                btnMaximize.Content = "\uE922";
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Login
        //private void LoginButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string username = UsernameTextBox.Text;
        //    string password = PasswordBox.Password;

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu");
        //        return;
        //    }

        //    string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        //    string sqlQuery = "SELECT COUNT(1) FROM Users WHERE Username = @Username and Password = @Password";

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Username", username);
        //                cmd.Parameters.AddWithValue("@Password", password);

        //                conn.Open();
        //                int count = (int)cmd.ExecuteScalar();

        //                if (count == 1) // Khi có data khớp
        //                {
        //                    // logic Remember Me
        //                    if (RememberMeCheckBox.IsChecked == true)
        //                    {
        //                        // if checked is selected, save user name and state
        //                        Settings.Default.RememberedUsername = username;
        //                        Settings.Default.RememberMe = true;
        //                    }
        //                    else
        //                    {
        //                        // otherwise, delete settings
        //                        Settings.Default.RememberedUsername = "";
        //                        Settings.Default.RememberMe = false;
        //                    }
        //                    Settings.Default.Save();
        //                    // Login successfully
        //                    MainWindow mainWindow = new MainWindow();
        //                    mainWindow.Show();
        //                    this.Close();
        //                }
        //                else
        //                {
        //                    // Login failed
        //                    MessageBox.Show("Username hoặc Password bị sai");
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
        //    }
        //}

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            //this.Close();

            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            ClearErrors();


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(email))
                {
                    ShowError(EmailTextBox, ErrorText, "Email đăng nhập không thể để trống.");
                    EmailTextBox.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(password))
                {
                    ShowError(PasswordBox, ErrorText, "Mật khẩu không thể để trống.");
                    PasswordBox.Focus();
                    return;
                }
                return;
            }

            LoginButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {

                // Gọi CloudService
                CloudService cloudService = new CloudService();

                // Gọi hàm LoginUserAsync. Kết quả trả về là object UserData hoặc null
                UserData loggedInUser = await cloudService.LoginUserAsync(email, password);

                if (loggedInUser != null)
                {
                    // --- ĐẶT TÀI KHOẢN MẶC ĐỊNH ---
                    if (RememberMeCheckBox.IsChecked == true)
                    {
                        Settings.Default.RememberedUsername = email;
                        Settings.Default.RememberMe = true;
                    }
                    else
                    {
                        Settings.Default.RememberedUsername = "";
                        Settings.Default.RememberMe = false;
                    }
                    Settings.Default.Save();

                    // --- CHUYỂN MÀN HÌNH ---
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); 
                }
                else
                {
                    // Login failed (Thông báo khi Firebase bảo sai mật khẩu)
                    ShowError(PasswordBox, ErrorText, "Mật khẩu không chính xác. Vui lòng thử lại.");
                }

            }
            catch (Exception ex)
            {
                // Bắt các lỗi chung (ví dụ lỗi mạng internet khi gọi Firebase)
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        #endregion
        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // I can't do it
            return;
        }
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.ShowDialog();
        }

        #region ShowPassword
        private bool isPasswordVisible = false;
        private void EyeButton_Show(object sender, MouseButtonEventArgs e)
        {
            PasswordShowBox.Text = PasswordBox.Password;
            PasswordShowBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
        }
        private void EyeButton_Hide(object sender, MouseEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Visible;
            PasswordShowBox.Visibility = Visibility.Collapsed;
            PasswordShowBox.Text = "";
        }
        #endregion
    }
}