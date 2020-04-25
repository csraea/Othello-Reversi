using System;

namespace Reversi {
    public class Cell {
        private byte weight;
        private CellTypes _type;
        private int x;
        private int y;

        public Cell(byte weight, CellTypes type, int x, int y) {
            this.weight = weight;
            _type = type;
            this.x = x;
            this.y = y;
        }

        public byte Weight {
            get => weight;
            set => weight = value;
        }

        public CellTypes Type {
            get => _type;
            set => _type = value;
        }

        public int X {
            get => x;
            set => x = value;
        }

        public int Y {
            get => y;
            set => y = value;
        }
    }
    
                
}