using System;
using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {
    public class AIEasy : Behaviour {
        private readonly GameLogic _gameLogic;

        public AIEasy(GameLogic gameLogic) {
            _gameLogic = gameLogic;
        }

        public override void Sorcery(ref Cell[,] gameBoard) {
            _gameLogic.DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
            GetPossibleMoves(gameBoard, ref PossibleMoves);
            Cell selected = GetRandomCell();
            gameBoard[selected.Y, selected.X].Type = CellTypes.Selected;
            PossibleMoves.Clear();
        }

        private Cell GetRandomCell() {
            return PossibleMoves[new Random().Next(PossibleMoves.Count-1)];   // Here the index is out of range
        }
    }
}