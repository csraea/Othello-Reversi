using System;

namespace Reversi.Core.Service.Rating {
    [Serializable]
    public class Rating {
        public string Player { get; set; }

        public int Mark { get; set; }
        
    }
}