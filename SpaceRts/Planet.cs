using System;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using SpaceRts.Map;
using System.Collections.Generic;
using SpaceRts.Structures;
using Microsoft.Xna.Framework.Input;
using SpaceRts.Noise;

namespace SpaceRts
{
    public class Planet
    {
        public enum PlanetTypes { Magma, Desert, Rocky, Terran, Cold, Gas };
        public enum PlanetSizes { Small, Medium, Large };
        public static int[][] PlanetSizeDImensions = { new int[] { 5, 5 } , new int[] { 8, 8 }, new int[] { 12, 12 } };

        public PlanetTypes PlanetType;
        public PlanetSizes PlanetSize;
        private const int CHUNK_WIDTH = 16;
        private const int CHUNK_HEIGHT = 16;
        private static Effect effect;
        private int ChunksWidth, ChunksHeight, CellsWidth, CellsHeight;
        public int Id;
        public static Color[][] GradientColors = new Color[][] { new Color[6] { Color.Yellow, Color.Orange, Color.Red, Color.DarkRed, Color.Gray, Color.Black } };
        public static float[][] GradientValues = new float[][] { new float[6] { 0, 0.1f, 0.2f, 0.3f, 0.5f, 0.7f } };

        private NoiseGenerator noiseGenerator;

        private Map.Map Map;

        private List<Structure> Structures = new List<Structure>();
        public Planet(int id, int seed, GraphicsDeviceManager graphics, PlanetTypes planetType, PlanetSizes planetSize)
        {
            Id = id;
            PlanetType = planetType;
            PlanetSize = planetSize;
            ChunksWidth = PlanetSizeDImensions[(int)planetSize][0];
            ChunksHeight = PlanetSizeDImensions[(int)planetSize][1];
            CellsWidth = ChunksWidth * CHUNK_WIDTH;
            CellsHeight = ChunksHeight * CHUNK_HEIGHT;

            noiseGenerator = new NoiseGenerator(planetType, seed, CellsWidth, CellsHeight);

            Map = new Map.Map(id, seed, ChunksWidth, ChunksHeight, noiseGenerator, graphics, planetType);

            List<Vector2> greens = PoissonSample.GeneratePoints(new Random(seed), 15, new Vector2(CellsWidth, CellsHeight));
            List<Vector2> blues = PoissonSample.GeneratePoints(new Random(seed), 20, new Vector2(CellsWidth, CellsHeight));
            List<Vector2> reds = PoissonSample.GeneratePoints(new Random(seed), 25, new Vector2(CellsWidth, CellsHeight));

            for (int i = 0; i < greens.Count; i++)
            {
                Structures.Add(new Ore(GetCellAtPosition(greens[i]), Ore.OreTypes.Green));
            }
            for (int i = 0; i < blues.Count; i++)
            {
                Structures.Add(new Ore(GetCellAtPosition(blues[i]), Ore.OreTypes.Blue));
            }
            for (int i = 0; i < reds.Count; i++)
            {
                Structures.Add(new Ore(GetCellAtPosition(reds[i]), Ore.OreTypes.Red));
            }
        }

        public Cell GetCellAtPosition(Vector2 position)
        {
            return GetCellAtPosition((int)position.X, (int)position.Y);
        }

        public Cell GetCellAtPosition(int x, int y)
        {
            return Map.CellAtPosition(x, y);
        }

        public void Update()
        {

        }

        public void SelectedUpdate()
        {
            if (Global.MouseState.LeftButton == ButtonState.Pressed)
            {
                (Chunk _chunk, Cell _cell) = Map.Intersect(Global.ClickRay);

                Structures.Add(new Base(Models.Base, _cell));
            }
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

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            Console.WriteLine(PlanetType);

            Map.Draw(spriteBatch, graphics, camera);

            for (int i = 0; i < Structures.Count; i++)
            {
                if (Structures[i] != null && camera.Frustum.Contains(Structures[i].BoundingBox) == ContainmentType.Disjoint)
                    Structures[i].Draw(spriteBatch, graphics, camera);
            }
        }
    }
}

