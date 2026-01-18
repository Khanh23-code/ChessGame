using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLogic
{
    public class ChessAI
    {
        private readonly int maxDepth;
        private readonly Random _random = new Random();

        // Điểm số vô cực (Thắng/Thua tuyệt đối)
        private const int CHECKMATE_SCORE = 100000;
        private const int STALEMATE_SCORE = 0;

        // --- 1. BẢNG ĐIỂM VỊ TRÍ (PIECE-SQUARE TABLES) ---
        // Giúp AI biết quân cờ nằm ở đâu thì mạnh. (Ví dụ: Mã ở trung tâm > Mã ở biên)
        // Giá trị dương: Tốt cho Trắng. (Khi dùng cho Đen sẽ lật ngược bàn cờ)

        // Tốt: Thích tiến lên, chiếm trung tâm
        private static readonly int[] PawnTable = {
             0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
             5,  5, 10, 25, 25, 10,  5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5, -5,-10,  0,  0,-10, -5,  5,
             5, 10, 10,-20,-20, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };

        // Mã: Thích ở trung tâm, ghét ở góc
        private static readonly int[] KnightTable = {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        };

        // Tượng: Thích đường chéo dài, trung tâm
        private static readonly int[] BishopTable = {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
        };

        // Xe: Thích hàng 7, cột mở (Logic này xử lý thêm ở code)
        private static readonly int[] RookTable = {
             0,  0,  0,  0,  0,  0,  0,  0,
             5, 10, 10, 10, 10, 10, 10,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
             0,  0,  0,  5,  5,  0,  0,  0
        };

        // Hậu: Tự do
        private static readonly int[] QueenTable = {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -5,  0,  5,  5,  5,  5,  0, -5,
             0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };

        // Vua (Khai cuộc/Trung cuộc): Cần được bảo vệ an toàn
        private static readonly int[] KingMidTable = {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
             20, 20,  0,  0,  0,  0, 20, 20,
             20, 30, 10,  0,  0, 10, 30, 20
        };

        public ChessAI(int depth = 3)
        {
            maxDepth = depth;
        }

        // --- 2. THUẬT TOÁN TÌM NƯỚC ĐI TỐT NHẤT ---
        public Move GetBestMove(GameState gameState)
        {
            List<Move> allMoves = gameState.AllLegalMovesFor(gameState.CurrentPlayer).ToList();

            // Sắp xếp nước đi để Alpha-Beta cắt tỉa hiệu quả hơn (Ăn quân xét trước)
            allMoves = OrderMoves(allMoves, gameState.Board);

            Move bestMove = null;
            int bestScore = int.MinValue;

            // Khởi tạo Alpha-Beta
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            // Danh sách các nước đi có điểm bằng nhau (để random cho đỡ nhàm chán)
            List<Move> bestCandidates = new List<Move>();

            foreach (Move move in allMoves)
            {
                GameState newState = SimulateMove(gameState, move);

                // Gọi Minimax đệ quy
                // Lưu ý dấu trừ (-): Điểm tốt cho đối thủ là điểm xấu cho mình
                int score = -Minimax(newState, maxDepth - 1, -beta, -alpha);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                    bestCandidates.Clear();
                    bestCandidates.Add(move);
                }
                else if (score == bestScore)
                {
                    bestCandidates.Add(move);
                }

                // Cập nhật Alpha
                if (score > alpha) alpha = score;
            }

            // Chọn ngẫu nhiên một trong các nước đi tốt nhất
            if (bestCandidates.Count > 0)
            {
                return bestCandidates[_random.Next(bestCandidates.Count)];
            }

            return bestMove;
        }

        // --- 3. MINIMAX VỚI ALPHA-BETA PRUNING ---
        private int Minimax(GameState gameState, int depth, int alpha, int beta)
        {
            if (depth == 0 || gameState.IsGameOver())
            {
                // Trả về điểm đánh giá tĩnh + điểm thưởng depth (để ưu tiên thắng sớm)
                return EvaluateBoard(gameState, depth);
            }

            List<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer).ToList();

            if (moves.Count == 0)
            {
                if (gameState.Board.IsInCheck(gameState.CurrentPlayer))
                    return -CHECKMATE_SCORE - depth; // Bị chiếu hết
                return STALEMATE_SCORE; // Hòa
            }

            // Sắp xếp nước đi -> Yếu tố cốt lõi để Alpha-Beta chạy nhanh
            moves = OrderMoves(moves, gameState.Board);

            int maxEval = int.MinValue;

            foreach (Move move in moves)
            {
                GameState newState = SimulateMove(gameState, move);

                // Đệ quy Minimax (Negamax variant)
                int eval = -Minimax(newState, depth - 1, -beta, -alpha);

                if (eval > maxEval) maxEval = eval;

                // Alpha-Beta Pruning (Cắt tỉa)
                // Nếu tìm thấy nước đi tốt hơn mức mong đợi (alpha), cập nhật alpha
                if (eval > alpha) alpha = eval;

                // Nếu nhánh này tệ hơn nhánh đã tìm thấy trước đó (beta) -> Cắt bỏ
                if (alpha >= beta) break;
            }

            return maxEval;
        }

        private GameState SimulateMove(GameState currentState, Move move)
        {
            Board newBoard = currentState.Board.Copy();
            move.Execute(newBoard);
            //////
            return new GameState(currentState.CurrentPlayer.Opponent(), newBoard, currentState.WhiteTime, "Default");
            /////
        }

        // --- 4. HÀM ĐÁNH GIÁ THÔNG MINH (EVALUATION) ---
        private int EvaluateBoard(GameState gameState, int depth)
        {
            // Xử lý thắng thua
            if (gameState.IsGameOver())
            {
                if (gameState.Result.Winner == gameState.CurrentPlayer) return CHECKMATE_SCORE + depth;
                if (gameState.Result.Winner == gameState.CurrentPlayer.Opponent()) return -CHECKMATE_SCORE - depth;
                return STALEMATE_SCORE;
            }

            int score = 0;
            Player me = gameState.CurrentPlayer;
            Player opp = me.Opponent();

            // Lấy tất cả vị trí quân cờ
            var myPieces = gameState.Board.PiecePositionsFor(me);
            var oppPieces = gameState.Board.PiecePositionsFor(opp);

            // A. Đánh giá vật chất (Material)
            score += CountMaterial(gameState.Board, myPieces) - CountMaterial(gameState.Board, oppPieces);

            // B. Đánh giá vị trí (Position Tables) - ĐÂY LÀ PHẦN GIÚP MÁY KHÔN HƠN
            score += CountPositionBonus(gameState.Board, myPieces, me) - CountPositionBonus(gameState.Board, oppPieces, opp);

            // C. Điểm cơ động (Mobility) - Khuyến khích máy phát triển quân, mở đường
            // Máy sẽ tránh đi những nước làm bí cờ
            score += (gameState.AllLegalMovesFor(me).Count() - gameState.AllLegalMovesFor(opp).Count()) * 10;

            return score;
        }

        private int CountMaterial(Board board, IEnumerable<Position> positions)
        {
            int score = 0;
            foreach (var pos in positions)
            {
                score += GetPieceValue(board[pos].Type);
            }
            return score;
        }

        private int CountPositionBonus(Board board, IEnumerable<Position> positions, Player player)
        {
            int score = 0;
            foreach (var pos in positions)
            {
                Piece piece = board[pos];
                int[] table = GetTableFor(piece.Type);

                if (table != null)
                {
                    // Tính chỉ số trong mảng 1 chiều (8x8 = 64 phần tử)
                    // Nếu là quân Trắng: hàng 0 là ở trên cùng mảng
                    // Nếu là quân Đen: phải lật ngược bàn cờ (Mirror) để áp dụng cùng bảng
                    int row = (player == Player.White) ? pos.Row : (7 - pos.Row);
                    int col = pos.Column;
                    int index = row * 8 + col;

                    if (index >= 0 && index < 64)
                        score += table[index];
                }
            }
            return score;
        }

        private int[] GetTableFor(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => PawnTable,
                PieceType.Knight => KnightTable,
                PieceType.Bishop => BishopTable,
                PieceType.Rook => RookTable,
                PieceType.Queen => QueenTable,
                PieceType.King => KingMidTable,
                _ => null
            };
        }

        private int GetPieceValue(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => 100,
                PieceType.Knight => 320,
                PieceType.Bishop => 330,
                PieceType.Rook => 500,
                PieceType.Queen => 900,
                PieceType.King => 20000,
                _ => 0
            };
        }

        // --- 5. SẮP XẾP NƯỚC ĐI (MOVE ORDERING) ---
        // Kỹ thuật này giúp Alpha-Beta Pruning cắt tỉa được nhiều nhánh hơn -> Chạy nhanh hơn
        private List<Move> OrderMoves(List<Move> moves, Board board)
        {
            return moves.OrderByDescending(move =>
            {
                int score = 0;

                // Ưu tiên 1: Ăn quân (MVV-LVA: Most Valuable Victim - Least Valuable Attacker)
                // Ví dụ: Lấy Tốt ăn Hậu (Tốt) > Lấy Hậu ăn Tốt (Tệ hơn)
                if (!board.IsEmpty(move.ToPos))
                {
                    int victimValue = GetPieceValue(board[move.ToPos].Type);
                    int attackerValue = GetPieceValue(board[move.FromPos].Type);
                    score += 10 * victimValue - attackerValue;
                }

                // Ưu tiên 2: Phong cấp
                if (move.Type == MoveType.PawnPromotion) score += 1000;

                // Ưu tiên 3: Kiểm soát trung tâm (e4, d4, e5, d5)
                // Giúp máy hạn chế đi quân ra biên
                if ((move.ToPos.Row == 3 || move.ToPos.Row == 4) && (move.ToPos.Column == 3 || move.ToPos.Column == 4))
                {
                    score += 10;
                }

                return score;
            }).ToList();
        }
    }
}