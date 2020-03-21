using System;
using System.Collections.Generic;
using System.Threading;

namespace Reversi {
    public class GameLogic {
        private byte boardSize;
        private Cell[,] gameBoard;
        private readonly byte _gameMode;
        private bool winnable;
        Player humanPlayer;
        private Player secondPlayer;
        private int state;
        private UI ui;

        public GameLogic(byte boardSize, byte gameMode, UI ui) {
            this.boardSize = boardSize;
            this.ui = ui;
            gameBoard = new Cell[boardSize, boardSize];
            _gameMode = gameMode;
            winnable = true;
            humanPlayer = new HumanPlayer();
            secondPlayer = (gameMode == 1) ? (Player) new AIPlayer() : new HumanPlayer();
            state = 2;
            FillBoard(CellTypes.Free);
            LocatePlayers();
        }

        private void FillBoard(CellTypes cellType) {
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    gameBoard[i, j] = new Cell(1, cellType, j, i);
                }
            }
        }

        private void LocatePlayers() {
            gameBoard[boardSize / 2, boardSize / 2 - 1].Type = CellTypes.Player2;
            gameBoard[boardSize / 2 - 1, boardSize / 2].Type = CellTypes.Player2;
            gameBoard[boardSize / 2, boardSize / 2].Type = CellTypes.Player1;
            gameBoard[boardSize / 2 - 1, boardSize / 2 -1].Type = CellTypes.Player1;
            
            // gameBoard[5, 2].Type = CellTypes.Player2;
            // gameBoard[6, 2].Type = CellTypes.Player2;
            // gameBoard[7,2].Type = CellTypes.Player2;
            // gameBoard[8,2].Type = CellTypes.Player2;
            // gameBoard[9,2].Type = CellTypes.Player2;
            //
            // gameBoard[6,3].Type = CellTypes.Player2;
            // gameBoard[8,3].Type = CellTypes.Player2;
            //     
            // gameBoard[6,4].Type = CellTypes.Player1;
            // gameBoard[7,4].Type = CellTypes.Player2;
            // gameBoard[8,4].Type = CellTypes.Player1;
            //
            // gameBoard[5,5].Type = CellTypes.Player1;
            // gameBoard[6,5].Type = CellTypes.Player1;
            // gameBoard[7,5].Type = CellTypes.Player1;
            //
            // gameBoard[4,6].Type = CellTypes.Player1;
            // gameBoard[5,6].Type = CellTypes.Player1;
            // gameBoard[6,6].Type = CellTypes.Player1;
            // gameBoard[6,7].Type = CellTypes.Player2;
        }

        public sbyte StartGame() {
            bool exit = false;
            while (IsGameWinnable() || state  > 0) {
                DetermineUsableCells(CellTypes.Player1, CellTypes.Player2);
                ui.DisplayGame(humanPlayer, secondPlayer, gameBoard, boardSize);

                int[] coords;
                do {
                    coords = humanPlayer.MakeTurn();
                } while (!CheckAndPlace(coords, CellTypes.Selected, ref exit) && !exit);
                if (exit) break;
                ChangeCellType(CellTypes.Usable, CellTypes.Free);
                Magic(CellTypes.Player2, CellTypes.Player1);
                
                if (_gameMode == 2) {
                    DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
                    ui.DisplayGame(humanPlayer, secondPlayer, gameBoard, boardSize);
                    
                    do {
                        coords = secondPlayer.MakeTurn();
                    } while (!CheckAndPlace(coords, CellTypes.Selected, ref exit) && !exit);
                    if(exit) break;
                    ChangeCellType(CellTypes.Usable, CellTypes.Free);
                    Magic(CellTypes.Player1, CellTypes.Player2);
                }
            }
            
            return 0;
        }

        private bool CheckAndPlace(int[] coords, CellTypes type, ref bool exit) {
            if (coords[0] == -1) {
                ui.Exit();
                exit = true;
                return false;
            }

            if (gameBoard[coords[0], coords[1]].Type != CellTypes.Usable) return false;
            gameBoard[coords[0], coords[1]].Type = type;
            return true;
        }

        private void DetermineUsableCells(CellTypes target, CellTypes with) {
            FindNeighbouring(target, with);
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (gameBoard[i, j].Type == CellTypes.Neighbouring) {
                        InspectCellByLine(gameBoard[i,j], with, 8, 1);
                    }
                }
            }
            ChangeCellType(CellTypes.Neighbouring, target);
        }
        
        private void InspectCellByLine(Cell currentCell, CellTypes type, int depth, int recursionFlag) {
            int x = 0, y = 0, limit1 = 0, limit2 = 0; // values used in recursion

            int initialX = 0, initialY = 0; //save the initial cell coords for which all the directions are being tested
            if (recursionFlag == 1) {
                initialX = currentCell.X;
                initialY = currentCell.Y;
            }
            
            ParseDirection(ref x, ref y, ref limit1, ref limit2, depth); //update variables
            
            //exact recursion
            if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == type) {
                InspectCellByLine(gameBoard[currentCell.Y + y, currentCell.X + x], type, depth, 0);
            } else if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Free && currentCell.Type == type) {
                gameBoard[currentCell.Y + y, currentCell.X + x].Type = CellTypes.Usable;
            }
            
            //change the direction
            while (recursionFlag == 1 && depth > 1) {
                InspectCellByLine(gameBoard[initialY, initialX], type, --depth, 0);
            }
        }

        private void ParseDirection(ref int x, ref int y, ref int limit1, ref int limit2, int depth) {
            switch (depth) {
                case 8: limit1 = -1; limit2 = 0; x = 0; y = -1; break;
                case 7: limit1 = -1; limit2 = boardSize - 1; x = 0; y = 1; break;
                case 6: limit1 = 0; limit2 = -1; x = -1; y = 0; break;
                case 5: limit1 = boardSize - 1; limit2 = -1; x = 1; y = 0; break;
                case 4: limit1 = 0; limit2 = 0; x = -1; y = -1; break;
                case 3: limit1 = boardSize - 1; limit2 = boardSize - 1; x = 1; y = 1; break;
                case 2: limit1 = boardSize - 1; limit2 = 0; x = 1; y = -1; break;
                case 1: limit1 = 0; limit2 = boardSize - 1; x = -1; y = 1; break;
                default: return;
            }
        }


        private void Magic(CellTypes type, CellTypes playerCell) {
            int substitution = 0;
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (gameBoard[i, j].Type == CellTypes.Selected) {
                        CompleteLine(gameBoard[i,j], type, playerCell, 8, 1,  ref substitution);
                        ChangeCellType(CellTypes.Selected, playerCell);
                        return;
                    }
                }
            }
        }
        
        private void CompleteLine(Cell currentCell, CellTypes type, CellTypes playerCell, int depth, int recursionFlag, ref int substitution) {
            //Console.WriteLine(currentCell.Y + " " + currentCell.X);
            int x = 0, y = 0, limit1 = 0, limit2 = 0; // values used in recursion
            
            int initialX = 0, initialY = 0; //save the initial cell coords for which all the directions are being tested
            if (recursionFlag == 1) {
                initialX = currentCell.X;
                initialY = currentCell.Y;
            }
            
            ParseDirection(ref x, ref y, ref limit1, ref limit2, depth); //update variables
            
            //exact recursion
            if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == type) {
                CompleteLine(gameBoard[currentCell.Y + y, currentCell.X + x], type, playerCell, depth, 0, ref substitution);
                if (substitution == 1 && currentCell.Type != CellTypes.Selected) currentCell.Type = playerCell;
                if (currentCell.X == initialX && currentCell.Y == initialY) substitution = 0;
            } else if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == playerCell && currentCell.Type == type) {
                gameBoard[currentCell.Y, currentCell.X].Type = playerCell;
                substitution = 1;
            }
            
            //change the direction
            while (recursionFlag == 1 && depth > 1) {
                CompleteLine(gameBoard[initialY, initialX], type, playerCell, --depth, 0, ref substitution);
            }
        }
        
        private void ChangeCellType(CellTypes from, CellTypes to) {
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (gameBoard[i, j].Type == from) gameBoard[i, j].Type = to;
                }
            }
        }
        
        private void FindNeighbouring(CellTypes type1, CellTypes type2) {
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (i != 0 && gameBoard[i - 1, j].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i - 1, j].Type = CellTypes.Neighbouring;
                    }
                    if (i != boardSize - 1 && gameBoard[i + 1, j].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i + 1, j].Type = CellTypes.Neighbouring;
                    }
                    if (j != 0 && gameBoard[i, j - 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i, j - 1].Type = CellTypes.Neighbouring;
                    }
                    if (j != boardSize - 1 && gameBoard[i, j + 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i, j + 1].Type = CellTypes.Neighbouring;
                    }
                    if (i != 0 && j != 0 && gameBoard[i - 1, j - 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i - 1, j - 1].Type = CellTypes.Neighbouring;
                    }
                    if (i != boardSize - 1 && j != boardSize - 1 && gameBoard[i + 1, j + 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i + 1, j + 1].Type = CellTypes.Neighbouring;
                    }
                    if (i != 0 && j != boardSize - 1 && gameBoard[i - 1, j + 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i - 1, j + 1].Type = CellTypes.Neighbouring;
                    }
                    if (j != 0 && i != boardSize - 1 && gameBoard[i + 1, j - 1].Type == type1 && gameBoard[i, j].Type == type2) {
                        gameBoard[i + 1, j - 1].Type = CellTypes.Neighbouring;
                    }
                }
            }
        }
        
        private bool IsGameWinnable() {
            if (!winnable) return false;
            // int counter = 0;
            // for (int i = 0; i < boardSize; i++) {
            //     for (int j = 0; j < boardSize; j++) {
            //         if (gameBoard[i, j].Type == CellTypes.Usable)
            //             counter++;
            //     }
            // }
            //
            // if (counter == 0 && state != 2) state = 0;
            // if (state == 2) state--;
            return true;
        }


        private byte BoardSize => boardSize;
    }
}

        // private void GetUsableCells(Cell currentCell, CellTypes type, int direction) {
        //     if (direction == 8) {
        //         if(currentCell.Y == 0) return;
        //         if (currentCell.Y != 0 && gameBoard[currentCell.Y - 1, currentCell.X].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y - 1, currentCell.X], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y - 1, currentCell.X].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y - 1, currentCell.X].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //
        //     if (direction == 7) {
        //         if(currentCell.Y == boardSize - 1) return;
        //         if (currentCell.Y != boardSize - 1 && gameBoard[currentCell.Y + 1, currentCell.X].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y + 1, currentCell.X], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y + 1, currentCell.X].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y + 1, currentCell.X].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 6) {
        //         if(currentCell.X == 0) return;
        //         if (currentCell.X != 0 && gameBoard[currentCell.Y, currentCell.X - 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y, currentCell.X - 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y, currentCell.X - 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y, currentCell.X - 1].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 5) {
        //         if(currentCell.X == boardSize - 1) return;
        //         if (currentCell.X != boardSize - 1 && gameBoard[currentCell.Y, currentCell.X + 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y, currentCell.X + 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y, currentCell.X + 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y, currentCell.X + 1].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 4) {
        //         if(currentCell.X == 0 && currentCell.Y == 0) return;
        //         if (currentCell.X != 0 && currentCell.Y != 0 && gameBoard[currentCell.Y - 1, currentCell.X - 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y - 1, currentCell.X - 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y - 1, currentCell.X - 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y - 1, currentCell.X - 1].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 3) {
        //         if(currentCell.X == boardSize - 1 && currentCell.Y == boardSize - 1) return;
        //         if (currentCell.X != boardSize - 1 && currentCell.Y != boardSize - 1 && gameBoard[currentCell.Y + 1, currentCell.X + 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y + 1, currentCell.X + 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y + 1, currentCell.X + 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y + 1, currentCell.X + 1].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 2) {
        //         if(currentCell.Y == 0 && currentCell.X == boardSize - 1) return;
        //         if (currentCell.Y != 0 && currentCell.X != boardSize - 1 && gameBoard[currentCell.Y - 1, currentCell.X + 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y - 1, currentCell.X + 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y - 1, currentCell.X + 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y - 1, currentCell.X + 1].Type = CellTypes.Usable;
        //         }
        //         else return;
        //     }
        //     
        //     if (direction == 4) {
        //         if(currentCell.X == 0 && currentCell.Y == boardSize - 1) return;
        //         if (currentCell.X != 0 && currentCell.Y != boardSize - 1 && gameBoard[currentCell.Y + 1, currentCell.X - 1].Type == type) {
        //             GetUsableCells(gameBoard[currentCell.Y + 1, currentCell.X - 1], type, direction);
        //         }
        //         else if (gameBoard[currentCell.Y + 1, currentCell.X - 1].Type == CellTypes.Free && currentCell.Type == type) {
        //             gameBoard[currentCell.Y + 1, currentCell.X - 1].Type = CellTypes.Usable;
        //         }
        //     }
        //     
        // }