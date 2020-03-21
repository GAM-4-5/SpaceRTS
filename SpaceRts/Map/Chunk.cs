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

        public BoundingBox BoundingBox;

        public VertexPositionColor[] Vertices;

        static Effect effect;
        public Chunk(int chunkX, int chunkY, int width, int height, int mapWidth, int mapHeight, float[] noiseMap, GraphicsDeviceManager graphics, PlanetTypes planetType, Color color)
        {
            Width = width;
            Height = height;

            ChunkY = chunkY;


            Vertices = new VertexPositionColor[width * height * 6 * 3];

            Vector3[] positions = new Vector3[width * height * 6 * 3];

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

                    float value = noiseMap[(y + chunkY * height) * width * mapWidth + x + chunkX * width];

                    Vector3 pos = new Vector3(tx, ty, value * 100);
                    for (int i = 0; i < 6; i++)
                    {
                        positions[t] = pos + corners[i];
                        positions[t + 1] = pos + corners[i + 1];
                        positions[t + 2] = pos;
                        t += 3;

                    }
                }
            }

            t = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = noiseMap[(y + chunkY * height) * width * mapWidth + x + chunkX * width];
                    int c = (int)(MathHelper.Clamp(value, 0.1f, 0.9f) * 255);

                    for (int i = 0; i < 6; i++)
                    {
                        Vertices[t] = new VertexPositionColor(positions[t], new Color(c, 125, 125));
                        Vertices[t + 1] = new VertexPositionColor(positions[t + 1], new Color(c, 125, 125));
                        Vertices[t + 2] = new VertexPositionColor(positions[t + 2], new Color(c, 125, 125));

                        t += 3;
                    }
                }
            }

            BoundingBox = BoundingBox.CreateFromPoints(positions);


        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            if(camera.Frustum.Contains(BoundingBox) == ContainmentType.Disjoint)
                return;

            Space.ChunksDrawn++;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineStrip,
                    Vertices,
                    0,
                    Width * Height * 6);
            }
        }
    }
}