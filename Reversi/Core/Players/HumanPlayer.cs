using System;

namespace Reversi {
    public class HumanPlayer : Player {
        private GameLogic _gameLogic;
        
        public HumanPlayer(GameLogic gameLogic, String name, ConsoleColor color) {
            _gameLogic = gameLogic;
            Name = name;
            _color = color;
        }

        public override bool MakeTurn(ref Cell[,] gameBoard) {
            Console.ForegroundColor = _color;
            Console.Write(Name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write( ", enter the coordinates: ");
            Console.ForegroundColor = _color;
            
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
            
            Console.ResetColor();
            if(Char.IsDigit((char)(posX+48)) && Char.IsDigit((char)(posY+48))) {
                Console.ReadLine();
                
                if(!CheckAndPlace(posY, posX, CellTypes.Selected, ref gameBoard)) {
                    return MakeTurn(ref gameBoard);
                }
            }
            
            return true;
        }

    }
}