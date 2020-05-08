using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceRts.Map;
using System;
namespace SpaceRts.Structures
{
    public class Structure
    {
        public enum StructureTypes
        {
            Base,
            Ore,
            Mine,
        }

        private int Cost;
        private int Prod;
        public Cell Cell;
        public Player Player;

        Model Model;
        StructureTypes StructureType;
        //private model 
        //wpublic int [] ;
        public Structure(StructureTypes structureType, Model model, Cell cell, int cost, int prod, Player player)
        {
            Cell = cell;
            Cost = cost;
            Prod = prod;
            Player = player;
        }

        public Structure(Cell cell, Model model, StructureTypes structureType)
        {
            Cell = cell;
            Model = model;
            StructureType = structureType;
        }

        public void Build()
        {

        }

        public int Production()
        {
            return 0;
        }


        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            if(Cell != null)
            {
                Cell.DrawOnTop(Model, spriteBatch, graphics, camera);
            }
        }
    }
}
