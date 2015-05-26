using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.GameObjects;

namespace TicTacToe.AI
{
    public class Minimax
    {
        public static Random random = new Random();
        public static bool easy = true;
        public static IEnumerable<GameBoard> GetPossibleBoards(GameBoard board, Player currentPlayer)
        {
            for (int line = 0; line < board.Height; line++)
            {
                for (int column = 0; column < board.Width; column++)
                {
                    if (board[line, column] == Player.None)
                    {
                        GameBoard boardClone = board.Clone();
                        boardClone.Play(line, column, currentPlayer);
                        yield return boardClone;
                    }
                }
            }
        }

        public static GameBoard Play(GameBoard board, Player current, int depth = 0)
        {
            if (easy && random.Next(0, 100) >= 50)
            {
                IEnumerable<GameBoard> branches = GetPossibleBoards(board, current);
                return branches.ElementAt(random.Next(0, branches.Count()));
            }
            else
            {
                int bestValue;
                List<GameBoard> bestBoards = new List<GameBoard>();
                IEnumerable<GameBoard> branches = GetPossibleBoards(board, current);
                if (current == Player.CPU)
                {
                    // CPU
                    bestValue = int.MaxValue;
                    bestBoards.Add(GameBoard.BEST_X);
                    foreach (GameBoard branch in branches)
                    {
                        int val = Run(branch, Player.Human, depth + 1);
                        if (val < bestValue)
                        {
                            bestBoards.Clear();
                        }
                        if (val <= bestValue)
                        {
                            bestValue = val;
                            bestBoards.Add(branch);
                        }
                    }
                }
                else
                {
                    // human
                    bestValue = int.MinValue;
                    bestBoards.Add(GameBoard.BEST_O);
                    foreach (GameBoard branch in branches)
                    {
                        int val = Run(branch, Player.CPU, depth + 1);
                        if (val > bestValue)
                        {
                            bestBoards.Clear();
                        }
                        if (val >= bestValue)
                        {
                            bestValue = val;
                            bestBoards.Add(branch);
                        }
                    }
                }

                return bestBoards[random.Next(0, bestBoards.Count)];
            }
        }

        private static int Run(GameBoard currentBoard, Player currentPlayer, int depth = 0)
        {
            if (currentBoard.IsOver() || depth >= currentBoard.Width * currentBoard.Height)
            {
                return (int)currentBoard.GetWinner();
            }
            int val;
            IEnumerable<GameBoard> branches = GetPossibleBoards(currentBoard, currentPlayer);
            if (currentPlayer == Player.CPU)
            {
                // CPU
                val = int.MaxValue;
                foreach (GameBoard board in branches)
                {
                    val = Math.Min(val, Run(board, Player.Human, depth + 1));
                }
            }
            else
            {
                // human
                val = int.MinValue;
                foreach (GameBoard board in branches)
                {
                    val = Math.Max(val, Run(board, Player.CPU, depth + 1));
                }
            }

            return val;
        }
    }
}
