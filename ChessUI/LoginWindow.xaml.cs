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
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.Configuration;
// remember settings
using ChessUI.Properties;


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
                UsernameTextBox.Text = Settings.Default.RememberedUsername;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        // taskbar button
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
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu");
                return;
            }
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            string sqlQuery = "SELECT COUNT(1) FROM Users WHERE Username = @Username and Password = @Password";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();

                        if (count == 1)
                        {
                            // logic Remember Me
                            if (RememberMeCheckBox.IsChecked == true)
                            {
                                // if checked is selected, save user name and state
                                Settings.Default.RememberedUsername = username;
                                Settings.Default.RememberMe = true;
                            }
                            else
                            {
                                // otherwise, delete settings
                                Settings.Default.RememberedUsername = "";
                                Settings.Default.RememberMe = false;
                            }
                            Settings.Default.Save();
                            // Login successfully
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            // Login failed
                            MessageBox.Show("Username hoặc Password bị sai");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.ShowDialog();
        }
    }
}