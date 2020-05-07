using SpaceRts.Map;
using System;
namespace SpaceRts.Structures
{
    public class Base
    {
        public int x;
        public int y;
        private int cost;
        private int prod;
        public Cell cell;
        public Player player;
        //private model 
        //wpublic int [] ;
        public Base(int x, int y, Cell cell, int cost, int prod, Player player)
        {
            
            this.x = x;
            this.y = y;
            this.cell = cell;
            this.cost = cost;
            this.prod = prod;
            this.player = player;
         }

        public void Build()
        {
            player.pay(cost);
            //cell.draw(model);
        }

        public int Production()
        {
            return this.prod;
        }


             
        
    }
}
