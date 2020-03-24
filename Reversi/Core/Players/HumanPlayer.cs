using System;

namespace Reversi {
    public class HumanPlayer : Player {
        private GameLogic _gameLogic;

        public HumanPlayer(GameLogic gameLogic) {
            _gameLogic = gameLogic;
        }

        public bool MakeTurn(ref Cell[,] gameBoard) {

            Console.Write("Please, enter the coordinates: ");
            char posY = Convert.ToChar(Console.Read());
            if (posY == 'Z' || posY == 'z') {
                return false;
            }
            Console.Read();
            char posX = Convert.ToChar(Console.Read());

            if (Char.IsLetter(posX) && Char.IsLetter(posY)) {
                posX = (char) (Char.ToUpper(posX) - 'A');
                posY = (char) (Char.ToUpper(posY) - 'A');
            } else {
                Console.ReadLine();
                return MakeTurn(ref gameBoard);
            }

            if(Char.IsDigit((char)(posX+48)) && Char.IsDigit((char)(posY+48))) {
                Console.ReadLine();
                _gameLogic.
                return coord;
            }

            Program.ClearBuffer();

            return MakeTurn(gameBoard);
        }

    }
}