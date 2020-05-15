using System;
using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {

    [Serializable]
    public class AIEasy : Behaviour {

        public AIEasy(GameLogic logic, Player player) {
            Logic = logic;
            Player = player;
        }
        
        public override void Sorcery(ref Cell[,] gameBoard) {
            Logic.DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
            GetPossibleMoves(gameBoard, ref PossibleMoves);
            Cell selected = GetRandomCell();
            if(selected != null) gameBoard[selected.Y, selected.X].Type = CellTypes.Selected;
            PossibleMoves.Clear();
        }

        private Cell GetRandomCell() {
            if(PossibleMoves.Count != 0) return PossibleMoves[new Random().Next(PossibleMoves.Count-1)];
            return null;
        }
    }
}