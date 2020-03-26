using System;
using System.Security.Cryptography.X509Certificates;
using Reversi.Core.Service.Comments;
using Service;

namespace Reversi {
    public interface UI {

        byte RenderWelcomeMenu();
        byte GetBoardSize();
        int GetChoice();
        void RenderRules();
        void RenderCredits();
        void RenderTitle();
        void RenderGameOptions();
        void DisplayGame(Player humanPlayer, Player secondPlayer, Cell[,] gameBoard, byte boardSize);
        void Exit();
        String GetName();
        void PrintScores(IScoreService scoreService);
        public void PrintComments(ICommentService commentService);
        void Think();
        String GetComment(Player player);
        sbyte Restart();
    }
}