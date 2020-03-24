using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {
    public abstract class Behaviour {
        
        public enum Mode {
            Easy,
            Medium,
            Difficult
        }

        public abstract void Sorcery(ref Cell[,] gameBoard);
        
        public void GetPossibleMoves(Cell[,] gameBoard, ref List<Cell> possibleMoves) {
            foreach (Cell cell in gameBoard) {
                if (cell.Type == CellTypes.Usable) {
                    possibleMoves.Add(cell);
                }
            }
        }
        
        
    }
}