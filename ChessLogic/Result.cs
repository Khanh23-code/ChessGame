using ChessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Result
    {
        public Player Winner { get; }
        public EndReason Reason { get; }

        public Result(Player Winner, EndReason Reason)
        {
            this.Winner = Winner;
            this.Reason = Reason;
        }

        public static Result Win(Player winner, EndReason reason = EndReason.Checkmate)
        {
            return new Result(winner, reason);
        }

        public static Result Draw(EndReason reason)
        {
            return new Result(Player.None, reason);
        }
    }
}
