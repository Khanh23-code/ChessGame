using System;
using System.Threading.Tasks;

namespace ChessLogic
{
    /// <summary>
    /// Class quản lý AI trong game cờ vua
    /// </summary>
    public class AIController
    {
        private ChessAI ai;
        private Player aiPlayer;
        private bool isAIEnabled;

        public enum Difficulty
        {
            Easy = 2,       // Depth 2
            Medium = 3,     // Depth 3
            Hard = 4,       // Depth 4
            Expert = 5      // Depth 5
        }

        public AIController(Player aiPlayer, Difficulty difficulty = Difficulty.Medium)
        {
            this.aiPlayer = aiPlayer;
            this.ai = new ChessAI((int)difficulty);
            this.isAIEnabled = true;
        }

        /// <summary>
        /// Kiểm tra có phải lượt AI không
        /// </summary>
        public bool IsAITurn(GameState gameState)
        {
            return isAIEnabled && gameState.CurrentPlayer == aiPlayer && !gameState.IsGameOver();
        }

        /// <summary>
        /// AI thực hiện nước đi (async để không block UI)
        /// </summary>
        public async Task<Move> GetAIMoveAsync(GameState gameState)
        {
            if (!IsAITurn(gameState))
            {
                return null;
            }

            // Chạy AI trên thread riêng để không block UI
            return await Task.Run(() =>
            {
                try
                {
                    Move bestMove = ai.GetBestMove(gameState);
                    return bestMove;
                }
                catch (Exception ex)
                {
                    // Log error nếu cần
                    Console.WriteLine($"AI Error: {ex.Message}");
                    return null;
                }
            });
        }

        /// <summary>
        /// Thay đổi độ khó của AI
        /// </summary>
        public void SetDifficulty(Difficulty difficulty)
        {
            ai = new ChessAI((int)difficulty);
        }

        /// <summary>
        /// Thay đổi màu mà AI chơi
        /// </summary>
        public void SetAIPlayer(Player player)
        {
            aiPlayer = player;
        }

        /// <summary>
        /// Bật/tắt AI
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            isAIEnabled = enabled;
        }

        /// <summary>
        /// Lấy thông tin AI đang chơi màu nào
        /// </summary>
        public Player GetAIPlayer()
        {
            return aiPlayer;
        }

        /// <summary>
        /// Kiểm tra AI có đang bật không
        /// </summary>
        public bool IsEnabled()
        {
            return isAIEnabled;
        }
    }
}