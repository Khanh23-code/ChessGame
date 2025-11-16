using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class StateString    
    {
        #region Properties / Constructors - Build FEN string
        private readonly StringBuilder sb = new StringBuilder();

        public StateString(Player currentPlayer, Board board)
        {
            AddPiecePlacement(board);   // Add piece placement data
            sb.Append(' ');
            AddCurrentPlayer(currentPlayer); // Add current player
            sb.Append(' ');
            AddCastlingRights(board); // Add caslting rights
            sb.Append(' ');
            AddEnPassant(board, currentPlayer); // Add enpassant data
        }

        public override string ToString()
        {
            return sb.ToString();
        }
        #endregion

        #region Add Datas

        #region Piece Datas
        private static char PieceChar(Piece piece)
        {
            char c = piece.Type switch
            {
                PieceType.King => 'k',
                PieceType.Pawn => 'p',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                _ => ' '
            };

            if (piece.Color == Player.White)
            {
                return char.ToUpper(c);
            }

            return c;
        }

        private void AddRowData(Board board, int row)
        {
            int empty = 0;

            for (int i = 0; i < 8; i++)
            {
                if (board[row, i] == null)
                {
                    empty++;
                    continue;
                }

                if (empty > 0)
                {
                    sb.Append(empty);
                    empty = 0;
                }

                sb.Append(PieceChar(board[row, i]));
            }

            if (empty > 0) sb.Append(empty);
        }

        private void AddPiecePlacement(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                if (r != 0) sb.Append('/');
                AddRowData(board, r);
            }
        }
        #endregion

        private void AddCurrentPlayer(Player player)
        {
            if (player == Player.White)
            {
                sb.Append('w');
            }
            else
            {
                sb.Append('b');
            }
        }

        private void AddCastlingRights(Board board)
        {
            bool castleWKS = board.CanCaslteKS(Player.White);
            bool castleWQS = board.CanCaslteQS(Player.White);
            bool castleBKS = board.CanCaslteKS(Player.Black);
            bool castleBQS = board.CanCaslteQS(Player.Black);

            if (!castleWKS && !castleWQS && !castleBKS && !castleBQS)
            {
                sb.Append('-');
                return;
            }

            if (castleWKS) sb.Append('K');
            if (castleWQS) sb.Append('Q');
            if (castleBKS) sb.Append('k');
            if (castleBQS) sb.Append('q');
        }

        private void AddEnPassant(Board board, Player currentPlayer)
        {
            if (!board.CanCaptureEnPassant(currentPlayer))
            {
                sb.Append('-');
                return;
            }

            Position skipPos = board.GetPawnSkipPosition(currentPlayer.Opponent());
            char file = (char)('a' + skipPos.Column);
            int rank = 8 - skipPos.Row;

            // file, rank la chi so danh dau cua co vua; voi file = 'a' -> 'h' tu trai sang phai; rank = 8 -> 1 tu tren xuong duoi

            sb.Append(file);
            sb.Append(rank);
        }
        #endregion
    }
}
