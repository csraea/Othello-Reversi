using System.Collections.Generic;

namespace Reversi.Core.Service.Rating {
    public interface IRatingService {
        void Rate(Rating rating);

        IList<Rating> GetLastRatings();

        void ClearRating();

        float GetAverageRating();
    }
}