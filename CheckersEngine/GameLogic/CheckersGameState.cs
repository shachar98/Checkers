﻿using CheckersEngine.GameLogic;
using GameEnginesCommon;
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
        public MovesHandler MovesHandler { get; set; }
        public List<Piece[,]> MidStates { get; set; } = new List<Piece[,]>();
        private WinningChecker m_WinningChecker;
        public int NumOfAgents => 2;
        public Piece[,] Board { get; }

        public CheckersGameState(Piece[,] board, Player currPlayer)
        {
            this.Board = board;
            Player = currPlayer;
            MovesHandler = new MovesHandler();
            m_WinningChecker = new WinningChecker(MovesHandler);
        }

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

                    double pieceScore = currPiece.PieceType == PieceType.Queen ? 1000 : 400;
                    if (currPiece.PieceType == PieceType.Regular)
                    {
                        double rowScore = Math.Min(row, Board.GetLength(0) - row);
                        double colScore = Math.Min(col, Board.GetLength(1) - col);
                        pieceScore -= rowScore * rowScore - colScore;
                    }

                    int factor = currPiece.Player == Player ? 1 : -1;
                    score += factor * pieceScore;
                }
            }

            return score;
        }

        public IEnumerable<CheckersGameState> GetNextActions(int agentIndex)
        {
            Player player = GetPlayer(agentIndex);
            return MovesHandler.GetNextActions(player, Board, this.Player);
        }

        public IGameState<CheckersGameState> GetNextState(int agentIndex, CheckersGameState action)
        {
            return action;
            // return new CheckersGameState(action, m_currPlayer);
        }

        public bool IsLost()
        {
            return m_WinningChecker.IsLost(Player, Board);
        }

        public bool IsWin()
        {
            return m_WinningChecker.IsLost(Player.GetOtherPlayer(), Board);
        }

        private Player GetPlayer(int playerIndex)
        {
            return playerIndex == 0 ? Player : Player.GetOtherPlayer();
        }

        private int GetIndex(Player player)
        {
            return player == Player ? 0 : 1;
        }

        public CheckersGameState CloneWithNewState(Piece[,] newState)
        {
            CheckersGameState newGameState = new CheckersGameState(newState, Player);
            newGameState.MidStates= new List<Piece[,]>(MidStates);
            newGameState.MidStates.Add(Board.CloneBoard());

            return newGameState;
        }
    }
}
