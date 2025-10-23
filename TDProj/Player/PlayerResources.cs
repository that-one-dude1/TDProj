using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TDProj
{
    public class PlayerResources
    {
        public int Money { get; set; }
        public int Lives { get; set; }

        public PlayerResources(int startingMoney = 500, int lives = 10)
        {
            Money = startingMoney;
            Lives = lives;
        }

        public bool Spend(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                return true;
            }
            return false;
        }

        public void Earn(int amount)
        {
            Money += amount;
        }

        public void LoseLife(int amount)
        {
            Lives -= amount;
            if (Lives < 0)
                Lives = 0;
        }

        public void Reset()
        {
            Money = 500;
            Lives = 10;
        }
    }
}
