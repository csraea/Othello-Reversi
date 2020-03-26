using System;

namespace NPuzzle.Entity {
    [Serializable]
    public class Score {

        public string Player { get; set; }

        public int Points { get; set; }
        
        public DateTime Time { get; set; }
    }
}
