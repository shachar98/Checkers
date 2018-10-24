using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.Pieces
{
    public class RegularPiece : Piece
    {
        public RegularPiece(Player player) : base(player)
        {
        }

        public override PieceType PieceType => PieceType.Regular;

        public override Piece TurnToQueen()
        {
            return new QueenPiece(Player);
        }
    }
}
