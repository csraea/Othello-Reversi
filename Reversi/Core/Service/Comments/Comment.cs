using System;

namespace Reversi.Core.Service.Comments {
    [Serializable]
    public class Comment {

        public string Player { get; set; }
        
        public string Text { get; set; }

        public DateTime Time { get; set; }
    }
}