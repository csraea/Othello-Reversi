namespace Reversi {
    public interface Player {
        int[] MakeTurn();
        
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
    }
}