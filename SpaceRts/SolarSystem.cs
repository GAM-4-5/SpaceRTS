using System;
using Map;
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
                PlanetTypes pt = (PlanetTypes)random.Next(0, 5);
                PlanetSizes ps = (PlanetSizes)random.Next(0, 2);
                Planets[i] = new Planet(i, random.Next(), graphics, pt, ps);
            }
        }

        public void Update()
        {
            Planets[Global.SelectedPlanet].SelectedUpdate();
            for (int i = 0; i < Planets.Length; i++)
            {
                if(i != Global.SelectedPlanet)
                    Planets[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            Planets[Global.SelectedPlanet].Draw(spriteBatch, graphics, camera);
            return;
            
            for (int i = 0; i < Planets.Length; i++)
            {
                Planets[i].Draw(spriteBatch, graphics, camera);
            }
        }
    }
}
