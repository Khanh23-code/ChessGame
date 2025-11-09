namespace ChessLogic 
{
    public class Board      // Lớp dành cho bàn cờ
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        // Dictionary để lưu nước đi 2 bước của tốt cho mỗi Player => phục vụ En Passant
        private readonly Dictionary<Player, Position> pawnSkipPositions = new Dictionary<Player, Position>
        {
            {Player.White, null },
            {Player.Black, null }
        };

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

        public Position GetPawnSkipPosition(Player player)
        {
            return pawnSkipPositions[player];
            return pawnSkipPositions[player];
        }

        public void SetPawnSkipPostion(Player player, Position position)
        {
            pawnSkipPositions[player] = position;
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

            // Test for Castle
            //pieces[0, 0] = new Rook(Player.Black);
            //pieces[0, 4] = new King(Player.Black);
            //pieces[5, 7] = new Rook(Player.Black);
            //pieces[7, 0] = new Rook(Player.White);
            //pieces[6, 4] = new King(Player.White);
            //pieces[7, 7] = new Rook(Player.White);
            //pieces[0, 2] = new Knight(Player.White);

            // Test for EnPassant
            //pieces[4, 1] = new Pawn(Player.Black);
            //pieces[0, 3] = new King(Player.Black);
            //pieces[6, 2] = new Pawn(Player.White);
            //pieces[7, 4] = new King(Player.White);

            // Test for Insufficient Material
            // P1
            //pieces[1, 5] = new King(Player.Black);
            //pieces[6, 2] = new King(Player.White);
            //pieces[6, 1] = new Queen(Player.Black);
            // P2
            //pieces[7, 2] = new Bishop(Player.White);
            // P3
            //pieces[7, 2] = new Knight(Player.White);
            // P4.1
            //pieces[7, 2] = new Bishop(Player.White);
            //pieces[0, 2] = new Bishop(Player.Black);
            // P4.2
            //pieces[7, 2] = new Bishop(Player.White);
            //pieces[0, 5] = new Bishop(Player.Black);

            // Test for 50-move Rule: Chỉnh biến noCaptureOrPawnMoves trong GameState.cs thành 95

            // Test for ThreefoldCompetition Rule
            //pieces[7, 0] = new King(Player.Black);
            //pieces[4, 1] = new Rook(Player.White);
            //pieces[5, 2] = new King(Player.White);

            pieces[0, 0] = new Rook(Player.Black);
            pieces[0, 1] = new Knight(Player.Black);
            pieces[0, 2] = new Bishop(Player.Black);
            pieces[0, 3] = new Queen(Player.Black);
            pieces[0, 4] = new King(Player.Black);
            pieces[0, 5] = new Bishop(Player.Black);
            pieces[0, 6] = new Knight(Player.Black);
            pieces[0, 7] = new Rook(Player.Black);

            pieces[7, 0] = new Rook(Player.White);
            pieces[7, 1] = new Knight(Player.White);
            pieces[7, 2] = new Bishop(Player.White);
            pieces[7, 3] = new Queen(Player.White);
            pieces[7, 4] = new King(Player.White);
            pieces[7, 5] = new Bishop(Player.White);
            pieces[7, 6] = new Knight(Player.White);
            pieces[7, 7] = new Rook(Player.White);

            for (int i = 0; i < 8; i++)
            {
                pieces[1, i] = new Pawn(Player.Black);
                pieces[6, i] = new Pawn(Player.White);
            }
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

        public Counting CountPieces()
        {
            Counting counting = new Counting();

            foreach(Position pos in PiecePositions())
            {
                Piece piece = pieces[pos.Row, pos.Column];

                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        } 

        public bool IsInsufficientMaterial()
        {
            Counting counting = CountPieces();

            // Cac dieu kien de insufficient material: 4 case: 
            return IsKingVKing(counting) || IsKingBishopVKing(counting) || IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
        }

        private static bool IsKingVKing(Counting counting)
        {
            return counting.TotalCount == 2;
        }

        private static bool IsKingBishopVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.WhiteCount(PieceType.Bishop) == 1 || counting.BlackCount(PieceType.Bishop) == 1);
        }

        private static bool IsKingKnightVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.WhiteCount(PieceType.Knight) == 1 || counting.BlackCount(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVKingBishop(Counting counting)
        {
            if (counting.TotalCount != 4) return false;

            if (counting.WhiteCount(PieceType.Bishop) != 1 || counting.BlackCount(PieceType.Bishop) != 1) return false;

            Position wBishopPos = FindPiece(Player.White, PieceType.Bishop);
            Position bBishopPos = FindPiece(Player.Black, PieceType.Bishop);

            // 2 tượng khác màu vẫn được xem là Sufficient => Phải cùng màu 
            return wBishopPos.SquareColor() == bBishopPos.SquareColor();
        }

        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => pieces[pos.Row, pos.Column].Type == type);
        }

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)       // Kiểm tra vua và xe đã di chuyển chưa => (đơn giản hóa) truyền vào vị trí ban đầu của xe và vua
        {
            if (IsEmpty(kingPos) || IsEmpty(rookPos)) return false;

            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            return king.Type == PieceType.King && rook.Type == PieceType.Rook && !king.HasMoved && !rook.HasMoved;
        }

        public bool CanCaslteKS(Player player)      // Kiem tra nhap thanh canh vua (ben phai)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }

        public bool CanCaslteQS(Player player)      // Kiem tra nhap thanh canh hau (ben trai)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions)
            {
                if (!IsInside(pos)) continue;

                Piece piece = this[pos];
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn) continue;
                
                EnPassant move = new EnPassant(pos, skipPos);
                if (move.IsLegal(this))
                {
                    return true;        // Chi can co 1 vi tri hop le
                }
            }

            return false;
        }

        public bool CanCaptureEnPassant(Player player)
        //Hàm dùng để kiểm tra xem phía player có thể thực hiện EnPassant hay ko?
        //Ví dụ hàm này gọi cho Player.White
        //B1: Lấy ra skipPos của đối thủ. VD lấy ra skipPos của Player.Black (nếu skipPos này khác null tức bên Black vừa thực hiện DoublePawn)
        //B2: Từ skipPos sẽ suy ra được 2 vị trí có thể thực hiện EnPassant. Ví dụ skipPos là của Black, v thì 2 ô dưới bên trái/phải của skipPos là vị trí có thể EnPassant
        //B3: Kiểm tra 2 vị trí đó: Có chứa quân tốt của player ko? Có thể thực hiện EnPassant ko?
        {
            Position skipPos = GetPawnSkipPosition(player.Opponent());

            if (skipPos == null)        // Doi thu vua roi khong di chuyen DoublePawn => khong co skipPos
            {
                return false;
            }

            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { skipPos + Direction.SouthEast, skipPos + Direction.SouthWest },
                Player.Black => new Position[] { skipPos + Direction.NorthEast, skipPos + Direction.NorthWest },
                _ => Array.Empty<Position>()
            };

            return HasPawnPosition(player, pawnPositions, skipPos);
        }
    }
}
