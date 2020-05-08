using System;
using System.Collections.Generic;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Map.NoiseGenerator;
using static SpaceRts.Planet;

namespace SpaceRts.Map
{
    public class Chunk
    {
        BasicEffect testEffect;

        public static Color[] CornerColors = {
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Black,
            Color.Aqua
        };

        int ChunkX, ChunkY, Width, Height;

        public static Vector3 IREGULARITY_VECTOR = new Vector3(0, 0, 0);
        public static Vector3 HEIGHT_SIZE_FALLOFF = new Vector3(1.5f, 1.5f, 0);

        public static RasterizerState TerrainRasterizerState = new RasterizerState()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
        };

        public BoundingBox BoundingBox;

        public VertexPositionColorTexture[][] Vertices;
       // public Texture2D[] Textures;
        public IndexBuffer Indicies;

        static Effect effect;

        static Texture2D texture;

        Cell[,] Cells;

        List<BiomeType> Biomes;
        PlanetTypes planetType;
        public Chunk(int chunkX, int chunkY, int width, int height, int mapWidth, int mapHeight, NoiseGenerator noiseGenerator, GraphicsDeviceManager graphics, PlanetTypes planetType)
        {
            Width = width;
            Height = height;
            this.planetType = planetType;

            Cells = new Cell[height, width];

            ChunkX = chunkX;
            ChunkY = chunkY;

            List<List<Vector3>> positions = new List<List<Vector3>>();
            List<List<Vector2>> texturePositions = new List<List<Vector2>>();
            List<List<Color>> colors = new List<List<Color>>();
            Biomes = new List<BiomeType>();

            int biomesTotal = 0;
            Dictionary<int, int> biomesToPos = new Dictionary<int, int>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cells[y, x] = new Cell();

                    int _cellGlobalIndex = (y + chunkY * height) * width * mapWidth + x + chunkX * width;

                    BiomeType biome = noiseGenerator.BiomeAtIndex(_cellGlobalIndex);

                    int pushIndex = -1;
                    if (!biomesToPos.TryGetValue((int)biome, out pushIndex)){
                        positions.Add(new List<Vector3>());
                        texturePositions.Add(new List<Vector2>());
                        colors.Add(new List<Color>());
                        biomesToPos.Add((int)biome, biomesTotal);
                        pushIndex = biomesTotal;
                        biomesTotal++;
                        Biomes.Add(biome);

                    }

                    (List<Vector3> __positions, List<Vector2> __texturePositions, List<Color> __colors, List<int> __indicies) = Cells[y, x].GenerateMesh(chunkX, chunkY, width, height, mapWidth, mapHeight, x, y, noiseGenerator);

                    positions[pushIndex].AddRange(__positions);
                    texturePositions[pushIndex].AddRange(__texturePositions);
                    colors[pushIndex].AddRange(__colors);
                }
            }

            //positions.Reverse();
            //colors.Reverse();
            //vertices = new VertexPositionColor[positions.Count];
            Indicies = new IndexBuffer(graphics.GraphicsDevice, IndexElementSize.ThirtyTwoBits, positions.Count, BufferUsage.WriteOnly);

            Vertices = new VertexPositionColorTexture[biomesTotal][];

            int[] indicies = new int[positions.Count];

            for (int i = 0; i < positions.Count; i++)
            {
                var _vertices = new VertexPositionColorTexture[positions[i].Count];
                for (int j = 0; j < positions[i].Count; j++)
                {
                    _vertices[j] = new VertexPositionColorTexture(positions[i][j], colors[i][j], texturePositions[i][j]);
                }

                Vertices[i] = _vertices;

                //vertices[i] = new VertexPositionColor(positions[i], colors[i]);
                //indicies[i] = i;
            }

            Indicies.SetData(indicies);

            BoundingBox = new BoundingBox(new Vector3(chunkX * width * Cell.innerRadius * 2, chunkY * height * Cell.outerRadius * 3 / 2, 0), new Vector3((chunkX + 1 ) * width * Cell.innerRadius * 2, (chunkY + 1)* height * Cell.outerRadius * 3 / 2, 0));

            testEffect = new BasicEffect(graphics.GraphicsDevice);
        }

        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/EarthLike");
            texture = content.Load<Texture2D>("Textures/Sand");
        }

        public void Update()
        {

        }

        public float? Intersect(Ray ray)
        {
            return BoundingBox.Intersects(ray);
        }

        public Cell Intersects(Ray ray)
        {
            Cell _cell = null;

            float min = float.PositiveInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float? m = Cells[y, x].Intersects(ray);
                    if (m != null)
                    {
                        min = MathHelper.Min(min, (float)m);
                        return _cell = Cells[y, x];
                    }
                }
            }

            return _cell;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            if (camera.Frustum.Contains(BoundingBox) == ContainmentType.Disjoint)
                return;

            Space.ChunksDrawn++;

            //effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);

            //effect.Parameters["TileTexture"].SetValue(texture);

            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(camera.ViewMatrix);
            effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            //effect.Parameters["ViewVector"].SetValue(camera.lookAtVector);

            effect.Parameters["ModelTexture"].SetValue(texture);

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(Matrix.Identity * camera.ViewMatrix));
            //effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);


            graphics.GraphicsDevice.RasterizerState = TerrainRasterizerState;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            for (int i = 0; i < Vertices.Length; i++)
            {
                effect.Parameters["ModelTexture"].SetValue(NoiseGenerator.Textures[NoiseGenerator.biomeValues[(int)planetType].Length + (int)Biomes[i]]);

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }


                graphics.GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                Vertices[i],
                0,
                Vertices[i].Length / 3,
                VertexPositionColorTexture.VertexDeclaration
            );
            }
        }
    }
}
