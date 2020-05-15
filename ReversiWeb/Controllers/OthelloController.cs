using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Reversi;
using Reversi.Core.Players;
using Reversi.Core.Players.AIBehaviours;
using Reversi.Core.Service.Comments;
using Reversi.Core.Service.Rating;
using Reversi.Core.Service.Score;
using ReversiWeb.Models;
using Service;

namespace ReversiWeb.Controllers
{
    public class OthelloController : Controller
    {
        private bool fiPl = false;
        private bool sePl = false;
        private Behaviour.Mode aiMode;
        private string aiColor;
        private CellTypes pl, en;
        
        public IActionResult Index()
        {
            var logic = new GameLogic();
            HttpContext.Session.SetObject("Logic", logic);
            HttpContext.Session.SetObject("SService", logic.scoreService);
            HttpContext.Session.SetObject("CService", logic.commentService);
            HttpContext.Session.SetObject("RService", logic.ratingService);


            bool mp = true;
            HttpContext.Session.SetObject("Real", mp);

            return View();
        }


        // Puts HumanPlayer into the HTTP Context
        public IActionResult AdjustHumanPlayer(string color, string name, string pattern)
        {
            var HuPlayer = new HumanPlayer(ObtainLogic(), name, color);
            HttpContext.Session.SetObject(pattern, HuPlayer);


            return View("SinglePlayerGameSettings");
        }

        // Puts AIPlayer into the HTTP Context
        public IActionResult AdjustAI(Behaviour.Mode mode, string color)
        {
            var SePlayer = new AIPlayer(mode, ObtainLogic(), color);
            HttpContext.Session.SetObject("SePlayer", SePlayer);

            aiMode = mode;
            aiColor = color;


            bool mp = false;
            HttpContext.Session.SetObject("Real", mp);
            return View("SinglePlayerGameSettings");
        }

        public IActionResult AdjustBoard(byte boardSize)
        {
            HttpContext.Session.SetObject("BSize", boardSize);


            return View("SinglePlayerGameSettings");
        }


        public IActionResult SinglePlayerGameSettings()
        {
            return View();
        }

        public IActionResult MultiPlayerGameSettings()
        {
            return View();
        }





        public IActionResult StartGame(bool mp)
        {
            GameLogic logic = ObtainLogic();
            logic.humanPlayer = ObtainHuPlayer();
            logic.secondPlayer = ObtainSePlayer();
            logic.boardSize = ObtainBSize();
            logic.GameBoard = new Cell[ObtainBSize(), ObtainBSize()];
            logic.FillBoard(CellTypes.Free);
            logic.LocatePlayers();
            logic.DetermineUsableCells(CellTypes.Player1, CellTypes.Player2);
            HttpContext.Session.SetObject("Logic", logic);


            byte nowPlaying = 0;
            HttpContext.Session.SetObject("now", nowPlaying);

            var model = InitializeModel(ObtainHuPlayer().Name + "'s move", null);
            model.NowPlaying = ObtainPlayer();
            
            PushModel(model);

            return mp ? View("GamePl2", model) : View("GamePl1", model);
            //return Content("Logic was pushed to the current context, model was generated");
        }

        public IActionResult AIMove()
        {
            var model = InitializeModel(ObtainHuPlayer().Name+"'s move", ObtainLogic().Think());

            model = UpdateMoveInfo(model);
            if (model.Logic.IsGameWinnable(pl) == 0)
            {
                PushModel(model);
                return View("GamePl1", model);
            }
            if (model.Logic.IsGameWinnable(pl) == -1 || model.Logic.IsGameWinnable(en) == -1)
            {

                IScoreService service = ObtainSService();



                service.AddScore(new Score
                {
                    Player = model.Logic.secondPlayer.Name,
                    Points = model.Logic.secondPlayer.GetScore(model.Logic.GameBoard, CellTypes.Player2, model.Logic.boardSize),
                    Time = DateTime.Now
                });
                service.AddScore(new Score
                {
                    Player = model.Logic.humanPlayer.Name,
                    Points = model.Logic.humanPlayer.GetScore(model.Logic.GameBoard, CellTypes.Player1, model.Logic.boardSize),
                    Time = DateTime.Now
                });


                return Content("THE END");
            }
            model.Logic.secondPlayer = new AIPlayer((Behaviour.Mode)HttpContext.Session.GetObject("ai"), ObtainLogic(), ObtainAIColor());

            Cell[,] g = model.Logic.GameBoard;

            model.Logic.secondPlayer.MakeTurn(ref g);

            model.Logic.GameBoard = g;

            model.Logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
            model.Logic.Magic(en, pl);
            model.Logic.ChangeCellType(CellTypes.Selected, CellTypes.Player2);
            model.Logic.DetermineUsableCells(en, pl);

            PushModel(model);
            return View("GamePl1", model);

        }



        public IActionResult NewMove(int y, int x)
        {

            byte pla = ObtainPlayer();

            var model = pla == 0
                ? InitializeModel(ObtainSePlayer().Name + "'s move", null)
                : InitializeModel(ObtainHuPlayer().Name + "'s move", null);

            bool mp = (bool)HttpContext.Session.GetObject("Real");

            string view = mp ? "GamePl2" : "GamePl1";

            model = UpdateMoveInfo(model);
            if (model.Logic.IsGameWinnable(pl) == 0)
            {
                PushModel(model);
                return View(view, model);
            }
            if (model.Logic.IsGameWinnable(pl) == -1 || model.Logic.IsGameWinnable(en) == -1)
            {
                IScoreService service = ObtainSService();



                service.AddScore(new Score
                {
                    Player = model.Logic.secondPlayer.Name,
                    Points = model.Logic.secondPlayer.GetScore(model.Logic.GameBoard, CellTypes.Player2, model.Logic.boardSize),
                    Time = DateTime.Now
                });
                service.AddScore(new Score
                {
                    Player = model.Logic.humanPlayer.Name,
                    Points = model.Logic.humanPlayer.GetScore(model.Logic.GameBoard, CellTypes.Player1, model.Logic.boardSize),
                    Time = DateTime.Now
                });



                return Content("THE END");
            }
            if (model.Logic.GameBoard[y, x].Type == CellTypes.Usable)
                model.Logic.GameBoard[y, x].Type = CellTypes.Selected;

                
            else
            {
                model.Message = "Wrong cell selected!";
                model.NowPlaying = model.NowPlaying == 0 ? (byte)1 : (byte)0;
                PushModel(model);
                return View(view, model);
            }


            model.Logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
            model.Logic.Magic(en, pl);
            model.Logic.DetermineUsableCells(en, pl);

            PushModel(model);
            return View(view, model);

        }



        private OthelloModel UpdateMoveInfo(OthelloModel model)
        {
            if (model.NowPlaying == 0)
            {
                model.NowPlaying = 1;
                pl = CellTypes.Player1;
                en = CellTypes.Player2;
            }
            else
            {
                model.NowPlaying = 0;
                pl = CellTypes.Player2;
                en = CellTypes.Player1;
            }

            return model;
        }


        [HttpPost]
        public IActionResult AdjustSinglePlayer(int red, int green, int blue, int red1, int green1, int blue1, string name, int boardSize, bool usable, bool jack, string difficulty)
        {
            var HuPlayer = new HumanPlayer(ObtainLogic(), name, "rgb(" + red1 + ", " + green1 + ", " + blue1 + ")");
            HttpContext.Session.SetObject("HuPlayer", HuPlayer);

            var SePlayer = new AIPlayer((Behaviour.Mode) Int32.Parse( difficulty), ObtainLogic(), "rgb(" + red + ", " + green + ", " + blue + ")");
            HttpContext.Session.SetObject("SePlayer", SePlayer);

            aiMode = (Behaviour.Mode)int.Parse(difficulty);
            aiColor = "rgb(" + red + ", " + green + ", " + blue + ")";
            HttpContext.Session.SetObject("AIcolor", aiColor);
            HttpContext.Session.SetObject("ai", aiMode);

            bool mp = false;
            HttpContext.Session.SetObject("Real", mp);

            HttpContext.Session.SetObject("BSize", (byte) boardSize);



            bool us = usable;
            bool ja = jack;
            HttpContext.Session.SetObject("usable", us);
            HttpContext.Session.SetObject("jack", ja);

            return StartGame(mp);
        }


        [HttpPost]
        public IActionResult AdjustMultiPlayer(int red, int green, int blue, int red1, int green1, int blue1, string name, string name1, int boardSize)
        {
            var HuPlayer = new HumanPlayer(ObtainLogic(), name, "rgb(" + red + ", " + green + ", " + blue + ")");
            HttpContext.Session.SetObject("HuPlayer", HuPlayer);

            var SePlayer = new HumanPlayer(ObtainLogic(), name1, "rgb(" + red1 + ", " + green1 + ", " + blue1 + ")");
            HttpContext.Session.SetObject("SePlayer", SePlayer);


            bool mp = true;
            HttpContext.Session.SetObject("Real", mp);

            HttpContext.Session.SetObject("BSize", (byte)boardSize);

            bool ja = false;
            HttpContext.Session.SetObject("jack", ja); 
            bool us = false;
            HttpContext.Session.SetObject("usable", us);


            return StartGame(mp);
        }


        public IActionResult DisplayScores()
        {
            var model = InitModel("display scores", null);

            return View("ScoreTable",model);
        }






        private void PushModel(OthelloModel model)
        {
            HttpContext.Session.SetObject("Model", model);
            HttpContext.Session.SetObject("Logic", model.Logic);
            HttpContext.Session.SetObject("now", model.NowPlaying);
        }

        private OthelloModel InitializeModel(string message, string opinion)
        {
            return new OthelloModel
            {
                Logic = ObtainLogic(),
                Message = message,
                Opinion = opinion,
                Scores = ObtainSService().GetTopScores(),
                Comments = ObtainCService().GetLastComments(),
                Ratings = ObtainRService().GetLastRatings(),
                NowPlaying = ObtainPlayer(),
                tips = (bool)HttpContext.Session.GetObject("usable"),
                jack = (bool)HttpContext.Session.GetObject("jack")


        };
        }

        private OthelloModel InitModel(string message, string opinion)
        {
            return new OthelloModel
            {
                Logic = ObtainLogic(),
                Message = message,
                Opinion = opinion,
                Scores = ObtainSService().GetTopScores(),
                Comments = ObtainCService().GetLastComments(),
                Ratings = ObtainRService().GetLastRatings()
            };
        }

        private string ObtainAIColor()
        {
            return (string) HttpContext.Session.GetObject("AIcolor");
        }

        private GameLogic ObtainLogic()
        {
            return (GameLogic) HttpContext.Session.GetObject("Logic");
        }

        private OthelloModel ObtainModel()
        {
            return (OthelloModel) HttpContext.Session.GetObject("Model");
        }

        private byte ObtainPlayer()
        {
            return (byte) HttpContext.Session.GetObject("now");
        }

        private byte ObtainBSize()
        {
            return (byte)HttpContext.Session.GetObject("BSize");
        }

        private Player ObtainSePlayer()
        {
            return (Player) HttpContext.Session.GetObject("SePlayer");
        }

        private Player ObtainHuPlayer()
        {
            return (Player) HttpContext.Session.GetObject("HuPlayer");
        }

        private ICommentService ObtainCService()
        {
            return (ICommentService) HttpContext.Session.GetObject("CService");
        }

        private IScoreService ObtainSService()
        {
            return (IScoreService) HttpContext.Session.GetObject("SService");
        }

        private IRatingService ObtainRService()
        {
            return (IRatingService) HttpContext.Session.GetObject("RService");
        }
    }


    // Model = InitializeModel("New othello model is ready!");
    // HttpContext.Session.SetObject("Model", Model);
    // // HttpContext.Session.SetObject("HumanPlayer", Model.HumanPlayer);





    // public IActionResult Index()
    // {
    //     var field = new Field(4, 4);
    //     HttpContext.Session.SetObject("field", field);
    //
    //     var model = PrepareModel("New field created");
    //     return View(model);
    // }

    // public IActionResult Move(int tile)
    // {
    //     var field = (Field)HttpContext.Session.GetObject("field");
    //     field.Move(tile);
    //     HttpContext.Session.SetObject("field", field);
    //
    //     if (field.IsSolved())
    //         _scoreService.AddScore(new Score { Player = "Janko", Points = field.GetScore() });
    //
    //     var model = PrepareModel("Moved");
    //     return View("Index", model);
    // }
}