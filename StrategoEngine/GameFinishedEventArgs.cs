using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoEngine
{
    public class GameFinishedEventArgs : EventArgs
    {
        public GameFinishedEventArgs(Player winner)
        {
            Winner = winner;
        }

        public Player Winner { get; }
    }
}
