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
    }
}