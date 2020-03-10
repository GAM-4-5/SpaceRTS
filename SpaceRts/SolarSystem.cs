using System;
using Microsoft.Xna.Framework;

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
    }
}
