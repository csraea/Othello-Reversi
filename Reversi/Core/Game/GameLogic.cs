using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NPuzzle.Entity;
using NPuzzle.Service;
using Reversi.Core.Players;
using Reversi.Core.Players.AIBehaviours;
using Reversi.Core.Service.Comments;
using Service;

namespace Reversi {
    public class GameLogic {
        private byte boardSize;
        private Cell[,] gameBoard;
        private readonly byte _gameMode;
        Player humanPlayer;
        Player secondPlayer;
        private int state;
        private UI ui;
        
        private readonly ICommentService commentService = new CommentService();
        private readonly IScoreService scoreService = new ScoreServiceFile();


        public GameLogic(byte boardSize, byte gameMode, UI ui) {
            this.boardSize = boardSize;
            this.ui = ui;
            gameBoard = new Cell[boardSize, boardSize];
            _gameMode = gameMode;
            humanPlayer = new HumanPlayer(this, ui.GetName(), ConsoleColor.Blue);
            secondPlayer = (gameMode == 1) ? (Player) new AIPlayer(Behaviour.Mode.Easy, this, ConsoleColor.Red) : new HumanPlayer(this, ui.GetName(), ConsoleColor.Red);
            state = 2;
        }
        
        private void LocatePlayers() {
            gameBoard[boardSize / 2, boardSize / 2 - 1].Type = CellTypes.Player2;
            gameBoard[boardSize / 2 - 1, boardSize / 2].Type = CellTypes.Player2;
            gameBoard[boardSize / 2, boardSize / 2].Type = CellTypes.Player1;
            gameBoard[boardSize / 2 - 1, boardSize / 2 -1].Type = CellTypes.Player1;
        }

        private void FillBoard(CellTypes cellType) {
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    gameBoard[i, j] = new Cell(1, cellType, j, i);
                }
            }
        }

        public sbyte StartGame() {
            FillBoard(CellTypes.Free);
            LocatePlayers();
            Player player = secondPlayer;
            bool firstTurn = true;
            do {
                player = (player == humanPlayer) ? secondPlayer : humanPlayer;

                if(player == humanPlayer) DetermineUsableCells(CellTypes.Player1, CellTypes.Player2);
                else DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
                
                ui.DisplayGame(humanPlayer, secondPlayer, gameBoard, boardSize);
                if(!firstTurn && secondPlayer.Name.Equals("Handsome Jack") && player == secondPlayer) ui.Think();
                
                if(!IsGameWinnable()) break;
                if (!player.MakeTurn(ref gameBoard)) {
                        ui.Exit();
                        return 1;
                }

                ChangeCellType(CellTypes.Usable, CellTypes.Free);
                
                if(player == humanPlayer) Magic(CellTypes.Player2, CellTypes.Player1); 
                else Magic(CellTypes.Player1, CellTypes.Player2);

                firstTurn = false;
            } while (true);
            
            scoreService.AddScore(new Score{Player = humanPlayer.Name, Points = humanPlayer.GetScore(gameBoard,CellTypes.Player1, boardSize), Time = DateTime.Now});
            scoreService.AddScore(new Score{Player = secondPlayer.Name, Points = secondPlayer.GetScore(gameBoard,CellTypes.Player2, boardSize), Time = DateTime.Now});
            ui.PrintScores(scoreService);
            
            Console.ReadLine();
            if(commentService.GetLastComments().Any()) ui.PrintComments(commentService);
            commentService.AddComment(new Comment{Player = secondPlayer.Name, Text = ui.GetComment(secondPlayer), Time = DateTime.Now});
            commentService.AddComment(new Comment{Player = humanPlayer.Name, Text = ui.GetComment(humanPlayer), Time = DateTime.Now});
            if (ui.Restart() > -1) return StartGame();
            else {
                ui.Exit();
            }
            return 0;
        }

        public void DetermineUsableCells(CellTypes target, CellTypes with) {
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
        
        public void InspectCellByLine(Cell currentCell, CellTypes type, int depth, int recursionFlag) {
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
            int x = 0, y = 0, limit1 = 0, limit2 = 0; // values used in recursion 
            
            int initialX = 0, initialY = 0; // save the initial cell coords for which all the directions are being tested
            if (recursionFlag == 1) {
                initialX = currentCell.X;
                initialY = currentCell.Y;
            }
            
            ParseDirection(ref x, ref y, ref limit1, ref limit2, depth); // update variables with the direction based on depth
            
            //exact recursion
            if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == type) {
                CompleteLine(gameBoard[currentCell.Y + y, currentCell.X + x], type, playerCell, depth, 0, ref substitution);
                if (substitution == 1 && currentCell.Type != CellTypes.Selected) currentCell.Type = playerCell;
                if (currentCell.X == initialX && currentCell.Y == initialY) substitution = 0;
            } else if (currentCell.X != limit1 && currentCell.Y != limit2 && gameBoard[currentCell.Y + y, currentCell.X + x].Type == playerCell && currentCell.Type == type) {
                gameBoard[currentCell.Y, currentCell.X].Type = playerCell;
                substitution = 1;
            } else if (currentCell.X != limit1 && currentCell.Y != limit2 && (gameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Free || gameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Usable) && currentCell.Type == type) {
                substitution = 0;
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
            for (int i = 0; i < boardSize; i++) {
                for (int j = 0; j < boardSize; j++) {
                    if (gameBoard[i, j].Type == CellTypes.Usable) return true;
                }
            }

            return false;
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
        
        
// Behaviour.GetPossibleMoves(gameBoard, ref Behaviour.PossibleMoves);
// bool winnable = Behaviour.PossibleMoves.Any();
// Behaviour.PossibleMoves.Clear();
// return winnable;