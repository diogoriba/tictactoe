using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.GameObjects
{
    public enum Player
    {
        None = 0,
        Human = 1,
        CPU = -1
    }

    public class PlayerUtils
    {
        public static Player Next(Player current)
        {
            return (Player)(((int)current) * -1);
        }
    }
}
