using System;

namespace Reversi {
    public class HumanPlayer : Player {
        public int[] MakeTurn() {
            int[] coord = {0, 0};
            
            Console.Write("Please, enter the coordinates: ");
            char posY = Convert.ToChar(Console.Read());
            if (posY == 'Q' || posY == 'q') {
                coord[0] = -1;
                return coord;
            }
            Console.Read();
            char posX = Convert.ToChar(Console.Read());
            if(Char.IsDigit(posX) && Char.IsDigit(posY)) {
                coord[0] = posY - 48;
                coord[1] = posX - 48;
                Console.Read();
                return coord;
            }
            
            // here is necessary to clean input buffer
            
            return MakeTurn();
        }

    }
}