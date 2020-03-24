using System;
using System.Collections.Generic;
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

        public static Tuple<int, int>[] AdjentsEven = {
            new Tuple<int, int>(0, 1),
            null,
            null,
            null,
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(-1,1),
        };

        BasicEffect testEffect;
        public static Tuple<int, int>[] AdjentsOdd = {
            new Tuple<int, int>(1, 1),
            null,
            null,
            null,
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(0, 1),
        };

        public static Color[] CornerColors = {
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Black,
            Color.Aqua
        };

        int ChunkX, ChunkY, Width, Height;

        public static RasterizerState TerrainRasterizerState = new RasterizerState()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.CullCounterClockwiseFace,
        };

        public BoundingBox BoundingBox;

        public VertexBuffer Vertices;
        public IndexBuffer Indicies;

        static Effect effect;
        public Chunk(int chunkX, int chunkY, int width, int height, int mapWidth, int mapHeight, float[] noiseMap, GraphicsDeviceManager graphics, PlanetTypes planetType)
        {
            Width = width;
            Height = height;

            ChunkX = chunkX;
            ChunkY = chunkY;

            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            VertexPositionColor[] vertices;

            float pX = chunkX * width * innerRadius * 2;
            float pY = chunkY * height * outerRadius * 3 / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float tx = pX + (x + y * 0.5f - y / 2) * (innerRadius * 2f);
                    float ty = pY + y * (outerRadius * 1.5f);

                    float value = noiseMap[(y + chunkY * height) * width * mapWidth + x + chunkX * width];

                    int color = (int)(MathHelper.Clamp(value, 0.1f, 0.9f) * 255);

                    Vector3 pos = new Vector3(tx, ty, value * 50);
                    for (int i = 0; i < 6; i++)
                    {
                        Vector3 c1 = pos + corners[i];
                        Vector3 c2 = pos + corners[i + 1];

                        if (i == 0 || i == 4 || i == 5)
                        {
                            int ax = y % 2 == 0 ? AdjentsEven[i].Item1 : AdjentsOdd[i].Item1;
                            int ay = y % 2 == 0 ? AdjentsEven[i].Item2 : AdjentsOdd[i].Item2;
                            int nx = x + ax;
                            int ny = y + ay;

                            float tx2 = pX + (nx + ny * 0.5f - ny / 2) * (innerRadius * 2f);
                            float ty2 = pY + ny * (outerRadius * 1.5f);

                            int noiseMapIndex = (ny + chunkY * height) * width * mapWidth + nx + chunkX * width;
                            if (noiseMapIndex >= 0 && noiseMapIndex < noiseMap.Length)
                            {
                                float value2 = noiseMap[noiseMapIndex];
                                int color2 = (int)(MathHelper.Clamp(value2, 0.1f, 0.9f) * 255);

                                Vector3 pos2 = new Vector3(tx2, ty2, value2 * 50);

                                Vector3 c3 = pos2 + corners[(i + 4) % 6];
                                Vector3 c4 = pos2 + corners[(i + 3) % 6];

                                positions.Add(c1); 
                                positions.Add(c2);
                                positions.Add(c3);
                                colors.Add(new Color(color, 125, 125)); 
                                colors.Add(new Color(color, 125, 125));
                                colors.Add(new Color(color2, 125, 125));


                                positions.Add(c4); 
                                positions.Add(c3);
                                positions.Add(c2);
                                colors.Add(new Color(color2, 125, 125)); 
                                colors.Add(new Color(color2, 125, 125));
                                colors.Add(new Color(color, 125, 125));

                            }

                        }
                        positions.Add(c1); 
                        positions.Add(pos);
                        positions.Add(c2);


                        colors.Add(new Color(color, 225, 225));
                        colors.Add(new Color(color, 225, 225));
                        colors.Add(new Color(color, 225, 225));

                    }
                }
            }

            positions.Reverse();
            colors.Reverse();
            vertices = new VertexPositionColor[positions.Count];
            Indicies = new IndexBuffer(graphics.GraphicsDevice, IndexElementSize.ThirtyTwoBits, positions.Count, BufferUsage.WriteOnly);
            int[] indicies = new int[positions.Count];

            for (int i = 0; i < positions.Count; i++)
            {
                vertices[i] = new VertexPositionColor(positions[i], colors[i]);
                indicies[i] = i;
            }

            Vertices = new VertexBuffer(graphics.GraphicsDevice, VertexPositionColor.VertexDeclaration, positions.Count, BufferUsage.WriteOnly);
            Vertices.SetData(vertices);

            Indicies.SetData(indicies);

            BoundingBox = BoundingBox.CreateFromPoints(positions);

            testEffect = new BasicEffect(graphics.GraphicsDevice);
        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            if (camera.Frustum.Contains(BoundingBox) == ContainmentType.Disjoint)
                return;

            Space.ChunksDrawn++;

            effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);


            graphics.GraphicsDevice.RasterizerState = TerrainRasterizerState;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();
            }

            graphics.GraphicsDevice.SetVertexBuffer(Vertices);
            graphics.GraphicsDevice.Indices = Indicies;
            graphics.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                Indicies.IndexCount / 3);
        }
    }
}