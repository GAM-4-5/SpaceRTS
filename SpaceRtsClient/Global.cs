using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace SpaceRtsClient
{
    public static class Fonts
    {
        internal static void Load(ContentManager content)
        {
            throw new NotImplementedException();
        }
    }

    public static class Models
    {
        public static Model Zhus_Base;

        public static void Load(ContentManager content)
        {
            Zhus_Base = content.Load<Model>("Models/Zhus/Base");
        }
    }

    public static class Textures
    {
        public static Texture2D SmallGrainyNoise;

        public static void Load(ContentManager content)
        {
            SmallGrainyNoise = content.Load<Texture2D>("Shaders/Textures/noise");
        }
    }

    public static class Shaders
    {
        public static Effect MainMenu;
        public static Effect BlackHoleTransition;

        public static void Load(ContentManager content)
        {
            MainMenu = content.Load<Effect>("Shaders/MainMenu");
            MainMenu.Parameters["noiseTexture"].SetValue(Textures.SmallGrainyNoise);
            MainMenu.Parameters["planetTexture"].SetValue(content.Load<Texture2D>("Textures/FrozenLand"));

            BlackHoleTransition = content.Load<Effect>("Shaders/BlackHoleTransit");

        }

    }
}
