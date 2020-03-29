using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi;

namespace ReversiTest.Game {
    
    [TestClass]
    public class TLogic {

        private GameLogic Logic(byte boardSize) {
            var logic = new GameLogic(boardSize);
            return logic;
        }

        [TestMethod]
        public void EmptyBoard() {
            var logic = Logic(8);

            for (int i = 0; i < logic.boardSize; i++) {
                for (int j = 0; j < logic.boardSize; j++) {
                    Assert.IsNull(logic.GameBoard[i,j]);
                }
            }
        }


        [TestMethod]
        public void FilledBoard() {
            var logic = Logic(8);
            logic.FillBoard(CellTypes.Free);

            for (int i = 0; i < logic.boardSize; i++) {
                for (int j = 0; j < logic.boardSize; j++) {
                    Assert.IsNotNull(logic.GameBoard[i,j]);
                    Assert.AreEqual(logic.GameBoard[i, j].Type, CellTypes.Free);
                    Assert.AreEqual(logic.GameBoard[i,j].X, j);
                    Assert.AreEqual(logic.GameBoard[i,j].Y, i);
                }
            }
        }
        
        
        
    }
}