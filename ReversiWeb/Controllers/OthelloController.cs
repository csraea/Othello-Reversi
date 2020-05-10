using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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





        public IActionResult StartGame()
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

            var model = InitializeModel("game started", null);
            model.NowPlaying = ObtainPlayer();
            
            PushModel(model);

            return View("GamePl1", model);
            //return Content("Logic was pushed to the current context, model was generated");
        }

        public IActionResult AIMove()
        {
            var model = InitializeModel("Jack's move", ObtainLogic().Think());

            model = UpdateMoveInfo(model);
            if (model.Logic.IsGameWinnable(pl) == 0) return View("GamePl1", model);
            if (model.Logic.IsGameWinnable(pl) == -1)
            {
                return Content("THE END");
            }
            model.Logic.secondPlayer = new AIPlayer(aiMode, ObtainLogic(), ObtainAIColor());
            model.Logic.secondPlayer.MakeTurn(model.Logic.GameBoard);

            model.Logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
            model.Logic.Magic(en, pl);
            model.Logic.ChangeCellType(CellTypes.Selected, CellTypes.Player2);
            model.Logic.DetermineUsableCells(en, pl);

            PushModel(model);
            return View("GamePl1", model);

        }

        public IActionResult NewMove(int y, int x)
        {
            var model = InitializeModel("new move", null);

            model = UpdateMoveInfo(model);
            if (model.Logic.IsGameWinnable(pl) == 0) return View("GamePl1", model);
            if (model.Logic.IsGameWinnable(pl) == -1)
            {
                return Content("THE END");
            }
            if (model.Logic.GameBoard[y, x].Type == CellTypes.Usable)
                model.Logic.GameBoard[y, x].Type = CellTypes.Selected;
            else
            {
                model.Message = "Wrong cell selected!";
                model.NowPlaying = model.NowPlaying == 0 ? (byte)1 : (byte)0;
                PushModel(model);
                return View("GamePl1", model);
            }


            model.Logic.ChangeCellType(CellTypes.Usable, CellTypes.Free);
            model.Logic.Magic(en, pl);
            model.Logic.DetermineUsableCells(en, pl);

            PushModel(model);
            return View("GamePl1", model);

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
        public IActionResult AdjustSinglePlayer(int red, int green, int blue, int red1, int green1, int blue1)
        {
            var HuPlayer = new HumanPlayer(ObtainLogic(), "Lenin", "rgb(" + red1 + ", " + green1 + ", " + blue1 + ")");
            HttpContext.Session.SetObject("HuPlayer", HuPlayer);

            var SePlayer = new AIPlayer(Behaviour.Mode.Medium, ObtainLogic(), "rgb(" + red + ", " + green + ", " + blue + ")");
            HttpContext.Session.SetObject("SePlayer", SePlayer);

            aiMode = Behaviour.Mode.Medium;
            aiColor = "rgb(" + red + ", " + green + ", " + blue + ")";
            HttpContext.Session.SetObject("AIcolor", aiColor);


            bool mp = false;
            HttpContext.Session.SetObject("Real", mp);

            byte bs = 8;
            HttpContext.Session.SetObject("BSize", bs);





            return StartGame();
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
                NowPlaying = ObtainPlayer()
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