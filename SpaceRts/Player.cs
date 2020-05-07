using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceRts
{
    public class Player
    {
        private int money = 100;
        private int resource = 0;

        public void pay(int price)
        {
            this.money -= price;
        }



     
    }
}
