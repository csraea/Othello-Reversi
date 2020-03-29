using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Core.Service.Rating;

namespace ReversiTest.Service {
    
    [TestClass]
    public class TRating {
        private IRatingService RatingService() { 
            var ratingService = new RatingService();
            ratingService.ClearRating(); 
            return ratingService;
        }

        [TestMethod]
        public void AddRating1() {
            var ratingService = RatingService();
            
            ratingService.Rate(new Rating { Player = "Janko", Mark = 9 });
            var ratings = ratingService.GetLastRatings();

            Assert.AreEqual<int>(1, ratings.Count);
            Assert.AreEqual<string>("Janko", ratings[0].Player);
            Assert.AreEqual<int>(120, ratings[0].Mark);
        }

        [TestMethod]
        public void AddRating2() {
            var ratingService = RatingService();
            ratingService.Rate(new Rating { Player = "Janko", Mark = 3 });
            ratingService.Rate(new Rating { Player = "Zuzka", Mark = 7 });
            var ratings = ratingService.GetLastRatings();

            Assert.AreEqual<int>(2, ratings.Count);
            Assert.AreEqual<string>("Zuzka", ratings[0].Player);
            Assert.AreEqual<int>(7, ratings[0].Mark); 
            Assert.AreEqual<string>("Janko", ratings[1].Player); 
            Assert.AreEqual<int>(3, ratings[1].Mark);
        }
        
        [TestMethod]
        public void AddRating3() {
            var ratingService = RatingService();
            ratingService.Rate(new Rating { Player = "Alexa", Mark = 10 });
            ratingService.Rate(new Rating { Player = "Mozart", Mark = 9 });
            ratingService.Rate(new Rating { Player = "Adamko", Mark = 8 });
            ratingService.Rate(new Rating { Player = "Albert", Mark = 7 });
            ratingService.Rate(new Rating { Player = "Tirion", Mark = 6 });
            ratingService.Rate(new Rating { Player = "Martin", Mark = 5 });
            ratingService.Rate(new Rating { Player = "Lolita", Mark = 4 });
            var ratings = ratingService.GetLastRatings();

            Assert.AreEqual<int>(5, ratings.Count);
            Assert.AreEqual<string>("Alexa", ratings[0].Player);
            Assert.AreEqual<int>(10, ratings[0].Mark);
            Assert.AreEqual<string>("Mozart", ratings[1].Player); 
            Assert.AreEqual<int>(9, ratings[1].Mark);
            Assert.AreEqual<string>("Adamko", ratings[2].Player); 
            Assert.AreEqual<int>(8, ratings[2].Mark);
            Assert.AreEqual<string>("Albert", ratings[3].Player); 
            Assert.AreEqual<int>(7, ratings[3].Mark);
            Assert.AreEqual<string>("Tirion", ratings[4].Player); 
            Assert.AreEqual<int>(6, ratings[4].Mark);
        }
        
        [TestMethod]
        public void AddRating4() {
            var ratingService = RatingService();
            var ratings = ratingService.GetLastRatings();
            Assert.AreEqual<int>(0, ratings.Count);
        }
        
        [TestMethod]
        public void GetAverageRating1() {
            var ratingService = RatingService();
            ratingService.Rate(new Rating { Player = "Alexa", Mark = 10 });
            ratingService.Rate(new Rating { Player = "Mozart", Mark = 10 });

            Assert.AreEqual<float>(10, ratingService.GetAverageRating());
        }
        
        [TestMethod]
        public void GetAverageRating2() {
            var ratingService = RatingService();
            ratingService.Rate(new Rating { Player = "Alexa", Mark = 10 });
            ratingService.Rate(new Rating { Player = "Mozart", Mark = 0 });
            ratingService.Rate(new Rating { Player = "Marius", Mark = 5 });
            ratingService.Rate(new Rating { Player = "Lolita", Mark = 2 });

            Assert.AreEqual<float>(4.25f, ratingService.GetAverageRating());
        }
        
        [TestMethod]
        public void GetAverageRating3() {
            var ratingService = RatingService();

            Assert.AreEqual<float>(0f, ratingService.GetAverageRating());
        }
    }
}