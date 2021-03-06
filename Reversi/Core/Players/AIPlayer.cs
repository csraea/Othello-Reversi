using System;
using Reversi.Core.Players.AIBehaviours;

namespace Reversi.Core.Players {

    [Serializable]
    public class AIPlayer : Player {
        public Behaviour ai { get; set; }
        
        public AIPlayer(Behaviour.Mode mode, GameLogic gameLogic, ConsoleColor color) {
            Name = "Handsome Jack";
            ai = CreateAI(mode, gameLogic, this);
            _color = color;
        }

        public AIPlayer(Behaviour.Mode mode, GameLogic gameLogic, string color)
        {
            Name = "Handsome Jack";
            ai = CreateAI(mode, gameLogic, this);
            strColor = color;
        }

        public override bool MakeTurn(ref Cell[,] gameBoard) {
            ai.Sorcery(ref gameBoard);
            return true;
        }


        public override bool MakeAdvancedTurn(int y, int x, Cell[,] gameBoard)
        {
            return MakeTurn(ref gameBoard);
        }


        private Behaviour CreateAI(Behaviour.Mode mode, GameLogic gameLogic, Player player) {
            switch (mode) {
                case Behaviour.Mode.Easy:
                    return new AIEasy(gameLogic, player);
                case Behaviour.Mode.Medium:
                    return new AIMedium(gameLogic, player);
                case Behaviour.Mode.Difficult:
                    return new AIDifficult(gameLogic, player);
                case Behaviour.Mode.Impossible:
                    return new AIImpossible(gameLogic, player);
            }

            return null;
            
            return CreateAI((Behaviour.Mode) new Random().Next(3), gameLogic, player);
        }
    }
}








































//         
//                 public static int Evaluation(byte[,] board)
//         {
//             // Heuristical evaluation function that quantifies the attractiveness of a board,
//             // relative to the black player.
//             int evaluation = 0;
//             int blackScore = GetScore(board, Black);
//             int whiteScore = GetScore(board, White);
//             int blackMobility = GetValidMoves(board, Black).Count;
//             int whiteMobility = GetValidMoves(board, White).Count;
//             if (blackScore == 0)
//             {
//                 return -200000;
//             }
//             else if (whiteScore == 0)
//             {
//                 return 200000;
//             }
//             if (blackScore + whiteScore == 64 || blackMobility + whiteMobility == 0)
//             {
//                 if (Black < whiteScore)
//                 {
//                     return -100000 - whiteScore + blackScore;
//                 }
//                 else if (blackScore > whiteScore)
//                 {
//                     return 100000 + blackScore - whiteScore;
//                 }
//                 else
//                 {
//                     return 0;
//                 }
//             }
//             evaluation += blackScore - whiteScore;
//             if (blackScore + whiteScore > 55)
//             {
//                 return (blackScore - whiteScore);
//             }
//             evaluation += (blackMobility - whiteMobility) * 10;
//             evaluation += (CountCorners(board, Black) - CountCorners(board, White)) * 100;
//             return evaluation;
//         }
//
//         public static int MinimaxAlphaBeta(byte[,] board, int depth, int a, int b, byte tile, bool isMaxPlayer)
//         {
//             // The heart of our AI. Minimax algorithm with alpha-beta pruning to speed up computation.
//             // Higher search depths = greater difficulty.
//             if (depth == 0 || GetWinner(board) != Empty)
//             {
//                 return Evaluation(board);
//             }
//             int bestScore;
//             if (isMaxPlayer) bestScore = int.MinValue;
//             else bestScore = int.MaxValue;
//             List<Tuple<int, int>> validMoves = GetValidMoves(board, tile);
//             if (validMoves.Count > 0)
//             {
//                 foreach (Tuple<int, int> move in validMoves)
//                 {
//                     byte[,] childBoard = board.Clone() as byte[,];
//                     MakeMove(childBoard, move, tile);
//                     int nodeScore = MinimaxAlphaBeta(childBoard, depth - 1, a, b, OtherTile(tile), !isMaxPlayer);
//                     if (isMaxPlayer)
//                     {
//                         bestScore = Math.Max(bestScore, nodeScore);
//                         a = Math.Max(bestScore, a);
//                     }
//                     else
//                     {
//                         bestScore = Math.Min(bestScore, nodeScore);
//                         b = Math.Min(bestScore, b);
//                     }
//                     if (b <= a)
//                     {
//                         break;
//                     }
//                 }
//             }
//             else
//             {
//                 return MinimaxAlphaBeta(board, depth, a, b, OtherTile(tile), !isMaxPlayer);
//             }
//             return bestScore;
//         }
//
//         public static Tuple<int, int> GetAIMove(byte[,] board, int depth, byte tile)
//         {
//             // The "convienence" function that allows us to use our AI algorithm.
//             List<Tuple<int, int>> validMoves = GetValidMoves(board, tile);
//             validMoves = validMoves.OrderBy(a => rng.Next(-10, 10)).ToList();
//             if (validMoves.Count > 0)
//             {
//                 int bestScore;
//                 if (tile == Black)
//                 {
//                     bestScore = int.MinValue;
//                 }
//                 else if (tile == White)
//                 {
//                     bestScore = int.MaxValue;
//                 }
//                 else
//                 {
//                     return null;
//                 }
//                 Tuple<int, int> bestMove = validMoves[0];
//                 if (GetScore(board, Black) + GetScore(board, White) > 55)
//                 {
//                     depth = 100;
//                 }
//                 foreach (Tuple<int, int> move in validMoves)
//                 {
//                     byte[,] childBoard = board.Clone() as byte[,];
//                     MakeMove(childBoard, move, tile);
//                     int nodeScore;
//                     if (tile == Black)
//                     {
//                         nodeScore = MinimaxAlphaBeta(childBoard, depth - 1, int.MinValue, int.MaxValue, OtherTile(tile), false);
//                         if (nodeScore > bestScore)
//                         {
//                             bestScore = nodeScore;
//                             bestMove = move;
//                         }
//                     }
//                     else
//                     {
//                         nodeScore = MinimaxAlphaBeta(childBoard, depth - 1, int.MinValue, int.MaxValue, OtherTile(tile), true);
//                         if (nodeScore < bestScore)
//                         {
//                             bestScore = nodeScore;
//                             bestMove = move;
//                         }
//                     }
//                 }
//                 return bestMove;
//             }
//             return null;
//         }
//
//         public static Tuple<int, int> GetMaxScoreMove(byte[,] board, byte tile)
//         {
//             // A much more naive version of AI compared to GetAIMove or MinimaxAlphaBeta.
//             // This function just selects the move with highest score.
//             // This is the "easy" AI mode.
//             List<Tuple<int, int>> validMoves = GetValidMoves(board, tile);
//             if (validMoves.Count > 0)
//             {
//                 int bestScore = int.MinValue;
//                 Tuple<int, int> bestMove = validMoves[0];
//                 foreach (Tuple<int, int> move in validMoves)
//                 {
//                     byte[,] childBoard = board.Clone() as byte[,];
//                     MakeMove(childBoard, move, tile);
//                     int nodeScore = GetScore(board, tile) * rng.Next(-5, 5);
//                     if (nodeScore > bestScore)
//                     {
//                         bestScore = nodeScore;
//                         bestMove = move;
//                     }
//                 }
//                 return bestMove;
//             }
//             return null;
//         }
//
//     }
// }