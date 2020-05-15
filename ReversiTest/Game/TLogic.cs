using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi;
using Reversi.Core.Players;
using Reversi.Core.Players.AIBehaviours;

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

        [TestMethod]
        public void IsGameWinnableAtBeginning() {
            var logic = Logic(8);
            logic.FillBoard(CellTypes.Free);
            logic.LocatePlayers();
            
            Assert.AreEqual(1, logic.IsGameWinnable(CellTypes.Player1));
            Assert.AreEqual(1, logic.IsGameWinnable(CellTypes.Player2));
        }

        [TestMethod]
        public void MagicBasicTest() {
            var logic = Logic(8);
            logic.FillBoard(CellTypes.Free);
            logic.LocatePlayers();
            
            logic.secondPlayer = new AIPlayer(Behaviour.Mode.Easy, logic, ConsoleColor.Red);
            logic.DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
            logic.secondPlayer.MakeTurn(logic.GameBoard);
            logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
            logic.Magic(CellTypes.Player1, CellTypes.Player2);

            Assert.AreEqual(3, logic.secondPlayer.GetScore(logic.GameBoard, CellTypes.Player2, logic.boardSize));
        }

        [TestMethod]
        public void EndGameTest() {
            var logic = Logic(6);
            logic.FillBoard(CellTypes.Free);
            logic.LocatePlayers();
            
            logic.humanPlayer = new AIPlayer(Behaviour.Mode.Easy, logic, ConsoleColor.Blue);
            logic.secondPlayer = new AIPlayer(Behaviour.Mode.Easy, logic, ConsoleColor.Red);

            Player player = logic.humanPlayer;
            ;
            do {
                player = (player == logic.humanPlayer) ? logic.secondPlayer : logic.humanPlayer;

                if (!logic.skippedTurn) {
                    if (player == logic.humanPlayer) logic.DetermineUsableCells(CellTypes.Player1, CellTypes.Player2);
                    else logic.DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
                }
                logic.skippedTurn = false;

                sbyte winnable = logic.IsGameWinnable(player == logic.humanPlayer ? CellTypes.Player1 : CellTypes.Player2);
                if(winnable == -1) break;
                if(winnable == 0) goto NEXTPLAYER;
                if (!player.MakeTurn(logic.GameBoard)) {
                    break;
                }

                logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
                
                if(player == logic.humanPlayer) logic.Magic(CellTypes.Player2, CellTypes.Player1); 
                else logic.Magic(CellTypes.Player1, CellTypes.Player2);

                NEXTPLAYER: ;
                
            } while (true);
            
            Assert.AreEqual(0, player.GetScore(logic.GameBoard,CellTypes.Free, logic.boardSize));
        }

    }
}