using System;

namespace Reversi.Core.Service.Score {
    [Serializable]
    public class Score {

        public int Id { get; set; }

        public string Player { get; set; }

        public int Points { get; set; }
        
        public DateTime Time { get; set; }
    }
}
