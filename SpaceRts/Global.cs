using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
namespace SpaceRts
{
    public static class Global
    {
        public static int SelectedSolarSystem;
        public static int SelectedPlanet;

        public static Camera Camera;

        public static MouseState MouseState;
        public static KeyboardState KeyboardState;

        public static Ray ClickRay;
    }

    public static class Models
    {
        public static Model Base;
        public static Model[] Ores;

        public static void Load(ContentManager content)
        {
            Ores = new Model[]{ content.Load<Model>("Models/Ores/Green") , content.Load<Model>("Models/Ores/Blue") , content.Load<Model>("Models/Ores/Red") };
        }
    }
}
