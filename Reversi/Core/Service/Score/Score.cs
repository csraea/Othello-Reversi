using System;

namespace NPuzzle.Entity {
    [Serializable]
    public class Score {
        public int Id { get; set; }

        public string Player { get; set; }

        public int Points { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}
