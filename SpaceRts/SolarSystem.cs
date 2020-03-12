using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static SpaceRts.Planet;

namespace SpaceRts
{
    public class SolarSystem
    {
        public Vector2 Position;
        public SolarSystem[] Connections;

        public Planet[] Planets;

        public SolarSystem(int seed, GraphicsDeviceManager graphics, Vector2 position, SolarSystem[] connections, int numbnerOfPlanets)
        {
            Position = position;
            Connections = connections;

            Planets = new Planet[numbnerOfPlanets];
            Random random = new Random(seed);

            for (int i = 0; i < numbnerOfPlanets; i++)
            {
                Planets[i] = new Planet(random.Next(), 6, 6, graphics, (PlanetTypes)random.Next(0, 5));
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            Planets[0].Draw(spriteBatch, graphics, camera);
            return;
            
            for (int i = 0; i < Planets.Length; i++)
            {
                Planets[i].Draw(spriteBatch, graphics, camera);
            }
        }
    }
}
