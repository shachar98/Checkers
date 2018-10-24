using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public abstract class Piece
    {
        public Piece(Player player)
        {
            this.Player = player;
        }

        public Player Player { get; }
        public abstract PieceType PieceType { get; }
        public abstract Piece TurnToQueen();

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

    public enum Level
    {
        Easy,
        Medium,
        High
    }

    public static class PlayerExtensions
    {
        public static Player GetOtherPlayer(this Player player)
        {
            return player == Player.Black ? Player.White : Player.Black;
        }
    }
}
