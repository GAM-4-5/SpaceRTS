using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceRts
{
    public class SolarSystem
    {
        public Vector2 Position;
        public SolarSystem[] Connections;

        public SolarSystem(Vector2 position, SolarSystem[] connections)
        {
            Position = position;
            Connections = connections;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {

        }
    }
}
