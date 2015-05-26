using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe.GameObjects;

namespace test
{
    [TestClass]
    public class GameBoardTest
    {
        [TestMethod]
        public void ValidPlay()
        {
            Player player = new Player();
            GameBoard board = new GameBoard();
            bool result = board.Play(1, 1, player);
            Assert.AreEqual(player, board[1, 1]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InvalidPlay()
        {
            Player player = Player.Human;
            Player player2 = Player.CPU;
            GameBoard board = new GameBoard();
            board.Play(1, 1, player);
            bool result = board.Play(1, 1, player2);
            Assert.AreEqual(player, board[1, 1]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsOverFullBoard()
        {
            Player player = Player.Human;
            Player[,] myBoard = new Player[3, 3] { 
                { player, player, player }, 
                { player, player, player }, 
                { player, player, player } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsTrue(board.IsOver());
        }

        [TestMethod]
        public void IsNotOverNotFullBoard()
        {
            Player player = Player.CPU;
            Player[,] myBoard = new Player[3, 3] { 
                { player, Player.None, player }, 
                { Player.None, Player.None, Player.None }, 
                { player, Player.None, player } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsFalse(board.IsOver());
        }

        [TestMethod]
        public void IsOverHorizontalWinner()
        {
            Player player = Player.Human;
            Player[,] myBoard = new Player[3, 3] { 
                { Player.None, Player.None, Player.None }, 
                { Player.None, Player.None, Player.None }, 
                { player, player, player } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsTrue(board.IsOver());
        }

        [TestMethod]
        public void IsOverVerticalWinner()
        {
            Player player = Player.Human;
            Player[,] myBoard = new Player[3, 3] { 
                { Player.None, Player.None, player }, 
                { Player.None, Player.None, player }, 
                { Player.None, Player.None, player } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsTrue(board.IsOver());
        }

        [TestMethod]
        public void IsOverMainDiagonalWinner()
        {
            Player player = Player.Human;
            Player[,] myBoard = new Player[3, 3] { 
                { player, Player.None, Player.None }, 
                { Player.None, player, Player.None }, 
                { Player.None, Player.None, player } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsTrue(board.IsOver());
        }

        [TestMethod]
        public void IsOverSecondaryDiagonalWinner()
        {
            Player player = Player.Human;
            Player[,] myBoard = new Player[3, 3] { 
                { Player.None, Player.None, player }, 
                { Player.None, player, Player.None }, 
                { player, Player.None, Player.None } 
            };
            GameBoard board = new GameBoard(myBoard);
            Assert.IsTrue(board.IsOver());
        }
    }
}
