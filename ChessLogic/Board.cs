namespace ChessLogic // demo
{
    public class Board      // Lớp dành cho bàn cờ
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        public Piece this[int row, int col]     // Indexer
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        public Piece this[Position position]
        {
            get { return pieces[position.Row, position.Column]; }
            set { pieces[position.Row, position.Column] = value; }
        }

        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            // Hàm AddStartPieces() hiện tại đang tạo cố định bàn cờ với quân đen ở trên và quân trắng ở dưới
            // => Có khả năng mở rộng 
            return board;
        }

        private void AddStartPieces()       
        {
            //Test for Checkmate
            //pieces[0, 4] = new King(Player.Black);
            //pieces[2, 5] = new Queen(Player.White);
            //pieces[5, 5] = new Bishop(Player.White);
            //pieces[7, 6] = new King(Player.White);

            // Test for Stalemate
            //pieces[4, 4] = new King(Player.White);
            //pieces[5, 3] = new Queen(Player.White);
            //pieces[7, 4] = new King(Player.Black);

            // Test for PromotionMenu/PawnPromotion
            //pieces[1, 1] = new Pawn(Player.White);
            //pieces[6, 6] = new Pawn(Player.Black);

            // Test for Castle - P1
            pieces[0, 0] = new Rook(Player.Black);
            pieces[0, 4] = new King(Player.Black);
            pieces[5, 7] = new Rook(Player.Black);
            pieces[7, 0] = new Rook(Player.White);
            pieces[6, 4] = new King(Player.White);
            pieces[7, 7] = new Rook(Player.White);


            //pieces[0, 0] = new Rook(Player.Black);
            //pieces[0, 1] = new Knight(Player.Black);
            //pieces[0, 2] = new Bishop(Player.Black);
            //pieces[0, 3] = new Queen(Player.Black);
            //pieces[0, 4] = new King(Player.Black);
            //pieces[0, 5] = new Bishop(Player.Black);
            //pieces[0, 6] = new Knight(Player.Black);
            //pieces[0, 7] = new Rook(Player.Black);

            //pieces[7, 0] = new Rook(Player.White);
            //pieces[7, 1] = new Knight(Player.White);
            //pieces[7, 2] = new Bishop(Player.White);
            //pieces[7, 3] = new Queen(Player.White);
            //pieces[7, 4] = new King(Player.White);
            //pieces[7, 5] = new Bishop(Player.White);
            //pieces[7, 6] = new Knight(Player.White);
            //pieces[7, 7] = new Rook(Player.White);

            //for (int i = 0; i < 8; i++)
            //{
            //    pieces[1, i] = new Pawn(Player.Black);
            //    pieces[6, i] = new Pawn(Player.White);
            //}
        }

        public bool IsInside(Position pos)
        {
            return (0 <= pos.Row && pos.Row <= 7) && (0 <= pos.Column && pos.Column <= 7);
        }

        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> PiecePositions()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] != null) yield return new Position(i, j);
                }
            } 
        }

        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Board Copy()
        {
            Board newBoard = new Board();

            foreach(Position pos in PiecePositions())
            {
                newBoard[pos] = this[pos].Copy();
            }
            
            return newBoard;   
        }
    }
}
