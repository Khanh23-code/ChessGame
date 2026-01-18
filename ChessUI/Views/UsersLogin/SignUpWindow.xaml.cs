using Microsoft.Data.SqlClient; 
using System.Configuration; 
using System;
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
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        // register event button click
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string ageText = AgeTextBox.Text;
            string level = LevelComboBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(ageText) || string.IsNullOrEmpty(level) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả thông tin.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp.");
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                MessageBox.Show("Tuổi phải là một con số.");
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
                    MessageBox.Show("Email này đã được sử dụng. Vui lòng chọn Email khác.", "Đăng ký thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (result != null)
                {
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
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
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
    }
}