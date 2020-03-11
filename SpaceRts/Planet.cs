using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceRts
{
    public class Planet
    {
        public enum PlanetType { Magma, Dry, Rocky, EarthLike, Cold, Gas };

        private const int TILE_WIDTH = 1, TILE_DEPTH = 1;
        private static Effect effect;
        private static int Width, Height;
        private static VertexPositionColor[] Vertecies;

        private static Color[][] GradientColors = new Color[][] { new Color[] { Color.Yellow, Color.Orange, Color.Red, Color.DarkRed, Color.Gray } };
        private static float[][] GradientValues = new float[][] { new float[] { 0, 0.1f, 0.2f, 0.3f, 0.5f, 0.7f, 1f } };

        private Noise2d Noise;
        public Planet(int seed, int width, int height, GraphicsDeviceManager graphics, PlanetType planetType)
        {
            Noise = new Noise2d(seed);

            Width = width;
            Height = height;

            Vertecies = new VertexPositionColor[width * height * 6];
            var NoiseMap = Noise.GenerateNoiseMap(width, height, 100f, 1f);

            int i = 0;
            Random random = new Random(123);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value1 = NoiseMap[y * width + x];
                    float value2 = NoiseMap[Math.Min(y * width + x + 1, width * height - 1)];
                    float value3 = NoiseMap[Math.Min((y + 1) * width + x, width * height - 1)];
                    float value4 = NoiseMap[Math.Min((y + 1) * width + x + 1, width * height - 1)];

                    int al1 = 255;  //random.NextDouble() > 0.2 ? 0 : 255;
                    int al2 = 255;  //random.NextDouble() > 0.2 ? 0 : 255;


                    Color color1 = Color.FromNonPremultiplied(40, 40, (int)(value1 * 255), al1);
                    Color color2 = Color.FromNonPremultiplied(40, 40, (int)(value2 * 255), al1);
                    Color color3 = Color.FromNonPremultiplied(40, 40, (int)(value3 * 255), al1);
                    Color color4 = Color.FromNonPremultiplied(40, 40, (int)(value4 * 255), al1);

                    Color color1x = Color.FromNonPremultiplied(40, 40, (int)(value1 * 255), al2);
                    Color color2x = Color.FromNonPremultiplied(40, 40, (int)(value2 * 255), al2);
                    Color color3x = Color.FromNonPremultiplied(40, 40, (int)(value3 * 255), al2);
                    Color color4x = Color.FromNonPremultiplied(40, 40, (int)(value4 * 255), al2);

                    int cx = x * TILE_WIDTH;
                    int cy = y * TILE_DEPTH;

                    int nx = cx + TILE_WIDTH;
                    int ny = cy + TILE_DEPTH;
                    Color color = Color.Blue;//value > 10 ? value > 20 ? Color.SandyBrown: Color.Green : Color.Blue;

                    int a = 10;
                    Vertecies[i] = new VertexPositionColor(new Vector3(cx, cy, value1 * a), color1);
                    Vertecies[i + 1] = new VertexPositionColor(new Vector3(cx, ny, value3 * a), color3);
                    Vertecies[i + 2] = new VertexPositionColor(new Vector3(nx, cy, value2 * a), color2);

                    Vertecies[i + 3] = new VertexPositionColor(Vertecies[i + 1].Position, color3x);
                    Vertecies[i + 4] = new VertexPositionColor(new Vector3(nx, ny, value4 * a), color4x);
                    Vertecies[i + 5] = new VertexPositionColor(Vertecies[i + 2].Position, color2x);

                    i += 6;
                }
            }

            //using (var reader = new BinaryReader(File.Open("Content/Shaders/FogOfWar.xnb", FileMode.Open)))
            //{
            //    effect = new Effect(graphics.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
            //}

            //effect = new BasicEffect(graphics.GraphicsDevice);
        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        float x = 0;
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            //effect.View = camera.ViewMatrix;
            //effect.Projection = camera.ProjectionMatrix;
            //effect.VertexColorEnabled = true;

            effect.Parameters["Time"].SetValue(x);
            x += 0.07f;
            effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    Vertecies,
                    0,
                    Width * Height * 2);
            }
        }
    }
}

