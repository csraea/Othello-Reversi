using System;
using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours
{

    [Serializable]
    public class AIImpossible : Behaviour
    {
        public AIImpossible(GameLogic logic, Player player)
        {
            Logic = logic;
            Player = player;
        }

        public override void Sorcery(ref Cell[,] gameBoard)
        {

            List<Cell> enemies = new List<Cell>();
            for (int i = 0; i < Logic.boardSize; i++)
            {
                for (int j = 0; j < Logic.boardSize; j++)
                {
                    if (gameBoard[i, j].Type == CellTypes.Player1)
                    {
                        enemies.Add(gameBoard[i,j]);
                    }
                }
            }

            if (enemies.Count >= 6)
            {
                int idx = new Random().Next(enemies.Count - 1);
                enemies[idx].Type = CellTypes.Player2;
                gameBoard[enemies[idx].X, enemies[idx].Y].Type = CellTypes.Player2;
            }

            int bestX = -1, bestY = -1;
            MinimaxDecision(Logic, CellTypes.Player2, ref bestY, ref bestX);
            if (bestX != -1 && bestY != -1)
            {
                gameBoard[bestY, bestX].Type = CellTypes.Selected;
            }
        }

        private int Heruistic(CellTypes whoseTurn, GameLogic logic)
        {
            CellTypes opponent = CellTypes.Player2;
            if (whoseTurn == CellTypes.Player2) opponent = CellTypes.Player1;
            return Player.GetAdvancedScore(logic.GameBoard, whoseTurn, logic.boardSize) -
                   Player.GetAdvancedScore(logic.GameBoard, opponent, logic.boardSize);
        }


        // May be optimized. Deep copy is required, not shallow
        private Cell[,] GetBoardCopy(GameLogic gameLogic, Cell[,] gameBoard)
        {
            Cell[,] copy = new Cell[Logic.boardSize, Logic.boardSize];
            for (int i = 0; i < Logic.boardSize; i++)
            {
                for (int j = 0; j < Logic.boardSize; j++)
                {
                    copy[i, j] = new Cell(gameBoard[i, j].Weight, gameBoard[i, j].Type, gameBoard[i, j].X, gameBoard[i, j].Y);
                }
            }

            return copy;
        }

        private void MinimaxDecision(GameLogic logic, CellTypes whoseTurn, ref int bestY, ref int bestX)
        {
            CellTypes opponent = CellTypes.Player1;
            if (whoseTurn == CellTypes.Player1) opponent = CellTypes.Player2;
            GetPossibleMoves(logic.GameBoard, ref PossibleMoves);
            List<Cell> moves = new List<Cell>();
            foreach (var cell in PossibleMoves)
            {
                moves.Add(cell);
            }
            if (PossibleMoves.Count == 0)
            {
                bestX = -1;
                bestY = -1;
            }
            else
            {
                int bestValue = -99999;
                for (int i = 0; i < moves.Count; i++)
                {

                    GameLogic tempLogic = new GameLogic(Logic.boardSize);
                    tempLogic.GameBoard = GetBoardCopy(tempLogic, logic.GameBoard);

                    //make the move
                    tempLogic.GameBoard[moves[i].Y, moves[i].X].Type = whoseTurn;

                    int tempValue = MinimaxValue(tempLogic, 1, whoseTurn, opponent);
                    if (tempValue > bestValue)
                    {
                        bestValue = tempValue;
                        bestX = moves[i].X;
                        bestY = moves[i].Y;
                    }
                }
            }
        }

        private int MinimaxValue(GameLogic logic, int depth, CellTypes originalTurn, CellTypes currentTurn)
        {

            logic.DetermineUsableCells(currentTurn, (currentTurn == CellTypes.Player1) ? CellTypes.Player2 : CellTypes.Player1);
            if (depth == 7 || logic.IsGameWinnable(currentTurn) == -1)
            {
                return Heruistic(originalTurn, logic);
            }

            GetPossibleMoves(logic.GameBoard, ref PossibleMoves);
            CellTypes opponent = CellTypes.Player1;
            if (currentTurn == CellTypes.Player1) opponent = CellTypes.Player2;

            // If no moves are available, pass the turn to the other player
            if (PossibleMoves.Count == 0)
            {
                return MinimaxValue(logic, depth + 1, originalTurn, opponent);
            }
            else
            {
                int bestValue = -99999; // for finding max
                if (originalTurn != currentTurn) bestValue = 99999;
                // Try every single move
                for (int i = 0; i < PossibleMoves.Count; i++)
                {
                    GameLogic tempLogic = new GameLogic(logic.boardSize);
                    tempLogic.GameBoard = GetBoardCopy(tempLogic, logic.GameBoard);

                    // Make move
                    tempLogic.GameBoard[PossibleMoves[i].Y, PossibleMoves[i].X].Type = currentTurn;

                    // Exact recursive call
                    int newValue = MinimaxValue(tempLogic, depth + 1, originalTurn, currentTurn);

                    // Remember the best move
                    if (originalTurn == currentTurn)
                    {
                        // Remember Max if it's the originator's turn
                        if (newValue > bestValue) bestValue = newValue;
                    }
                    else
                    {
                        // Remember Min if it's the opponent turn
                        if (newValue < bestValue) bestValue = newValue;
                    }
                }

                return bestValue;
            }
        }
    }
}