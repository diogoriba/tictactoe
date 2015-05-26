using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.AI;
using TicTacToe.GameObjects;

namespace test.AI
{
    [TestClass()]
    public class MinimaxTest
    {
        [TestMethod()]
        public void GeneratePossibleBoardsSimple()
        {
            GameBoard board = new GameBoard(new Player[,] {
                {Player.CPU, Player.Human, Player.CPU},
                {Player.CPU, Player.Human, Player.Human},
                {Player.Human, Player.CPU, Player.None}
            });

            IEnumerable<GameBoard> possibleBoards = Minimax.GetPossibleBoards(board, Player.Human);
            GameBoard[] actual = possibleBoards.ToArray();
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(Player.Human, actual[0][2, 2]);
        }

        [TestMethod()]
        public void GeneratePossibleBoardsComplex()
        {
            Player[,] matrix = new Player[,] {
                {Player.None, Player.Human, Player.CPU},
                {Player.CPU, Player.None, Player.Human},
                {Player.Human, Player.CPU, Player.None}
            };

            GameBoard board = new GameBoard(matrix);

            IEnumerable<GameBoard> possibleBoards = Minimax.GetPossibleBoards(board, Player.Human);
            GameBoard[] actual = possibleBoards.ToArray();
            Assert.AreEqual(3, actual.Length);
        }

        [TestMethod()]
        public void MinimaxTest1()
        {
            GameBoard board = new GameBoard(new Player[,] {
                {Player.CPU, Player.Human, Player.CPU},
                {Player.CPU, Player.Human, Player.Human},
                {Player.Human, Player.CPU, Player.None}
            });
            board.SetFirstPlayer(Player.Human);
            int actual = (int)Minimax.Play(board).GetWinner();
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void MinimaxTest2()
        {
            GameBoard board = new GameBoard(new Player[,] {
                {Player.Human, Player.Human, Player.CPU},
                {Player.CPU, Player.Human, Player.Human},
                {Player.Human, Player.CPU, Player.None}
            });
            board.SetFirstPlayer(Player.Human);
            int actual = (int)Minimax.Play(board).GetWinner();
            Assert.AreEqual(1, actual);
        }
    }
}
