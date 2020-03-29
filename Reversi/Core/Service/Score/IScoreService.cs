using System.Collections.Generic;
using Reversi.Core.Service.Score;

namespace Service {
    public interface IScoreService {
        void AddScore(Score score);

        IList<Score> GetTopScores();

        void ClearScores();
    }
}
