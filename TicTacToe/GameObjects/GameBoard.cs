using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.GameObjects
{
    public class GameBoard
    {
        private const byte BOARD_SIZE = 3;
        private Player[,] board;
        private Player currentPlayer;

        public static GameBoard BEST_X = new GameBoard(new Player[,] { { Player.Human, Player.Human, Player.Human }, { Player.Human, Player.Human, Player.Human }, { Player.Human, Player.Human, Player.Human } }, Player.Human);
        public static GameBoard BEST_O = new GameBoard(new Player[,] { { Player.CPU, Player.CPU, Player.CPU }, { Player.CPU, Player.CPU, Player.CPU }, { Player.CPU, Player.CPU, Player.CPU } }, Player.CPU);

        public GameBoard()
        {
            board = new Player[BOARD_SIZE, BOARD_SIZE];
        }

        public GameBoard(Player[,] board)
        {
            this.board = board;
        }

        public GameBoard(Player[,] board, Player currentPlayer)
        {
            this.board = board;
            this.currentPlayer = currentPlayer;
        }

        public int Width
        {
            get { return this.board.GetLength(0); }
        }

        public int Height
        {
            get { return this.board.GetLength(1); }
        }

        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }

        public Player this[int x, int y]
        {
            get { return this.board[x, y]; }
        }

        public void SetFirstPlayer(Player player)
        {
            currentPlayer = player;
        }

        public void NextPlayer()
        {
            switch (this.CurrentPlayer)
            {
                case Player.CPU:
                    currentPlayer = Player.Human;
                    break;
                case Player.Human:
                    currentPlayer = Player.CPU;
                    break;
                default:
                    throw new Exception("Trying to switch players while game has not started");
            }
        }

        public bool Play(int line, int column, Player player)
        {
            if (board[line, column] == Player.None)
            {
                board[line, column] = player;
                return true;
            }
            return false;
        }

        public bool IsOver() 
        {
            bool isFull = true;
            foreach (Player cell in board)
            {
                if (cell == Player.None)
                {
                    isFull = false;
                    break;
                }
            }

            bool hasWinner = (this.GetWinner() != Player.None);
            return isFull || hasWinner;
        }

        public Player GetWinner()
        {
            // check horizontal winner
            for (int line = 0; line < board.GetLength(0); line++)
            {
                bool allEqual = true;
                Player first = board[line, 0];
                if (first != Player.None)
                {
                    for (int column = 0; column < board.GetLength(1); column++)
                    {
                        if (first != board[line, column])
                        {
                            allEqual = false;
                            break;
                        }
                    }

                    if (allEqual)
                    {
                        return first;
                    }
                }
            }

            // check vertical winner
            for (int column = 0; column < board.GetLength(1); column++)
            {
                bool allEqual = true;
                Player first = board[0, column];
                if (first != Player.None)
                {
                    for (int line = 0; line < board.GetLength(0); line++)
                    {
                        if (first != board[line, column])
                        {
                            allEqual = false;
                            break;
                        }
                    }

                    if (allEqual)
                    {
                        return first;
                    }
                }
            }

            // check main diagonal winner
            Player winCandidate = board[0,0];
            bool diagonalWin = true;
            if (winCandidate != Player.None)
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    if (winCandidate != board[i, i])
                    {
                        diagonalWin = false;
                        break;
                    }
                }
                if (diagonalWin)
                {
                    return winCandidate;
                }
            }

            // check secondary winner
            winCandidate = board[board.GetLength(0) - 1, 0];
            diagonalWin = true;
            if (winCandidate != Player.None)
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    if (winCandidate != board[i, board.GetLength(1) - (i + 1)])
                    {
                        diagonalWin = false;
                        break;
                    }
                }
                if (diagonalWin)
                {
                    return winCandidate;
                }
            }

            // no winner
            return Player.None;
        }

        public GameBoard Clone()
        {
            Player[,] clonedMatrix = (Player[,])this.board.Clone();
            GameBoard newBoard = new GameBoard(clonedMatrix, this.currentPlayer);
            return newBoard;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    Player value = board[line, column];
                    if (value != Player.None)
                    {
                        builder.Append(value.ToString() + "\t");
                    }
                    else
                    {
                        builder.Append("-\t");
                    }
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
