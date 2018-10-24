using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoEngine
{
    public class GameCell
    {
        public bool IsWater { get; set; }
        public bool IsEmpty { get; set; }
        public Piece Piece { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GameCell other)
            {
                if ((Piece == null && other.Piece != null) ||
                    (Piece != null && other.Piece == null) ||
                    IsWater != other.IsWater || 
                    IsEmpty != other.IsEmpty)
                    return false;

                return Piece == null || Piece.Equals(other.Piece);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            hash += 31 * IsWater.GetHashCode();
            hash += 31 * IsEmpty.GetHashCode();
            hash += 31 * Piece.GetHashCode();
            return hash;
        }

        public GameCell Clone()
        {
            return new GameCell()
            {
                IsEmpty = IsEmpty,
                IsWater = IsWater,
                Piece = Piece?.Clone()
            };
        }

        public override string ToString()
        {
            return $"Piece: {Piece}, IsEmpty: {IsEmpty}, IsWater: {IsWater}";
        }
    }

    public class Piece
    {
        public PieceType PieceType { get; set; }
        public Player PlayerOwner { get; set; }
        public bool DidExposed { get; set; }
        // Maybe change to biggest move, in order to recognize 2
        public bool DidMove { get; set; }
        public bool IsMoveable { get; set; }

        public Piece Clone()
        {
            return new Piece()
            {
                PieceType = PieceType,
                PlayerOwner = PlayerOwner,
                DidExposed = DidExposed,
                DidMove = DidMove,
                IsMoveable = IsMoveable
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is Piece other)
            {
                return DidMove == other.DidMove && DidExposed == other.DidExposed &&
                    IsMoveable == other.IsMoveable && PieceType == other.PieceType &&
                    PlayerOwner == other.PlayerOwner;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            hash += 31 * PlayerOwner.GetHashCode();
            hash += 31 * IsMoveable.GetHashCode();
            hash += 31 * PieceType.GetHashCode();
            hash += 31 * DidExposed.GetHashCode();
            hash += 31 * DidMove.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"PlayerOwner: {PlayerOwner}, PieceType: {PieceType}";
        }
    }

    public enum PieceType
    {
        Unknown,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Bomb,
        Flag
    }

    public enum Player
    {
        White,
        Black
    }

    public static class PlayerExtensions
    {
        public static Player GetOtherPlayer(this Player player)
        {
            if (player == Player.Black)
                return Player.White;

            return Player.Black;
        }
    }
}
