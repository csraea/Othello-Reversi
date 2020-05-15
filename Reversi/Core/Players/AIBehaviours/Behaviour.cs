using System;
using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {

    [Serializable]
    public abstract class Behaviour {
        public GameLogic Logic { get; set; }
        public Player Player { get; set; }

        protected static List<Cell> PossibleMoves = PossibleMoves ?? new List<Cell>();
        public enum Mode {
            Easy,
            Medium,
            Difficult,
            Impossible
        }

        public abstract void Sorcery(ref Cell[,] gameBoard);
        
        public static void GetPossibleMoves(Cell[,] gameBoard, ref List<Cell> possibleMoves) {
            possibleMoves.Clear();
            foreach (Cell cell in gameBoard) {
                if (cell.Type == CellTypes.Usable) {
                    possibleMoves.Add(cell);
                }
            }
        }
        
    }
}