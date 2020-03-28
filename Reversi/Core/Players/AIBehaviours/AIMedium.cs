using System.ComponentModel;

namespace Reversi.Core.Players.AIBehaviours {
    public class AIMedium : Behaviour {

        public override void Sorcery(ref Cell[,] gameBoard) {

        }

        private int Heruistic(CellTypes whoseTurn, GameLogic logic) {
            CellTypes opponent = CellTypes.Player2;
            if (whoseTurn == CellTypes.Player2) opponent = CellTypes.Player1;
            return Player.GetScore(logic.GameBoard, whoseTurn, logic.boardSize) -
                   Player.GetScore(logic.GameBoard, opponent, logic.boardSize);
        }


        // May be optimized. Deep copy is required, not shallow
        private Cell[,] GetBoardCopy(Cell[,] gameBoard) {
            Cell[,] copy = new Cell[Logic.boardSize, Logic.boardSize];
            for (int i = 0; i < Logic.boardSize; i++) {
                for (int j = 0; j < Logic.boardSize; j++) {
                    copy[i, j].Type = gameBoard[i, j].Type;
                    copy[i, j].Weight = gameBoard[i, j].Weight;
                    copy[i, j].X = gameBoard[i, j].X;
                    copy[i, j].Y = gameBoard[i, j].Y;
                }
            }

            return copy;
        }

        private void MinimaxDecision(GameLogic logic, CellTypes whoseTurn, ref int bestY, ref int bestX) {
            // Necessary? Already performs the action in the main game cycle
            CellTypes opponent = CellTypes.Player1;
            if (whoseTurn == CellTypes.Player1) opponent = CellTypes.Player2;
            logic.DetermineUsableCells(whoseTurn, opponent);

            GetPossibleMoves(logic.GameBoard, ref PossibleMoves);
            if (PossibleMoves.Count == 0) {
                bestX = -1;
                bestY = -1;
            }
            else {
                int bestValue = -99999;
                for (int i = 0; i < PossibleMoves.Count; i++) {

                    GameLogic tempLogic = new GameLogic(Logic.boardSize);
                    tempLogic.GameBoard = GetBoardCopy(logic.GameBoard);

                    //make the move
                    tempLogic.GameBoard[PossibleMoves[i].Y, PossibleMoves[i].X].Type = whoseTurn;

                    int tempValue = MinimaxValue(tempLogic, 1, whoseTurn, opponent);
                    if (tempValue > bestValue) {
                        bestValue = tempValue;
                        bestX = PossibleMoves[i].X;
                        bestY = PossibleMoves[i].Y;
                    }
                }
            }
        }

        private int MinimaxValue(GameLogic logic, int depth, CellTypes originalTurn, CellTypes currentTurn) {
            
            logic.DetermineUsableCells(currentTurn, (currentTurn == CellTypes.Player1) ? CellTypes.Player2 : CellTypes.Player1);
            if (depth == 5 || !logic.IsGameWinnable()) {
                return Heruistic(originalTurn, logic);
            }
            
            GetPossibleMoves(logic.GameBoard, ref PossibleMoves);
            CellTypes opponent = CellTypes.Player1;
            if (currentTurn == CellTypes.Player1) opponent = CellTypes.Player2;
            
            // If no moves are available, pass the turn to the other player
            if (PossibleMoves.Count == 0) {
                return MinimaxValue(logic, depth + 1, originalTurn, opponent);
            } else {
                int bestValue = -99999; // for finding max
                if (originalTurn != currentTurn) bestValue = 99999;
                // Try every single move
                for (int i = 0; i < PossibleMoves.Count; i++) {
                    GameLogic tempLogic = new GameLogic(logic.boardSize);
                    tempLogic.GameBoard = GetBoardCopy(logic.GameBoard);
                    
                    // Make move
                    tempLogic.GameBoard[PossibleMoves[i].Y, PossibleMoves[i].X].Type = currentTurn;
                    
                    // Exact recursive call
                    int newValue = MinimaxValue(tempLogic, depth + 1, originalTurn, currentTurn);
                    
                    // Remember the best move
                    if (originalTurn == currentTurn) {
                        // Remember Max if it's the originator's turn
                        if (newValue > bestValue) bestValue = newValue;
                    } else {
                        // Remember Min if it's the opponent turn
                        if (newValue < bestValue) bestValue = newValue;
                    }
                }

                return bestValue;
            }
        }

        /*
         * int minimaxValue(char board[][8], char originalTurn, char currentTurn, int searchPly)
{
if ((searchPly == 5) || gameOver(board)) // Change to desired ply lookahead
{
return heuristic(board, originalTurn);
}
int moveX[60], moveY[60];
int numMoves;
char opponent = 'X';
if (currentTurn == 'X')
opponent = 'O';
getMoveList(board, moveX, moveY, numMoves, currentTurn);
if (numMoves == 0) // if no moves skip to next player's turn
{
return minimaxValue(board, originalTurn, opponent, searchPly + 1);
}
else
{
// Remember the best move
int bestMoveVal = -99999; // for finding max
if (originalTurn != currentTurn)
bestMoveVal = 99999; // for finding min
// Try out every single move
for (int i = 0; i < numMoves; i++)
{
// Apply the move to a new board
char tempBoard[8][8];
copyBoard(board, tempBoard);
makeMove(tempBoard, moveX[i], moveY[i], currentTurn);
// Recursive call
int val = minimaxValue(tempBoard, originalTurn, opponent,
searchPly + 1);
// Remember best move
if (originalTurn == currentTurn)
{
// Remember max if it's the originator's turn
if (val > bestMoveVal)
bestMoveVal = val;
}
else
{
// Remember min if it's opponent turn
if (val < bestMoveVal)
bestMoveVal = val;
}
}
return bestMoveVal;
}
return -1; // Should never get here
}
         */
    }
}