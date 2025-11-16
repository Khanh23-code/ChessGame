namespace ChessLogic
{
    public class Position     // Các đối tượng thuộc lớp là các ô ứng với từng vị trí trên bàn cờ
    {
        #region Properties / Constructors
        public int Row { get; }
        public int Column { get; }

        public Position(int row, int column)
        {
            Row = row; 
            Column = column;
        }
        #endregion

        #region Set Color
        public Player SquareColor()     // Sử dụng Player để lấy màu sắc (vì cũng có trắng/đen) thay vì tạo thêm enum cho màu sắc bàn cờ
        {
            if ((Row + Column) % 2 == 0)
            {
                return Player.White;
            }
            return Player.Black;
        }
        #endregion

        #region Operators / Equal Functions
        // 4 phương thức (Equals, GetHashCode, operator ==, operator !=) được VS tạo tự động (phím tắt Ctrl + '.' >> "Generate Equals and GetHashCode...")
        // dùng để hộ trợ so sánh bằng

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Column == position.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        // Toán tử để di chuyển
        public static Position operator +(Position start, Direction dir)
        {
            return new Position(start.Row + dir.RowDelta, start.Column + dir.ColumnDelta);
        }
        #endregion
    }
}
