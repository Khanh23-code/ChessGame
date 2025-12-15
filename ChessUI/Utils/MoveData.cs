using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public class MoveData
    {
        public int Move { get; set; }
        public string White { get; set; }
        public string Black { get; set; }  

        public MoveData(int move, string white, string black)
        {
            Move = move;
            White = white;
            Black = black;
        }
    }
}
