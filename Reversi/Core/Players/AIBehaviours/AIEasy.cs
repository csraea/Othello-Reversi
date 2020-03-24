using System;
using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {
    public class AIEasy : Behaviour {
        private readonly GameLogic _gameLogic;
        private List<Cell> _possibleMoves = new List<Cell>();
        
        public AIEasy(GameLogic gameLogic) {
            _gameLogic = gameLogic;
        }

        public override void Sorcery(ref Cell[,] gameBoard) {
            _gameLogic.DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
            GetPossibleMoves(gameBoard, ref _possibleMoves);
            Cell selected = GetRandomCell();
            gameBoard[selected.Y, selected.X].Type = CellTypes.Selected;
            _possibleMoves.Clear();
        }

        private Cell GetRandomCell() {
            return _possibleMoves[new Random().Next(_possibleMoves.Count-1)];   // Here the index is out of range
        }
    }
}