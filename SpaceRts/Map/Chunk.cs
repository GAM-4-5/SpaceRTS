using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static SpaceRts.Planet;

namespace SpaceRts.Map
{
    public class Chunk
    {
        public const float outerRadius = 10f;

        public const float innerRadius = outerRadius * 0.86602540378f;

        public static Vector3[] corners = {
            new Vector3(0f, outerRadius, 0f),
            new Vector3(innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(0f, -outerRadius, 0f),
            new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(0f, outerRadius, 0f),
        };

        int ChunkX, ChunkY, Width, Height;

        public BoundingBox BoudningBox;

        public VertexPositionColor[] Vertices;

        static Effect effect;
        public Chunk(int chunkX, int chunkY, int width, int height, int mapWidth, int mapHeight, float[] noiseMap, GraphicsDeviceManager graphics, PlanetTypes planetType, Color color)
        {
            Width = width;
            Height = height;

            Vertices = new VertexPositionColor[width * height * 6 * 3];

            float pX = chunkX * width * innerRadius * 2;
            float pY = chunkY * height * outerRadius * 3 / 2;
            // float pY = pX;
            float min = float.PositiveInfinity, max = float.NegativeInfinity;
            int t = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float tx = pX + (x + y * 0.5f - y / 2) * (innerRadius * 2f);
                    float ty = pY + y * (outerRadius * 1.5f);
                    float value = noiseMap[chunkY * mapWidth * width + ChunkX * width + y * width + x];
                    Vector3 pos = new Vector3(tx, ty, value * 100);
                    for (int i = 0; i < 6; i++)
                    {
                        min = Math.Min(min, value);
                        max = Math.Max(max, value);
                        int c  = (int)(MathHelper.Clamp(value, 0.1f, 0.9f) * 255);
                        Vertices[t] = new VertexPositionColor(pos + corners[i], new Color( c, 125, 125));
                        Vertices[t + 1] = new VertexPositionColor(pos + corners[i + 1], new Color(c, 125, 125));
                        Vertices[t + 2] = new VertexPositionColor(pos, new Color(c, 125, 125));

                        // Vertices[t] = new VertexPositionColor(pos + corners[i], color);
                        // Vertices[t + 1] = new VertexPositionColor(pos + corners[i + 1], color);
                        // Vertices[t + 2] = new VertexPositionColor(pos, color);
                        if(c > 254) Console.WriteLine(c);
                        if(c < 1) Console.WriteLine(c);
                        t += 3;
                    }
                }
            }
        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            //if(camera.Frustum.Contains(BoundingBox) == ContainmentType.Disjoint)
            //    return;

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    Vertices,
                    0,
                    Width * Height * 6);
            }
        }
    }
}