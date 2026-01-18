using Microsoft.Data.SqlClient; 
using System;
using System.Configuration; 
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ChessLogic;
using System.Windows;
using System.Windows.Input;

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
        private void ShowError (string message)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Visibility = Visibility.Visible;
        }
        // register event button click
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Visibility = Visibility.Collapsed;
            StatusTextBlock.Text = "";
            StatusTextBlock.Foreground = Brushes.Red;

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
            try
            {
                var mailAddress = new MailAddress(email);
                if (mailAddress.Address != email) throw new FormatException();
            }
            catch (FormatException)
            {
                ShowError("Email không đúng định dạng.");
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

            // check confirm password
            if (password != confirmPassword)
            {
                ShowError("Mật khẩu xác nhận không khớp.");
                return;
            }

            // check age is number
            if (!int.TryParse(ageText, out int age))
            {
                ShowError("Tuổi phải là số.");
                return;
            }
            // insert data to database
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            string sqlInsert = @"INSERT INTO Users (Username, Password, FullName, Email, Age, ChessLevel) 
                                 VALUES (@Username, @Password, @FullName, @Email, @Age, @ChessLevel)";

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
                    using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password); 
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@ChessLevel", level);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            StatusTextBlock.Text = "Đăng ký thành công!";
                            StatusTextBlock.Foreground = Brushes.LightGreen;
                            StatusTextBlock.Visibility = Visibility.Visible;

                            //this.Close();   
                        }
                        else
                        {
                            ShowError("Đăng ký thất bại.");
                        }
                    }
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
                    MessageBox.Show("Email này đã được sử dụng. Vui lòng chọn Email khác.", "Đăng ký thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (result != null)
                {
                    ShowError("Username hoặc Email này đã tồn tại.");
                }
                else
                {
                    StatusTextBlock.Text = "Lỗi Database: " + ex.Message;
                    // Nếu result khác null và khác DUPLICATE thì đó là UserID -> Thành công
                    MessageBox.Show("Đăng ký tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Đóng form Register
                }
                else
                {
                    // Trường hợp trả về null
                    MessageBox.Show("Đăng ký thất bại. Vui lòng kiểm tra kết nối mạng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                StatusTextBlock.Text = "Lỗi hệ thống: " + ex.Message;
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
        private bool isPasswordVisible = false;
        private bool isConfirmPasswordVisible = false;
        private void EyeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible == false)
            {
                PasswordShowBox.Text = PasswordBox.Password;
                PasswordShowBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordShowBox.Focus();
                PasswordShowBox.Select(PasswordShowBox.Text.Length, 0);
                isPasswordVisible = true;
            }
            else
            {
                PasswordBox.Password = PasswordShowBox.Text;
                PasswordShowBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordShowBox.Text = "";

                isPasswordVisible = false;
            }
        }

        private void EyeConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConfirmPasswordVisible == false)
            {
                ConfirmPasswordShowBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordShowBox.Visibility = Visibility.Visible;
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordShowBox.Focus();
                ConfirmPasswordShowBox.Select(ConfirmPasswordShowBox.Text.Length, 0);
                isConfirmPasswordVisible = true;
            }
            else
            {
                ConfirmPasswordBox.Password = ConfirmPasswordShowBox.Text;
                ConfirmPasswordShowBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordBox.Visibility = Visibility.Visible;
                ConfirmPasswordShowBox.Text = "";

                isConfirmPasswordVisible = false;
            }
        }
        #endregion
    }
}