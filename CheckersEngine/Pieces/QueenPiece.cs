using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.Pieces
{
    public class QueenPiece : Piece
    {
        public QueenPiece(Player player) : base(player)
        {
        }

        public override PieceType PieceType => PieceType.Queen;

        public override Piece TurnToQueen()
        {
            return this;
        }
    }
}
