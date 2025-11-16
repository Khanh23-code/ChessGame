namespace ChessLogic
{
    public enum Player
    {
        None,
        White,
        Black
    }
    // Nếu mode skin thì đổi màu

    public static class PlayerExtensions
    {
        public static Player Opponent(this Player player)
        {
            return player switch
            {
                Player.White => Player.Black,
                Player.Black => Player.White,
                _ => Player.None,   // default
            };
        }
    }
}
