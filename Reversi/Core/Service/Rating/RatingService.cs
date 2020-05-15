using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reversi.Core.Service.Rating {
    public class RatingService : IRatingService{
        private const string FileName = "rating.bin";

        private List<Rating> rating = new List<Rating>();

        public void Rate(Rating r) {
            rating.Add(r);

            SaveRating();
        }

        public IList<Rating> GetLastRatings() {
            LoadRating();

            return (from r in rating orderby r.Mark 
                descending select r).Take(5).ToList();
        }

        public void ClearRating() {
            rating.Clear();
            File.Delete(FileName);
        }
        
        private void SaveRating() {
            using (var fs = File.OpenWrite(FileName)) {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, rating);
            }
        }

        private void LoadRating() {
            if (File.Exists(FileName)) {
                using (var fs = File.OpenRead(FileName)) {
                    var bf = new BinaryFormatter();
                    if(fs.Length != 0) rating = (List<Rating>)bf.Deserialize(fs);
                }
            }
        }

        public float GetAverageRating() {
            float total = 0f;
            foreach (var r in rating) {
                total += r.Mark;
            }

            return (rating.Count == 0) ? total : total / rating.Count;
        }
    }
}