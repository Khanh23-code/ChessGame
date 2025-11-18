//using ChessLogic;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

//namespace ChessUI
//{
//    public static class Images
//    {
//        private static readonly Dictionary<PieceType, ImageSource> WhitePieces = new()
//        {
//            { PieceType.Pawn, LoadImage("/Assets/Asset1/PawnW.png") },
//            { PieceType.Bishop, LoadImage("/Assets/Asset1/BishopW.png") },
//            { PieceType.Rook, LoadImage("/Assets/Asset1/RookW.png") },
//            { PieceType.Knight, LoadImage("/Assets/Asset1/KnightW.png") },
//            { PieceType.Queen, LoadImage("/Assets/Asset1/QueenW.png") },
//            { PieceType.King, LoadImage("/Assets/Asset1/KingW.png") }
//        };
//        private static readonly Dictionary<PieceType, ImageSource> BlackPieces = new()
//        {
//            { PieceType.Pawn, LoadImage("/Assets/Asset1/PawnB.png") },
//            { PieceType.Bishop, LoadImage("/Assets/Asset1/BishopB.png") },
//            { PieceType.Rook, LoadImage("/Assets/Asset1/RookB.png") },
//            { PieceType.Knight, LoadImage("/Assets/Asset1/KnightB.png") },
//            { PieceType.Queen, LoadImage("/Assets/Asset1/QueenB.png") },
//            { PieceType.King, LoadImage("/Assets/Asset1/KingB.png") }
//        };

//        // Có vẻ hàm này khi cần sử dụng Image đều cần
//        private static ImageSource LoadImage(string path)
//        {
//            return new BitmapImage(new Uri(path, UriKind.Relative));
//        }

//        public static ImageSource GetImage(Player color, PieceType type)
//        {
//            if (color == Player.White) return WhitePieces[type];
//            else return BlackPieces[type];
//        }

//        public static ImageSource GetImage(Piece piece)
//        {
//            if (piece == null) return null;

//            return GetImage(piece.Color, piece.Type);
//        }
//    }
//}

using ChessLogic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    public static class Images
    {
        // 1. Tạo một "cache" để lưu các bộ asset đã được load
        //    Khóa (Key) là int (index), Giá trị (Value) là PieceSet (bộ cờ)
        private static readonly Dictionary<int, PieceSet> AssetCache = new();

        // 2. Tạo một class private để chứa CẢ HAI dictionary
        private class PieceSet
        {
            public readonly Dictionary<PieceType, ImageSource> WhitePieces = new();
            public readonly Dictionary<PieceType, ImageSource> BlackPieces = new();

            public PieceSet(int assetIndex)
            {
                string pathPrefix = $"/Assets/Asset{assetIndex}/";

                WhitePieces = new()
                {
                    { PieceType.Pawn, LoadImage(pathPrefix + "PawnW.png") },
                    { PieceType.Bishop, LoadImage(pathPrefix + "BishopW.png") },
                    { PieceType.Rook, LoadImage(pathPrefix + "RookW.png") },
                    { PieceType.Knight, LoadImage(pathPrefix + "KnightW.png") },
                    { PieceType.Queen, LoadImage(pathPrefix + "QueenW.png") },
                    { PieceType.King, LoadImage(pathPrefix + "KingW.png") }
                };

                BlackPieces = new()
                {
                    { PieceType.Pawn, LoadImage(pathPrefix + "PawnB.png") },
                    { PieceType.Bishop, LoadImage(pathPrefix + "BishopB.png") },
                    { PieceType.Rook, LoadImage(pathPrefix + "RookB.png") },
                    { PieceType.Knight, LoadImage(pathPrefix + "KnightB.png") },
                    { PieceType.Queen, LoadImage(pathPrefix + "QueenB.png") },
                    { PieceType.King, LoadImage(pathPrefix + "KingB.png") }
                };
            }
        }

        private static ImageSource LoadImage(string path)
        {
            // Thêm try-catch để an toàn hơn khi load ảnh
            try
            {
                return new BitmapImage(new Uri(path, UriKind.Relative));
            }
            catch (Exception ex)
            {
                // Handle lỗi nếu không tìm thấy ảnh (ví dụ: Asset 3 không có ảnh)
                // Bạn có thể trả về một ảnh mặc định hoặc log lỗi
                Console.WriteLine($"Error loading image {path}: {ex.Message}");
                return null;
            }
        }

        // 3. Hàm helper để lấy từ cache (hoặc tạo mới nếu chưa có)
        private static PieceSet GetSet(int assetIndex)
        {
            // Thử tìm trong cache trước
            if (AssetCache.TryGetValue(assetIndex, out PieceSet set))
            {
                return set;
            }

            // Không tìm thấy -> tạo mới
            PieceSet newSet = new PieceSet(assetIndex);

            // Lưu vào cache cho lần sau
            AssetCache[assetIndex] = newSet;
            return newSet;
        }

        // 4. Cập nhật các hàm public GetImage

        // Hàm này giờ sẽ nhận thêm assetIndex
        public static ImageSource GetImage(Player color, PieceType type, int assetIndex = 1) // Đặt mặc định là 1
        {
            // Lấy bộ cờ từ cache (hoặc tạo mới)
            PieceSet set = GetSet(assetIndex);

            if (color == Player.White)
            {
                return set.WhitePieces.GetValueOrDefault(type);
            }

            return set.BlackPieces.GetValueOrDefault(type);
        }

        // Hàm này cũng cần biết assetIndex
        public static ImageSource GetImage(Piece piece, int assetIndex = 1)
        {
            if (piece == null)
            {
                return null;
            }

            return GetImage(piece.Color, piece.Type, assetIndex);
        }
    }
}
