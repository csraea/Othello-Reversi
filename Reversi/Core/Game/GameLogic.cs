using System;
using System.Linq;
using Reversi.Core.Players;
using Reversi.Core.Players.AIBehaviours;
using Reversi.Core.Service.Comments;
using Reversi.Core.Service.Rating;
using Reversi.Core.Service.Score;
using Service;

namespace Reversi {

    [Serializable]
    public class GameLogic
    {
        public byte boardSize { get; set; }
        public Cell[,] GameBoard { get; set; }
        public Player humanPlayer { get; set; }
        public Player secondPlayer { get; set; }

        private UI ui;

        public bool skippedTurn;

        //        private readonly ICommentService commentService = new CommentService();
        //        private readonly IScoreService scoreService = new ScoreService();
        //        private readonly IRatingService ratingService = new RatingService();

        public readonly ICommentService commentService = new CommentServiceEF();
        public readonly IScoreService scoreService = new ScoreServiceEF();
        public readonly IRatingService ratingService = new RatingServiceEF();

        public GameLogic(byte boardSize)
        {
            this.boardSize = boardSize;
            GameBoard = new Cell[boardSize, boardSize];
        }


        public GameLogic()
        {

        }

        public GameLogic(byte boardSize, byte gameMode, UI ui)
        {
            this.boardSize = boardSize;
            this.ui = ui;
            GameBoard = new Cell[boardSize, boardSize];
            humanPlayer = new HumanPlayer(this, ui.GetName(), ConsoleColor.Blue);
            secondPlayer = (gameMode == 1)
                ? (Player) new AIPlayer(ui.GetDifficulty(), this, ConsoleColor.Red)
                : new HumanPlayer(this, ui.GetName(), ConsoleColor.Red);
        }

        public void LocatePlayers()
        {
            GameBoard[boardSize / 2, boardSize / 2 - 1].Type = CellTypes.Player2;
            GameBoard[boardSize / 2 - 1, boardSize / 2].Type = CellTypes.Player2;
            GameBoard[boardSize / 2, boardSize / 2].Type = CellTypes.Player1;
            GameBoard[boardSize / 2 - 1, boardSize / 2 - 1].Type = CellTypes.Player1;
        }

        public void FillBoard(CellTypes cellType)
        {
            
            for (int i = 0; i < boardSize; i++)
            {
                byte weight = 1;

                for (int j = 0; j < boardSize; j++)
                {
                    if ((i == 0 && j == 0) || (i == 0 && j == boardSize - 1) ||
                        (i == boardSize - 1 && i == boardSize - 1) || (j == 0 && i == boardSize - 1))
                    {
                        weight = 10;
                    }
    
                    GameBoard[i, j] = new Cell(weight, cellType, j, i);
                }
            }
        }

        public sbyte StartGame()
        {
            FillBoard(CellTypes.Free);
            LocatePlayers();
            Player player = secondPlayer;
            bool firstTurn = true;

            do
            {
                player = (player == humanPlayer) ? secondPlayer : humanPlayer;

                if (!skippedTurn)
                {
                    if (player == humanPlayer) DetermineUsableCells(CellTypes.Player1, CellTypes.Player2);
                    else DetermineUsableCells(CellTypes.Player2, CellTypes.Player1);
                }

                skippedTurn = false;

                // UI based on player's turn. (1,3 - optional)
                if (firstTurn && !ratingService.GetLastRatings().Count.Equals(0)) ui.PrintRating(ratingService);
                ui.DisplayGame(humanPlayer, secondPlayer, GameBoard, boardSize);
                if (!firstTurn && secondPlayer.Name.Equals("Handsome Jack") && player == secondPlayer) ui.Think();

                sbyte winnable = IsGameWinnable(player == humanPlayer ? CellTypes.Player1 : CellTypes.Player2);
                if (winnable == -1) break;
                if (winnable == 0) goto NEXTPLAYER;

                Cell[,] gameboard = GameBoard; 

                if (!player.MakeTurn(ref gameboard))
                {
                    ui.Exit();
                    return 1;
                }

                GameBoard = gameboard;


                ChangeCellType(CellTypes.Usable, CellTypes.Free);

                if (player == humanPlayer) Magic(CellTypes.Player2, CellTypes.Player1);
                else Magic(CellTypes.Player1, CellTypes.Player2);

                firstTurn = false;

                NEXTPLAYER: ;

                Console.WriteLine(""+((AIPlayer)secondPlayer).ai);

            } while (true);

            scoreService.AddScore(new Score
            {
                Player = humanPlayer.Name, Points = humanPlayer.GetScore(GameBoard, CellTypes.Player1, boardSize),
                Time = DateTime.Now
            });
            scoreService.AddScore(new Score
            {
                Player = secondPlayer.Name, Points = secondPlayer.GetScore(GameBoard, CellTypes.Player2, boardSize),
                Time = DateTime.Now
            });
            ui.PrintScores(scoreService);

            Console.ReadLine();
            if (commentService.GetLastComments().Any()) ui.PrintComments(commentService);
            commentService.AddComment(new Comment
                {Player = secondPlayer.Name, Text = ui.GetComment(secondPlayer), Time = DateTime.Now});
            commentService.AddComment(new Comment
                {Player = humanPlayer.Name, Text = ui.GetComment(humanPlayer), Time = DateTime.Now});

            ratingService.Rate(new Rating {Mark = ui.GetMark(), Player = humanPlayer.Name});
            if (ui.Restart() > -1) return StartGame();
            ui.Exit();

            return 0;
        }

        public void DetermineUsableCells(CellTypes target, CellTypes with)
        {
            FindNeighbouring(target, with);
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (GameBoard[i, j].Type == CellTypes.Neighbouring)
                    {
                        InspectCellByLine(GameBoard[i, j], with, 8, 1);
                    }
                }
            }

            ChangeCellType(CellTypes.Neighbouring, target);
        }

        public void InspectCellByLine(Cell currentCell, CellTypes type, int depth, int recursionFlag)
        {
            int x = 0, y = 0, limit1 = 0, limit2 = 0; // values used in recursion

            int initialX = 0, initialY = 0; //save the initial cell coords for which all the directions are being tested
            if (recursionFlag == 1)
            {
                initialX = currentCell.X;
                initialY = currentCell.Y;
            }

            ParseDirection(ref x, ref y, ref limit1, ref limit2, depth); //update variables

            //exact recursion
            if (currentCell.X != limit1 && currentCell.Y != limit2 &&
                GameBoard[currentCell.Y + y, currentCell.X + x].Type == type)
            {
                InspectCellByLine(GameBoard[currentCell.Y + y, currentCell.X + x], type, depth, 0);
            }
            else if (currentCell.X != limit1 && currentCell.Y != limit2 &&
                     GameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Free && currentCell.Type == type)
            {
                GameBoard[currentCell.Y + y, currentCell.X + x].Type = CellTypes.Usable;
            }

            //change the direction
            while (recursionFlag == 1 && depth > 1)
            {
                InspectCellByLine(GameBoard[initialY, initialX], type, --depth, 0);
            }
        }

        private void ParseDirection(ref int x, ref int y, ref int limit1, ref int limit2, int depth)
        {
            switch (depth)
            {
                case 8:
                    limit1 = -1;
                    limit2 = 0;
                    x = 0;
                    y = -1;
                    break;
                case 7:
                    limit1 = -1;
                    limit2 = boardSize - 1;
                    x = 0;
                    y = 1;
                    break;
                case 6:
                    limit1 = 0;
                    limit2 = -1;
                    x = -1;
                    y = 0;
                    break;
                case 5:
                    limit1 = boardSize - 1;
                    limit2 = -1;
                    x = 1;
                    y = 0;
                    break;
                case 4:
                    limit1 = 0;
                    limit2 = 0;
                    x = -1;
                    y = -1;
                    break;
                case 3:
                    limit1 = boardSize - 1;
                    limit2 = boardSize - 1;
                    x = 1;
                    y = 1;
                    break;
                case 2:
                    limit1 = boardSize - 1;
                    limit2 = 0;
                    x = 1;
                    y = -1;
                    break;
                case 1:
                    limit1 = 0;
                    limit2 = boardSize - 1;
                    x = -1;
                    y = 1;
                    break;
                default: return;
            }
        }

        public void Magic(CellTypes type, CellTypes playerCell)
        {
            int substitution = 0;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (GameBoard[i, j].Type == CellTypes.Selected)
                    {
                        CompleteLine(GameBoard[i, j], type, playerCell, 8, 1, ref substitution);
                        ChangeCellType(CellTypes.Selected, playerCell);
                        return;
                    }
                }
            }
        }

        private void CompleteLine(Cell currentCell, CellTypes type, CellTypes playerCell, int depth, int recursionFlag,
            ref int substitution)
        {
            int x = 0, y = 0, limit1 = 0, limit2 = 0; // values used in recursion 

            int initialX = 0,
                initialY = 0; // save the initial cell coords for which all the directions are being tested
            if (recursionFlag == 1)
            {
                initialX = currentCell.X;
                initialY = currentCell.Y;
            }

            ParseDirection(ref x, ref y, ref limit1, ref limit2,
                depth); // update variables with the direction based on depth

            //exact recursion
            if (currentCell.X != limit1 && currentCell.Y != limit2 &&
                GameBoard[currentCell.Y + y, currentCell.X + x].Type == type)
            {
                CompleteLine(GameBoard[currentCell.Y + y, currentCell.X + x], type, playerCell, depth, 0,
                    ref substitution);
                if (substitution == 1 && currentCell.Type != CellTypes.Selected) currentCell.Type = playerCell;
                if (currentCell.X == initialX && currentCell.Y == initialY) substitution = 0;
            }
            else if (currentCell.X != limit1 && currentCell.Y != limit2 &&
                     GameBoard[currentCell.Y + y, currentCell.X + x].Type == playerCell && currentCell.Type == type)
            {
                GameBoard[currentCell.Y, currentCell.X].Type = playerCell;
                substitution = 1;
            }
            else if (currentCell.X != limit1 && currentCell.Y != limit2 &&
                     (GameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Free ||
                      GameBoard[currentCell.Y + y, currentCell.X + x].Type == CellTypes.Usable) &&
                     currentCell.Type == type)
            {
                substitution = 0;
            }

            //change the direction
            while (recursionFlag == 1 && depth > 1)
            {
                CompleteLine(GameBoard[initialY, initialX], type, playerCell, --depth, 0, ref substitution);
            }
        }

        public void ChangeCellType(CellTypes from, CellTypes to)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (GameBoard[i, j].Type == from) GameBoard[i, j].Type = to;
                }
            }
        }

        private void FindNeighbouring(CellTypes type1, CellTypes type2)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (i != 0 && GameBoard[i - 1, j].Type == type1 && GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i - 1, j].Type = CellTypes.Neighbouring;
                    }

                    if (i != boardSize - 1 && GameBoard[i + 1, j].Type == type1 && GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i + 1, j].Type = CellTypes.Neighbouring;
                    }

                    if (j != 0 && GameBoard[i, j - 1].Type == type1 && GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i, j - 1].Type = CellTypes.Neighbouring;
                    }

                    if (j != boardSize - 1 && GameBoard[i, j + 1].Type == type1 && GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i, j + 1].Type = CellTypes.Neighbouring;
                    }

                    if (i != 0 && j != 0 && GameBoard[i - 1, j - 1].Type == type1 && GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i - 1, j - 1].Type = CellTypes.Neighbouring;
                    }

                    if (i != boardSize - 1 && j != boardSize - 1 && GameBoard[i + 1, j + 1].Type == type1 &&
                        GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i + 1, j + 1].Type = CellTypes.Neighbouring;
                    }

                    if (i != 0 && j != boardSize - 1 && GameBoard[i - 1, j + 1].Type == type1 &&
                        GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i - 1, j + 1].Type = CellTypes.Neighbouring;
                    }

                    if (j != 0 && i != boardSize - 1 && GameBoard[i + 1, j - 1].Type == type1 &&
                        GameBoard[i, j].Type == type2)
                    {
                        GameBoard[i + 1, j - 1].Type = CellTypes.Neighbouring;
                    }
                }
            }
        }

        public sbyte IsGameWinnable(CellTypes nowPlaying)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (GameBoard[i, j].Type == CellTypes.Usable) return 1;
                }
            }

            DetermineUsableCells(nowPlaying == CellTypes.Player1 ? CellTypes.Player2 : CellTypes.Player1, nowPlaying);

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (GameBoard[i, j].Type == CellTypes.Usable) return 0;
                }
            }

            return -1;
        }


        public string Think()
        {
            var phrases = new[]
            {
                "Hmm...",
                "I'm thinking...",
                "Good move!",
                "Hah, bustard!!",
                "I'm afraid you gonna lose.",
                "FUCK YOUR FUCKIN' FUCK!",
                "I feel sadness about that.",
                "I'm confused. Haha! Joke.",
                "Enjoy your nothing! Idiot.",
                "I'm the best player EVER!",
                "You ain't win anyway...",
                "Oh no! You made a fatal mistake!",
                "I'm a hero. And heroes always win.",
                "Why don't you just lose?",
                "Handsome Jack is handsome.",
                "Jack — one, uglies — zero!",
                "Your move wasn't as good as you may think.",
                "It's so cute you're trying to resist. Kidding, NO.",
                "Oh no! You're still here.",
                "PLEASE, DON'T WIN. It's MY duty.",
                "I feel like I win when you lose.",
                "The probability of your victory is 0.0000000%."

            };

            return phrases[new Random().Next(21)];
        }
    }
}

// Behaviour.GetPossibleMoves(gameBoard, ref Behaviour.PossibleMoves);
// bool winnable = Behaviour.PossibleMoves.Any();
// Behaviour.PossibleMoves.Clear();
// return winnable;