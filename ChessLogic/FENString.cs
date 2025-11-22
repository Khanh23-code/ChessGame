using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// FENString format:
// Piece placement: row1/row2/.../row8
// Current player: 'w' or 'b'
// Castling rights: KQkq or '-'
// En passant target square
// Halfmove clock: number of moves since last capture or pawn move
// Fullmove number: starts at 1, increments
// Timer: "WhiteTime:BlackTime" (in seconds)

namespace ChessLogic
{
    public class FENString
    {
        #region Properties / Constructors
        private readonly StringBuilder sb = new StringBuilder();

        public FENString(GameState gameState)
        {
            AddPiecePlacement(gameState.Board);                     // Add piece placement data
            sb.Append(' ');
            AddCurrentPlayer(gameState.CurrentPlayer);              // Add current player
            sb.Append(' ');
            AddCastlingRights(gameState.Board);                     // Add caslting rights
            sb.Append(' ');
            AddEnPassant(gameState.Board, gameState.CurrentPlayer); // Add enpassant data - Vị trí 
            sb.Append(' ');            
            AddHalfMoveClock(gameState);                            // Add noCaptureOrPawnMoves data
            sb.Append(' ');
            AddFullMoveNumber(gameState);                           // Add fullmove number
            sb.Append(' ');
            AddTimer(gameState);                                    // Add timer data - Format: WhiteTime:BlackTime (in seconds)
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
            Position skipPos = board.GetPawnSkipPosition(currentPlayer.Opponent());

            if (skipPos == null)
            {
                sb.Append('-');
                return;
            }

            char file = (char)('a' + skipPos.Column);
            int rank = 8 - skipPos.Row;

            // file, rank la chi so danh dau cua co vua; voi file = 'a' -> 'h' tu trai sang phai; rank = 8 -> 1 tu tren xuong duoi

            sb.Append(file);
            sb.Append(rank);
        }

        private void AddHalfMoveClock(GameState gameState)
        {
            sb.Append(gameState.NoCaptureOrPawnMoves);
        }

        private void AddFullMoveNumber(GameState gameState)
        {
            sb.Append(gameState.FullMoveNumber);
        }

        private string TimerToString(TimeSpan timeSpan)
        {
            return ((int)timeSpan.TotalSeconds).ToString();
        }

        private string AddTimer(GameState gameState)
        {
            string timerString = TimerToString(gameState.WhiteTime) + ":" + TimerToString(gameState.BlackTime);
            sb.Append(timerString);
            return timerString;
        }
        #endregion

    }
}
