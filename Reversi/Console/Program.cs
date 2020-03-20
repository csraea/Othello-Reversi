using System;
using System.ComponentModel;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Reversi {
    class Program {

        static void Main(string[] args) {
            //Program program = new Program();

            byte gamemode = RenderWelcomeMenu();
            if(gamemode == 0) Environment.Exit(0); 
            
            GameLogic gameLogic = new GameLogic((gamemode == 1) ? (byte) 8 : GetBoardSize(), gamemode);
            gameLogic.StartGame();

            clearBuffer();
        }

        public static byte GetBoardSize() {
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

            return (!success) ? GetBoardSize() : size;
        }

        public static byte RenderWelcomeMenu() {
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

        private static int GetChoice() {
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

        private static void RenderRules() {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("You might read them in the Internet :)");
            Console.WriteLine("---------------------------------------");
        }
        
        private static void RenderCredits() {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Author: Alex Korotetskyi");
            Console.WriteLine("Email: csraea@gmail.com");
            Console.WriteLine("Telegram: @csraea");
            Console.WriteLine("Instagram: @korotetskiy_");
            Console.WriteLine("2020. All rights reserved ©");
            Console.WriteLine("--------------------------------");
        }

        private static void RenderTitle() {
            Console.WriteLine("################################");
            Console.WriteLine("# Welcome to the game Reversi! #");
            Console.WriteLine("################################");
        }

        private static void RenderGameOptions() {
            Console.WriteLine("1. Single-player");
            Console.WriteLine("2. Multi-player");
            Console.WriteLine("3. Rules");
            Console.WriteLine("4. About");
            Console.WriteLine("5. Exit");
            Console.WriteLine("--------------------------------");
        }



        private static void clearBuffer() {
            while (Console.KeyAvailable) {
                Console.ReadKey(false);
            }

            Console.ReadKey();
        }
    }
}