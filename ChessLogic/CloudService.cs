using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameSaveData
    {
        public string FenString { get; set; }
        public string TimeStamp { get; set; }
    }

    public class UserData
    {
        // ID này sẽ trùng với Key trên Firebase (để tiện truy xuất ngược)
        public string UserID { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Level { get; set; }
        public string CreatedAt { get; set; }
        public int AvatarIndex { get; set; } = 0;
    }

    public class CloudService
    {
        // Cấu hình Firebase
        private IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "5hZPuEohGg0iSSshFQ41pmm6DDjNpgGLGPIhGmDO",
            BasePath = "https://chessgameproject-fdc0b-default-rtdb.firebaseio.com/"
        };

        private IFirebaseClient client;

        public CloudService()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                // Logic không nên hiện MessageBox. 
                // Nếu lỗi kết nối thì client = null, các hàm sau sẽ tự bỏ qua.
            }
        }

        #region Game Data Tasks: Save, Load, Delete
        public async Task<bool> SaveGameAsync(string userID, string modeKey, string fen)
        {
            if (client == null) return false;

            try
            {
                // Đóng gói dữ liệu
                var data = new GameSaveData
                {
                    FenString = fen,
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Gửi lên đám mây (Firebase)
                await client.SetAsync($"Games/{userID}/{modeKey}", data);
                return true;
            }
            catch
            {
                return false; // Lưu thất bại
            }
        }

        public async Task<string> LoadGameAsync(string userID, string mode)
        {
            if (client == null) return null;

            try
            {
                // Lấy dự liệu từ đám mây (Firebase)
                FirebaseResponse response = await client.GetAsync($"Games/{userID}/{mode}");
                // Chuyển dữ liệu thành đối tượng GameSaveData
                GameSaveData data = response.ResultAs<GameSaveData>();

                // Trả về chuỗi FEN từ dữ liệu
                return data?.FenString;
            }
            catch
            {
                return null; // Tải thất bại
            }
        }

        public async Task<bool> DeleteGameAsync(string userID, string mode)
        {
            if (client == null) return false;

            try
            {
                await client.DeleteAsync($"Games/{userID}/{mode}");
                return true;
            }
            catch
            {
                return false; // Xóa thất bại
            }
        }
        #endregion

        #region User Data Tasks: Register, Login
        public async Task<bool> CheckUserExistsAsync(string email)
        {
            if (client == null) return false;
            try
            {
                // Lấy tất cả người dùng từ Firebase
                FirebaseResponse response = await client.GetAsync("Users");
                if (response.Body == "null")
                {
                    return false; // Chưa có người dùng nào
                }

                var allUsers = response.ResultAs<Dictionary<string, UserData>>();
                foreach (var userEntry in allUsers)
                {
                    var user = userEntry.Value;
                    if (user.Email == email)
                    {
                        return true; // Email đã tồn tại
                    }
                }
                return false; // Email chưa tồn tại
            }
            catch
            {
                return false; // Lỗi khi kiểm tra
            }
        }
        
        public async Task<string> RegisterUserAsync(UserData newUser)
        {
            if (client == null) return null;

            bool isExist = await CheckUserExistsAsync(newUser.Email);
            if (isExist)
            {
                return "#DUPLICATE"; // Trả về mã lỗi riêng để UI biết mà báo "Tên đã tồn tại"
            }

            try 
            {
                // Tạo một UserID mới (có thể dùng GUID hoặc bất kỳ phương pháp nào khác)
                string userID = Guid.NewGuid().ToString();
                newUser.UserID = userID;
                newUser.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                // Lưu dữ liệu người dùng lên Firebase
                await client.SetAsync($"Users/{userID}", newUser);

                return userID; // Trả về UserID nếu đăng ký thành công
            }
            catch
            {
                return null; // Đăng ký thất bại
            }
        }

        public async Task<UserData> LoginUserAsync(string email, string password)
        {
            if (client == null) return null;

            try
            {
                // Lấy tất cả người dùng từ Firebase
                FirebaseResponse response = await client.GetAsync("Users");
                if (response.Body == "null")
                {
                    return null; // Chưa có người dùng nào
                }

                var allUsers = response.ResultAs<Dictionary<string, UserData>>();

                // Tìm người dùng với username và password khớp
                foreach (var userEntry in allUsers)
                {
                    var user = userEntry.Value;
                    if (user.Email == email)
                    {
                        bool isCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);
                        // if find match, return user data
                        if (isCorrect)
                            return user; 
                        else
                            return null; 
                    }
                }
                return null; // Không tìm thấy người dùng khớp
            }
            catch
            {
                return null; // Đăng nhập thất bại
            }
        }
        #endregion
    }
}
