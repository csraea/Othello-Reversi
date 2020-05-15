using System;

namespace Reversi {
    
    [Serializable]
    public enum CellTypes {
        Free,
        Usable,
        Player1,
        Player2,
        Neighbouring,
        Selected
    }
}