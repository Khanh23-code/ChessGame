using ChessLogic;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessUI
{
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
            // populate age combobox
            for (int i = 5; i <= 100; i++)
            {
                AgeComboBox.Items.Add(i.ToString());
            }
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        // show error message
        private void ShowError(string message)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Visibility = Visibility.Visible;
        }
        // register event button click
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string ageText = AgeComboBox.Text;
            string level = LevelComboBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(ageText) || string.IsNullOrEmpty(level) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                ShowError("Vui lòng nhập đầy đủ tất cả thông tin.");
                return;
            }
            // check email format
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                ShowError("Email không đúng định dạng (thiếu @ hoặc tên miền như .com).");
                EmailTextBox.Focus();
                return;
            }

            // check full name not contain number
            if (Regex.IsMatch(fullName, @"\d"))
            {
                ShowError("Họ và tên không được chứa chữ số.");
                FullNameTextBox.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Mật khẩu xác nhận không khớp.");
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                ShowError("Tuổi phải là số.");
                return;
            }

            RegisterButton.IsEnabled = true;
            this.Cursor = Cursors.Wait;

            //string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            //string sqlInsert = @"INSERT INTO Users (Username, Password, FullName, Email, Age, ChessLevel) 
            //                     VALUES (@Username, @Password, @FullName, @Email, @Age, @ChessLevel)";

            try
            {
                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                //    using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@Username", username);
                //        cmd.Parameters.AddWithValue("@Password", password); 
                //        cmd.Parameters.AddWithValue("@FullName", fullName);
                //        cmd.Parameters.AddWithValue("@Email", email);
                //        cmd.Parameters.AddWithValue("@Age", age);
                //        cmd.Parameters.AddWithValue("@ChessLevel", level);

                //        conn.Open();
                //        int rowsAffected = cmd.ExecuteNonQuery();

                //        if (rowsAffected > 0)
                //        {
                //            MessageBox.Show("Đăng ký tài khoản thành công!", "SUCCESSFULLY REGISTER", MessageBoxButton.OKCancel);
                //            this.Close();
                //        }
                //        else
                //        {
                //            MessageBox.Show("Đăng ký thất bại");
                //        }
                //    }
                //}



                UserData newUser = new UserData
                {
                    UserName = username,
                    Password = password,
                    FullName = fullName,
                    Email = email,
                    Age = age,
                    Level = level,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Bước 2: Gọi Service
                CloudService cloudService = new CloudService();

                // Gọi hàm RegisterUserAsync và nhận về string kết quả
                string result = await cloudService.RegisterUserAsync(newUser);

                // Bước 3: Xử lý kết quả trả về dựa trên string
                if (result == "#DUPLICATE")
                {
                    ShowError("Email này đã được sử dụng. Vui lòng sử dụng Email khác.");
                    EmailTextBox.Focus();
                }
                else if (result != null)
                {
                    // Nếu result khác null và khác DUPLICATE thì đó là UserID -> Thành công
                    StatusTextBlock.Text = "Đăng ký thành công!";
                    StatusTextBlock.Foreground = Brushes.LightGreen;
                    StatusTextBlock.Visibility = Visibility.Visible;

                    RegisterButton.IsEnabled = false;
                    CancelButton.IsEnabled = false;

                    await Task.Delay(2000);
                    this.Close(); // Đóng form Register
                }
                else
                {
                    // Trường hợp trả về null
                    ShowError("Đăng ký thất bại. Vui lòng kiểm tra kết nối mạng.");
                }
            }
            //catch (SqlException ex)
            //{
            //    if (ex.Number == 2627 || ex.Number == 2601)
            //    {
            //        MessageBox.Show("Username hoặc Email này đã tồn tại. Vui lòng chọn tên khác.");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Lỗi cơ sở dữ liệu: " + ex.Message);
            //    }
            //}
            catch (Exception ex)
            {
                ShowError("Lỗi hệ thống: " + ex.Message);
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region ShowPassword
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
        private void EyeConfirmButton_Show(object sender, MouseButtonEventArgs e)
        {
            ConfirmPasswordShowBox.Text = ConfirmPasswordBox.Password;
            ConfirmPasswordShowBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
        }
        private void EyeConfirmButton_Hide(object sender, MouseEventArgs e)
        {
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordShowBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordShowBox.Text = "";
        }
        #endregion
    }
}