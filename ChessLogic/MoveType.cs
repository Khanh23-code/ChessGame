namespace ChessLogic
{
    public enum MoveType
    {
        Normal,
        // Special moves
        CaslteQS,       // Caslte queen side: Nhập thành cánh hậu
        CaslteKS,       // Nhập thành cánh vua
        DoublePawn,     // Di chuyển 2 ô đầu của tốt
        EnPassant,      // Bắt tốt qua đường
        PawnPromotion   // Phong tước cho tốt
    }
}
