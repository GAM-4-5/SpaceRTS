using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    }
}
