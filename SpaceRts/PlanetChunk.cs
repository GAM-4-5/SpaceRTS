using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceRts
{
    public class PlanetChunk
    {
        private static VertexPositionColor[] Vertecies;

        private static BoundingBox BoundingBox;

        private static Effect effect;

        private int Width, Height, X, Y;
        private float Cx, Cy;

        private const float TILE_SIZE = 2;
        private static float TILE_WIDTH = (float)Math.Sqrt(3) * TILE_SIZE;
        private static float TILE_WIDTH_2 = TILE_WIDTH / 2;
        private static float TILE_HEIGHT = 2 * TILE_SIZE;
        private static float TILE_HEIGHT_2 = TILE_HEIGHT / 2;
        private static float TILE_HEIGHT_4 = TILE_HEIGHT / 4;

        public PlanetChunk(float[] noiseMap, int x, int y, int cw, int ch)
        {
            Width = cw;
            Height = ch;

            X = x;
            Y = y;

            Cx = x * cw * TILE_WIDTH;
            Cy = y * ch * TILE_HEIGHT;

            Vector3[] positions = new Vector3[cw * ch * 12];

            int i = 0;
            for (int ty = 0; ty < ch; ty++)
            {
                for (int tx = 0; tx < cw; tx++)
                {

                    float value = noiseMap[ty * cw + tx];
                    
                    float cx = tx * TILE_WIDTH + Cx;
                    cx += ty % 2 == 0 ? TILE_WIDTH_2 : 0;
                    float cy = ty * 0.75f * TILE_HEIGHT + Cy;

                    float nx1 = cx - TILE_WIDTH_2;
                    float nx2 = cx;
                    float nx3 = cx + TILE_WIDTH_2;
                    float nx4 = cx + TILE_WIDTH_2;
                    float nx5 = cx;
                    float nx6 = cx - TILE_WIDTH_2;

                    float ny1 = cy - TILE_HEIGHT_4;
                    float ny2 = cy - TILE_HEIGHT_2;
                    float ny3 = cy - TILE_HEIGHT_4;
                    float ny4 = cy + TILE_HEIGHT_4;
                    float ny5 = cy + TILE_HEIGHT_2;
                    float ny6 = cy + TILE_HEIGHT_4;

                    float[][][] sidePositions = new float[][][] {
                        new float[][] { new float[] { nx1, nx2 }, new float[] { ny1, ny2} },
                        new float[][] { new float[] { nx2, nx3 }, new float[] { ny2, ny3} },
                        new float[][] { new float[] { nx3, nx4 }, new float[] { ny3, ny4} },
                        new float[][] { new float[] { nx4, nx5 }, new float[] { ny4, ny5} },
                        new float[][] { new float[] { nx5, nx6 }, new float[] { ny5, ny6} },
                        new float[][] { new float[] { nx6, nx1 }, new float[] { ny6, ny1} },
                    };
                    //Color color = Color.Blue;//value > 10 ? value > 20 ? Color.SandyBrown: Color.Green : Color.Blue;

                    int maxHeight = 20;
                    float z = (value % 0.1f) * maxHeight;

                    positions[i] = new Vector3(nx1, ny1, z);
                    positions[i + 1] = new Vector3(nx2, ny2, z);
                    positions[i + 2] = new Vector3(nx3, ny3, z);
                    
                    positions[i + 3] = new Vector3(nx1, ny1, z);
                    positions[i + 4] = new Vector3(nx3, ny3, z); 
                    positions[i + 5] = new Vector3(nx4, ny4, z);

                    positions[i + 6] = new Vector3(nx1, ny1, z);
                    positions[i + 7] = new Vector3(nx4, ny4, z);
                    positions[i + 8] = new Vector3(nx6, ny6, z);

                    positions[i + 9] = new Vector3(nx4, ny4, z);
                    positions[i + 10] = new Vector3(nx5, ny5, z);
                    positions[i + 11] = new Vector3(nx6, ny6, z);

                    // int j = 12;
                    // for(int p = 0; p < 6; p++){
                    //     positions[i + j] = new Vector3(sidePositions[p][0][0], sidePositions[p][1][0], z);
                    //     positions[i + j] = new Vector3(sidePositions[p][0][1], sidePositions[p][1][1], z);
                    //     positions[i + j] = new Vector3(sidePositions[p][0][0], sidePositions[p][1][0], z - 0.5f);

                    //     positions[i + j] = new Vector3(sidePositions[p][0][1], sidePositions[p][1][1], z);
                    //     positions[i + j] = new Vector3(sidePositions[p][0][1], sidePositions[p][1][1], z - 0.5f);
                    //     positions[i + j] = new Vector3(sidePositions[p][0][0], sidePositions[p][1][0], z - 0.5f);
                    //     j++;
                    // }
                    // i += 48;
                    i+=12;
                }
            }
    
            Vertecies = new VertexPositionColor[positions.Length];
            for (int v = 0; v < positions.Length; v++)
            {
                Color color = Planet.GradientColors[0][Planet.FindGradientValueIndex(0, positions[v].Z )];
                Vertecies[v] = new VertexPositionColor(positions[v], color);
            }
            BoundingBox = BoundingBox.CreateFromPoints(positions);
        }
        
        public static void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/FogOfWar");
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
                        //effect.Parameters["Time"].SetValue(x);
            //x += 0.07f;

            //if(camera.Frustum.Contains(BoundingBox) == ContainmentType.Disjoint)
            //    return;

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            effect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix * Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    Vertecies,
                    0,
                    Width * Height * 4);
            }
        }
    }
}