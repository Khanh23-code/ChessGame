using ChessLogic; // Import namespace chứa Logic của bạn
using System.Threading.Tasks;

namespace ChessUI
{
    public class ChessAIService
    {
        private ChessAI _ai;
        public Player AIPlayer { get; private set; }

        public ChessAIService()
        { 
            _ai = new ChessAI(depth: 3);
        }

        public void SetDifficulty(int depth)
        {
            // Cho phép chỉnh độ khó bằng độ sâu (1: Dễ, 2: Trung bình, 3-4: Khó)
            _ai = new ChessAI(depth);
        }

        public void SetAIPlayer(Player player)
        {
            // Lưu vào biến của class này thay vì gọi aiController
            this.AIPlayer = player;
        }

        // Thay vì trả về string (như "e2e4"), ta trả về đối tượng Move chính xác
        public async Task<Move> GetBestMoveAsync(GameState currentState)
        {
            return await Task.Run(() =>
            {
                // Gọi hàm AI trong luồng phụ để không đơ giao diện
                return _ai.GetBestMove(currentState);
            });
        }
    }
}