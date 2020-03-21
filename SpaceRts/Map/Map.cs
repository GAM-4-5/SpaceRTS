

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 

using static SpaceRts.Planet;

namespace SpaceRts.Map
{
    public class Map
    {
        

        Chunk[,] Chunks;
        int Width, Height;

        public const int ChunkWidth = 6, ChunkHeight = 6;
        public Map(int id, int seed, int width, int height, float[] noiseMap, GraphicsDeviceManager graphics, PlanetTypes planetType)
        {
            Width = width;
            Height= height;
            Chunks = new Chunk[height, width];
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Chunks[y, x] = new Chunk(x, y, ChunkWidth, ChunkHeight, width, height, noiseMap, graphics, planetType, new Color(0,0, (int)(i / 36 * 255)));
                    i++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            // Chunks[0, 0].Draw(spriteBatch, graphics, camera);
            // Chunks[0, 1].Draw(spriteBatch, graphics, camera);
            // Chunks[1, 1].Draw(spriteBatch, graphics, camera);
            // Chunks[1, 0].Draw(spriteBatch, graphics, camera);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Chunks[y, x].Draw(spriteBatch, graphics, camera);
                }
            }
        }
    }
}