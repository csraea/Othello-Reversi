using System;

namespace Reversi {
    class Program {

        static void Main(string[] args) {
            
            UI ui = new ConsoleUI();
            byte gamemode = ui.RenderWelcomeMenu();
            if(gamemode == 0) Environment.Exit(0); 
            
            GameLogic gameLogic = new GameLogic((gamemode == 1) ? (byte) 6 : ui.GetBoardSize(), gamemode, ui);
            gameLogic.StartGame();
            
            ClearBuffer();
        }

        public static void ClearBuffer() {
            while (Console.KeyAvailable) {
                Console.ReadKey(false);
            }

            Console.ReadKey();
        }
    }
}