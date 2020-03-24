using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using SpaceRts.Map;

namespace SpaceRts
{
    public class Planet
    {
        public enum PlanetTypes { Magma, Desert, Rocky, Terran, Cold, Gas };

        public PlanetTypes PlanetType;
        private const int CHUNK_WIDTH = 16;
        private const int CHUNK_HEIGHT = 16;
        private static Effect effect;
        private static int Width, Height;
        public int Id;
        public static Color[][] GradientColors = new Color[][] { new Color[6] { Color.Yellow, Color.Orange, Color.Red, Color.DarkRed, Color.Gray, Color.Black } };
        public static float[][] GradientValues = new float[][] { new float[6] { 0, 0.1f, 0.2f, 0.3f, 0.5f, 0.7f } };

        private Noise2d Noise;

        private Map.Map Map;
        public Planet(int id, int seed, int width, int height, GraphicsDeviceManager graphics, PlanetTypes planetType)
        {
            Noise = new Noise2d(seed);
            var NoiseMap = Noise.GenerateNoiseMap(width * CHUNK_WIDTH, height * CHUNK_HEIGHT, 5f, 1f);

            Id = id;
            Width = width;
            Height = height;
            PlanetType = planetType;

            Map = new Map.Map(id, seed, width, height, NoiseMap, graphics, planetType);


            //using (var reader = new BinaryReader(File.Open("Content/Shaders/FogOfWar.xnb", FileMode.Open)))
            //{
            //    effect = new Effect(graphics.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
            //}

            // effect = new BasicEffect(graphics.GraphicsDevice);
        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        public static int FindGradientValueIndex(int search, float value)
        {
            int ret = 0;
            for (int i = 0; i < GradientValues[search].Length; i++)
            {
                if (value > GradientValues[search][i])
                    ret = i;
            }

            return ret;
        }

        float x = 0;
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            // effect.View = camera.ViewMatrix;
            // effect.Projection = camera.ProjectionMatrix;
            // effect.VertexColorEnabled = true;

            Map.Draw(spriteBatch, graphics, camera);
        }
    }
}

