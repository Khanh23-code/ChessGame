using ChessLogic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> WhitePieces = new()
        {
            { PieceType.Pawn, LoadImage("Assets/PawnW.png") },
            { PieceType.Bishop, LoadImage("Assets/BishopW.png") },
            { PieceType.Rook, LoadImage("Assets/RookW.png") },
            { PieceType.Knight, LoadImage("Assets/KnightW.png") },
            { PieceType.Queen, LoadImage("Assets/QueenW.png") },
            { PieceType.King, LoadImage("Assets/KingW.png") }
        };
        private static readonly Dictionary<PieceType, ImageSource> BlackPieces = new()
        {
            { PieceType.Pawn, LoadImage("Assets/PawnB.png") },
            { PieceType.Bishop, LoadImage("Assets/BishopB.png") },
            { PieceType.Rook, LoadImage("Assets/RookB.png") },
            { PieceType.Knight, LoadImage("Assets/KnightB.png") },
            { PieceType.Queen, LoadImage("Assets/QueenB.png") },
            { PieceType.King, LoadImage("Assets/KingB.png") }
        };

        // Có vẻ hàm này khi cần sử dụng Image đều cần
        private static ImageSource LoadImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            if (color == Player.White) return WhitePieces[type];
            else return BlackPieces[type];
        }

        public static ImageSource GetImage(Piece piece)
        {
            if (piece == null) return null;

            return GetImage(piece.Color, piece.Type);
        }
    }
}
