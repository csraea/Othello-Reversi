using System.Collections.Generic;
using NPuzzle.Entity;

namespace Service {
    public interface IScoreService {
        void AddScore(Score score);

        IList<Score> GetTopScores();

        void ClearScores();
    }
}
