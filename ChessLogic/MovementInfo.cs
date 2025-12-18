using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class MovementInfo
    {
        public List<string> whiteMoves = new List<string>();
        public List<string> blackMoves = new List<string>();

        public string SetData(Move move, Board board, Player current)
        {
            string data = "";

            if (move.Type == MoveType.CaslteKS) data = "O-O";
            else if (move.Type == MoveType.CaslteQS) data = "O-O-O";
            else if (move.Type == MoveType.PawnPromotion)
            {
                PawnPromotion promotion = (PawnPromotion)move;
                data += ToFile(move.ToPos) + ToRank(move.ToPos) + "=" + PieceChar(promotion.newType, Player.White);
            }
            else
            {
                Piece piece = board[move.FromPos];

                if (piece.Type != PieceType.Pawn)
                {
                    data += PieceChar(piece.Type, Player.White);
                }

                if (!board.IsEmpty(move.ToPos) && board[move.ToPos].Color != current)
                {
                    if (piece.Type == PieceType.Pawn) data += ToFile(move.FromPos);
                    data += "x";
                }

                data += ToFile(move.ToPos) + ToRank(move.ToPos);

                move.Execute(board);
                if (board.IsInCheck(current.Opponent()))
                {
                    if (board.PiecePositionsFor(current.Opponent()).SelectMany(pos => board.LegalMovesForPiece(pos)).Any())
                    {
                        data += "+";
                    }
                    else
                    {
                        data += "#";
                    }
                }
                
            }

            if (current == Player.White)
            {
                whiteMoves.Add(data);
            }
            else
            {
                blackMoves.Add(data);
            }

            return data;
        }

        private char PieceChar(PieceType type, Player color)
        {
            char c = type switch
            {
                PieceType.King => 'k',
                PieceType.Pawn => 'p',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
            };

            if (color == Player.White)
            {
                return char.ToUpper(c);
            }

            return c;
        }

        private string ToFile(Position pos)
        {
            char file = (char)('a' + pos.Column);
            return file + "";
        }

        private string ToRank(Position pos)
        {
            int rank = 8 - pos.Row;
            return rank + "";
        }
    }
}
