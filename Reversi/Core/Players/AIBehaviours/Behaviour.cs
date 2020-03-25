using System.Collections.Generic;

namespace Reversi.Core.Players.AIBehaviours {
    public abstract class Behaviour {

        public static List<Cell> PossibleMoves = PossibleMoves ?? new List<Cell>();
        public enum Mode {
            Easy,
            Medium,
            Difficult
        }

        public abstract void Sorcery(ref Cell[,] gameBoard);
        
        public static void GetPossibleMoves(Cell[,] gameBoard, ref List<Cell> possibleMoves) {
            foreach (Cell cell in gameBoard) {
                if (cell.Type == CellTypes.Usable) {
                    possibleMoves.Add(cell);
                }
            }
        }
        
    }
}