﻿using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    // TODO:
    // * Add win-loss
    // * Improve window UI
    // * Improve preformence & game play
    // * Add human player
    // * Add train against impself

    public class CheckersGameState : IGameState<CheckersGameState>
    {
        public Player Player { get; }
        public List<Piece[,]> MidStates { get; set; } = new List<Piece[,]>();
        private IMovesHandler m_MovesHandler;
        public int NumOfAgents => 2;
        public Piece[,] Board { get; }

        public CheckersGameState(Piece[,] board, Player currPlayer)
        {
            this.Board = board;
            Player = currPlayer;
            m_MovesHandler = MovesHandler.Instance;
        }

        /// <summary>
        /// Estimate the score of the board
        /// </summary>
        /// <returns>The state estimated score</returns>
        public double CalcScore()
        {
            double score = 0;
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var currPiece = Board[row, col];
                    if (currPiece == null)
                        continue;

                    // Queen's score is heigher the regular piece
                    double pieceScore = currPiece.PieceType == PieceType.Queen ? 1000 : 400;

                    // Penalty for been in the middle of the board, because this is worst place in general
                    if (currPiece.PieceType == PieceType.Regular)
                    {
                        double rowScore = Math.Min(row, Board.GetLength(0) - row);
                        double colScore = Math.Min(col, Board.GetLength(1) - col);
                        pieceScore -= rowScore * rowScore - colScore;
                    }

                    // Oponent piece' score is exactly negate player piece' score
                    int factor = currPiece.Player == Player ? 1 : -1;
                    score += factor * pieceScore;
                }
            }

            return score;
        }

        public IEnumerable<CheckersGameState> GetNextActions(int agentIndex)
        {
            Player player = GetPlayer(agentIndex);
            return m_MovesHandler.GetNextActions(player, this);
        }

        public IGameState<CheckersGameState> GetNextState(int agentIndex, CheckersGameState action)
        {
            return action;
        }

        public bool IsLost()
        {
            return !m_MovesHandler.HaveActions(Player, new CheckersGameState(Board, Player));
        }

        public bool IsWin()
        {
            return !m_MovesHandler.HaveActions(Player.GetOtherPlayer(), new CheckersGameState(Board, Player.GetOtherPlayer()));

        }

        private Player GetPlayer(int playerIndex)
        {
            return playerIndex == 0 ? Player : Player.GetOtherPlayer();
        }

        private int GetIndex(Player player)
        {
            return player == Player ? 0 : 1;
        }

        /// <summary>
        /// Clone the board and adding the board given as the new state board
        /// </summary>
        /// <param name="newBoard">The bew Board</param>
        /// <returns>The new State</returns>
        public CheckersGameState CloneWithNewState(Piece[,] newBoard)
        {
            CheckersGameState newGameState = new CheckersGameState(newBoard, Player);
            newGameState.MidStates = new List<Piece[,]>(MidStates);
            newGameState.MidStates.Add(Board.CloneBoard());

            return newGameState;
        }
    }
}
