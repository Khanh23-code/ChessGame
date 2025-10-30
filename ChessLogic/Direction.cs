namespace ChessLogic
{
    public class Direction      // lớp biểu diễn hướng di chuyển
    {
        // Các direction để sử dụng, mọi đối tượng của lớp Direction chỉ có thể là 1 trong 8 hướng này
        public readonly static Direction North = new(-1, 0);
        public readonly static Direction South = new(1, 0);
        public readonly static Direction West = new(0, -1);
        public readonly static Direction East = new(0, 1);
        public readonly static Direction NorthEast = North + East;
        public readonly static Direction SouthEast = South + East;
        public readonly static Direction SouthWest = South + West;
        public readonly static Direction NorthWest = North + West;

        public int RowDelta { get; }
        public int ColumnDelta { get; }

        public Direction(int rowDelta, int columnDelta)
        {
            RowDelta = rowDelta;
            ColumnDelta = columnDelta;
        }

        // Toán tử để kết hợp các Directions
        public static Direction operator +(Direction d1, Direction d2)
        {
            return new Direction(d1.RowDelta + d2.RowDelta, d1.ColumnDelta + d2.ColumnDelta);
        }

        public static Direction operator *(int scalar, Direction d)
        {
            return new Direction(d.RowDelta * scalar, d.ColumnDelta * scalar);
        }
    }
}
