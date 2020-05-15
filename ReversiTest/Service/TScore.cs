using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Core.Service.Score;
using Service;

namespace ReversiTest.Service {
    
    [TestClass]
    public class TScore {
        private IScoreService ScoreService() {
            var scoreService = new ScoreService();
            scoreService.ClearScores();
            return scoreService;
        }


        [TestMethod]
        public void AddScore1() {
            var scoreService = ScoreService();

            scoreService.AddScore(new Score { Player = "Janko", Points = 120, Time = DateTime.Now});
            var scores = scoreService.GetTopScores();

            Assert.AreEqual<int>(1, scores.Count);
            Assert.AreEqual<string>("Janko", scores[0].Player);
            Assert.AreEqual<int>(120, scores[0].Points);
        }

        [TestMethod]
        public void AddScore2() {
            var scoreService = ScoreService();
            scoreService.AddScore(new Score { Player = "Janko", Points = 120, Time = DateTime.Now });
            scoreService.AddScore(new Score { Player = "Zuzka", Points = 200, Time = DateTime.Now });
            var scores = scoreService.GetTopScores();

            Assert.AreEqual<int>(2, scores.Count);
            Assert.AreEqual<string>("Zuzka", scores[0].Player);
            Assert.AreEqual<int>(200, scores[0].Points);
            Assert.AreEqual<string>("Janko", scores[1].Player);
            Assert.AreEqual<int>(120, scores[1].Points);
        }
        
        [TestMethod]
        public void AddScore3() {
            var scoreService = ScoreService();
            scoreService.AddScore(new Score { Player = "Janko", Points = 120, Time = DateTime.Now });
            scoreService.AddScore(new Score { Player = "Zuzka", Points = 200, Time = DateTime.Now });
            scoreService.AddScore(new Score { Player = "Alexa", Points = 100, Time = DateTime.Now });
            scoreService.AddScore(new Score { Player = "Marius", Points = 30, Time = DateTime.Now });
            var scores = scoreService.GetTopScores();

            Assert.AreEqual<int>(3, scores.Count);
            Assert.AreEqual<string>("Zuzka", scores[0].Player);
            Assert.AreEqual<int>(200, scores[0].Points);
            Assert.AreEqual<string>("Janko", scores[1].Player);
            Assert.AreEqual<int>(120, scores[1].Points);
            Assert.AreEqual<string>("Alexa", scores[2].Player);
            Assert.AreEqual<int>(100, scores[2].Points);
        }
        
        [TestMethod]
        public void AddScore4() {
            var scoreService = ScoreService();
            var scores = scoreService.GetTopScores();
            Assert.AreEqual<int>(0, scores.Count);
        }
        
    }
}