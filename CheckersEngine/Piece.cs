using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class Piece
    {
        public Piece(Player player, PieceType pieceType = PieceType.Regular)
        {
            this.Player = player;
            this.PieceType = pieceType;
        }

        public Player Player { get; }
        public PieceType PieceType { get; }

        public Piece CloneAsQueen()
        {
            return new Piece(Player, PieceType.Queen);
        }

        public override bool Equals(object obj)
        {
            Piece other = obj as Piece;
            if (other == null)
                return false;
            return this.PieceType == other.PieceType && this.Player == other.Player;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 7;
                hashCode = (hashCode * 31) + this.PieceType.GetHashCode();
                hashCode = (hashCode * 31) + this.Player.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Player}, {PieceType}";
        }
    }

    public enum PieceType
    {
        Regular,
        Queen
    }

    public enum Player
    {
        Black,
        White
    }
}
