using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Reversi.Core.Service.Rating
{

    [Serializable]
    public class RatingServiceEF : IRatingService
    {
        public void Rate(Rating rating)
        {
            using (var context = new ReversiDBContext())
            {
                context.Ratings.Add(rating);
                context.SaveChanges();
            }
        }


        public IList<Rating> GetLastRatings()
        {
            using (var context = new ReversiDBContext())
            {
                return (from r in context.Ratings
                        orderby r.Mark
                           descending
                        select r).Take(5).ToList();
            }
        }

        [System.Obsolete]
        public void ClearRating()
        {
            using (var context = new ReversiDBContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Ratings");
            }
        }

        public float GetAverageRating()
        {
            var context = new ReversiDBContext();
            float total = 0f;
            int counter = 0;
            foreach (var r in context.Ratings)
            {
                total += r.Mark;
                counter++;
            }

            return (counter == 0) ? total : total / counter;
        }
    }
}
