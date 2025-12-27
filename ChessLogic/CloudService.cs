using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
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

        // Class lưu dữ liệu (tạm): FEN, Time là thời gian lưu
        public class GameSaveData
        {
            public string FenString { get; set; }
            public string TimeStamp { get; set; }
        }

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

    }
}
