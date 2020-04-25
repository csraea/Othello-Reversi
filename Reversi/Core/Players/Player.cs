using System;

namespace Reversi {
    public abstract class Player {
        public String Name { get; set; }
        public ConsoleColor _color { get; set; }

        protected Player(String name) {
        }

        protected Player() {
            
        }

        public abstract bool MakeTurn(Cell[,] gameBoard);
        
        public int GetScore(Cell[,] gameBoard, CellTypes cellType, byte boardSize) {
            int tempScore = 0;
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (gameBoard[i, j].Type == cellType) {
                        tempScore++;
                    }
                }
            }

            return tempScore;
        }
        
        public bool CheckAndPlace(int Y, int X, CellTypes type, ref Cell[,] gameBoard) {
            if (gameBoard[Y, X].Type != CellTypes.Usable) return false;
            gameBoard[Y, X].Type = type;
            
            return true;
        }
        
    }
}