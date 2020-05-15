using System;

namespace Reversi.Core.Service.Rating {
    [Serializable]
    public class Rating {

        public int Id { get; set; }

        public string Player { get; set; }

        public int Mark { get; set; }
        
    }
}