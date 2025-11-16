namespace ChessLogic
{
    public enum EndReason
    {
        Checkmate,          // Chiếu hết
        Stalemate,          // Không thể di chuyển => hòa
        FiftyMoveRule,      // Luật 50 bước => hòa
        InsufficentMaterial,// Không đủ tài nguyên => hòa
        ThreefoldRepetition // Chiếu 3 lần liên tiếp => hòa
    }
}
