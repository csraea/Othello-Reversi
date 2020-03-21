using System;

namespace Reversi {
    public class HumanPlayer : Player {
        public int[] MakeTurn() {
            int[] coord = {0, 0};

            Console.Write("Please, enter the coordinates: ");
            char posY = Convert.ToChar(Console.Read());
            if (posY == 'Z' || posY == 'z') {
                coord[0] = -1;
                return coord;
            }
            Console.Read();
            char posX = Convert.ToChar(Console.Read());

            Console.WriteLine("Y - " + (int)posY);
            Console.WriteLine("X - " + (int)posX);
            
            if (Char.IsLetter(posX) && Char.IsLetter(posY)) {
                posX = (char) (Char.ToUpper(posX) - 'A');
                posY = (char) (Char.ToUpper(posY) - 'A');
            } else {
                Console.ReadLine();
                return MakeTurn();
            }

            if(Char.IsDigit((char)(posX+48)) && Char.IsDigit((char)(posY+48))) {
                coord[0] = posY;
                coord[1] = posX;
                Console.ReadLine();
                return coord;
            }

            Program.ClearBuffer();

            return MakeTurn();
        }

    }
}