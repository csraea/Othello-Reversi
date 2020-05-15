using System;
using System.Collections.Generic;
using Reversi;
using Reversi.Core.Service.Comments;
using Reversi.Core.Service.Rating;
using Reversi.Core.Service.Score;

namespace ReversiWeb.Models
{

    [Serializable]
    public class OthelloModel
    {
        public GameLogic Logic { get; set; }

        public byte NowPlaying { get; set; }

        public string Opinion { get; set; }

        public bool RealPlayers { get; set; }
        public string Message { get; set; }

        public IList<Score> Scores { get; set; }

        public IList<Comment> Comments { get; set; }

        public IList<Rating> Ratings { get; set; }


        public bool jack { get; set; }

        public bool tips { get; set; }
    }
    }

