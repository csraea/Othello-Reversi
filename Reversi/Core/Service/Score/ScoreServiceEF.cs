using System;
using Service;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Reversi.Core.Service.Score
{

    [Serializable]
    public class ScoreServiceEF : IScoreService
    {
        public void AddScore(Score score)
        {
            using (var context = new ReversiDBContext())
            {
                context.Scores.Add(score);
                context.SaveChanges();
            }
        }

        public IList<Score> GetTopScores()
        {
            using (var context = new ReversiDBContext())
            {
                return (from s in context.Scores
                        orderby s.Points
                           descending
                        select s).Take(5).ToList();
            }
        }

        [System.Obsolete]
        public void ClearScores()
        {
            using (var context = new ReversiDBContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Scores");
            }
        }
    }

}
