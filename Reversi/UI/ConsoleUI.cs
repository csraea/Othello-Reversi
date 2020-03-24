using System;
using System.Threading;

namespace Reversi {
    public class ConsoleUI : UI{
        
        public byte RenderWelcomeMenu() {
            RenderTitle();
            RenderGameOptions(); 
            Inputer:
            switch (GetChoice()) {
                case 1:
                    Console.Read();
                    return 1;
                case 2:
                    Console.Read();
                    return 2;
                case 3:
                    RenderRules();
                    Console.Read();
                    goto Inputer;
                case 4:
                    RenderCredits();
                    Console.Read();
                    goto Inputer;
                case 5:
                    return 0;
                default:
                    goto Inputer;
            }
        }
        
        public byte GetBoardSize() {
            Console.Write("Enter the board size: ");
            string input = Console.ReadLine();
            byte size = 8;
            bool success = true;
            try {
                size = Convert.ToByte(input);
            }
            catch (FormatException) {
                Console.WriteLine("Invalid input!");
                success = false;
            }
            catch (OverflowException) {
                Console.WriteLine("Invalid input!");
                success = false;
            }

            if ('A' + size - 1 > 'Z' || size % 2 != 0) {
                Console.WriteLine("Invalid input!");
                success = false;
            }
            
            return (!success) ? GetBoardSize() : size;
        }

        public int GetChoice() {
            Console.Write("Your choice: ");
            int input = Console.Read();
            switch (input) {
                case 1: case '1':
                    return 1;
                case 2: case '2':
                    return 2;
                case 3: case '3':
                    return 3;
                case 4: case '4':
                    return 4;
                case 5: case '5':
                    return 5;
                default: GetChoice();
                    break;
            }

            return 5;
        }

        public void RenderRules() {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("You might read them in the Internet :)");
            Console.WriteLine("---------------------------------------");
        }

        public void RenderCredits() {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Author: Alex Korotetskyi");
            Console.WriteLine("Email: csraea@gmail.com");
            Console.WriteLine("Telegram: @csraea");
            Console.WriteLine("Instagram: @korotetskiy_");
            Console.WriteLine("2020. All rights reserved ©");
            Console.WriteLine("--------------------------------");
        }

        public void RenderTitle() {
            Console.WriteLine("################################");
            Console.WriteLine("# Welcome to the game Reversi! #");
            Console.WriteLine("################################");
        }

        public void RenderGameOptions() {
            Console.WriteLine("1. Single-player");
            Console.WriteLine("2. Multi-player");
            Console.WriteLine("3. Rules");
            Console.WriteLine("4. About");
            Console.WriteLine("5. Exit");
            Console.WriteLine("--------------------------------");
        }

        public void Think() {
            
        }

        private char ParseOutput(int cellType) {
            switch (cellType) {
                case 0 :
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    return '.';
                case 1 : 
                    Console.ForegroundColor = ConsoleColor.Green;
                    return '+';
                case 2 : 
                    Console.ForegroundColor = ConsoleColor.Blue;
                    return 'O';
                case 3 : 
                    Console.ForegroundColor = ConsoleColor.Red;
                    return 'X';
            }

            return ' ';
        }

        public void DisplayGame(Player humanPlayer, Player secondPlayer, Cell[,] gameBoard, byte boardSize) {
            Console.ResetColor();
            Console.WriteLine("Current state of the game: " +
                              humanPlayer.GetScore(gameBoard, CellTypes.Player1, boardSize) + ":" +
                              secondPlayer.GetScore(gameBoard, CellTypes.Player2, boardSize));

            for (int i = 0; i < boardSize; i++) {
                if (i == 0) Console.Write("   ");
                Console.Write(" " + (char)(i + 'A'));
            }

            Console.Write("\n");

            for (int i = 0; i < boardSize * 2 + 3; i++) {
                if (i == 0) Console.Write("  ");
                if (i == 0 || i == boardSize * 2 + 2) Console.Write("+");
                else Console.Write("-");
            }

            Console.Write("\n");

            for (ushort i = 0; i < boardSize; i++) {
                Console.Write((char)(i + 'A') + " |");
                for (ushort j = 0; j < boardSize; j++) {
                    Console.Write(" " + ParseOutput((int) gameBoard[i, j].Type));
                    Console.ResetColor();
                }

                Console.ResetColor();
                Console.Write(" |\n");
            }

            for (int i = 0; i < boardSize * 2 + 3; i++) {
                if (i == 0) Console.Write("  ");
                if (i == 0 || i == boardSize * 2 + 2) Console.Write("+");
                else Console.Write("-");
            }

            Console.Write("\n");
        }

        public void Exit() {
            foreach (var VARIABLE in "Exiting...") {
                Console.Write(VARIABLE);
                Thread.Sleep(210);
            }

            Console.WriteLine("\nLoser.");
            Thread.Sleep(200);
            Console.Clear();
        }
    }
}