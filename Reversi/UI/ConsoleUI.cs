using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Reversi.Core.Players;
using Reversi.Core.Service.Comments;
using Reversi.Core.Service.Rating;
using Service;

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
            String[] phrases = new[] {
                "Hmm...",
                "I'm thinking...",
                "Good move!",
                "Hah, Bustard!!",
                "I'm afraid you gonna lose.",
                "FUCK YOUR FUCKIN' FUCK!",
                "I feel sadness about that.",
                "I'm confused. Haha! Joke."
            };
            
            Console.Write("Jack's opinion: ");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var letter in phrases[new Random().Next(7)]) {
                Console.Write(letter);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            
            Console.ResetColor();
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
                    return 'X';
                case 3 : 
                    Console.ForegroundColor = ConsoleColor.Red;
                    return 'O';
            }

            return ' ';
        }

        public void DisplayGame(Player humanPlayer, Player secondPlayer, Cell[,] gameBoard, byte boardSize) {
            Console.ForegroundColor = humanPlayer._color;
            Console.Write(humanPlayer.Name + " : " + humanPlayer.GetScore(gameBoard, CellTypes.Player1, boardSize));
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(" | ");
            Console.ForegroundColor = secondPlayer._color;
            Console.Write(secondPlayer.Name + " : " + secondPlayer.GetScore(gameBoard, CellTypes.Player2, boardSize));
            Console.ResetColor();
            Console.WriteLine();
            
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
                Thread.Sleep(200);
            }

            Console.WriteLine("\nLoser.");
            Thread.Sleep(230);
            Console.Clear();
        }

        public String GetName() {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Your name: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            return Console.ReadLine();
        }
        
        public void PrintScores(IScoreService scoreService) {
            Console.WriteLine("Top scores of the session:");
            foreach (var score in scoreService.GetTopScores()) {
                if(score.Player.Equals("Handsome Jack")) Console.WriteLine("{0} : {1}\t{2}", score.Player, score.Points, score.Time.ToString("F"));
                else Console.WriteLine("{0} : {1}\t\t{2}", score.Player, score.Points, score.Time.ToString("F"));
            }
        }
        
        public void PrintComments(ICommentService commentService) {
            Console.WriteLine("-------------------------");
            Console.WriteLine("Recent comments:");
            foreach (var comment in commentService.GetLastComments()) { 
                Console.WriteLine("{0}: {1}\t({2})", comment.Player, comment.Text, comment.Time.ToString("G"));
            }
        }

        public String GetComment(Player player) {
            if (!player.Name.Equals("Handsome Jack")) {
                Console.WriteLine("Leave your comment, hero!");
                Console.Write(player.Name + ": ");
            }
            return player.Name.Equals("Handsome Jack")
                ? "What a fantastic game! I adore it! I'M THE BEST PLAYER EVER!!!"
                : GetCommentContent();
        }

        private string GetCommentContent() {
            string input = Console.ReadLine();
            if (input.Length.Equals(0)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Comment cannot be empty: ");
                Console.ResetColor();
                return GetCommentContent();
            }

            return input;
        }

        public sbyte Restart() {
            Console.ResetColor();
            Console.Write("Play again? [y/n]:  ");
            String answer = Console.ReadLine();
            foreach (var symbol in answer) {
                if (symbol.Equals('y') || symbol.Equals('Y')) return 0;
            }

            return -1;
        }

        public void PrintRating(IRatingService ratingService) {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("World game rating: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(ratingService.GetAverageRating().ToString("#.00"));
            Console.WriteLine();
            Console.ResetColor();
        }

        public int GetMark() {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Rate us: ");
            return CheckMark();
        }

        private int CheckMark() {
            int result = -1;
            string input = Console.ReadLine();
            bool success = true;
            try {
                result = Convert.ToInt32(input);
            }
            catch (FormatException) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input!");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Try again: ");
                success = false;
            }
            catch (OverflowException) {
                if (success) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input!");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("Try again: ");
                }

                success = false;
            }

            if (result < 0 || result > 10) {
                if (success) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input!");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;;
                    Console.Write("Try again: ");
                }

                success = false;
            }
            
            return (!success) ? CheckMark() : result;
        }
    }
}