using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceRts.Util;

namespace SpaceRts
{
    public class Space
    {
        public static int ChunksDrawn = 0;
        public static SpriteFont spriteFont;

        public Random Random;
        public GameOptions GameOptions;

        public SolarSystem[] SolarSystems;

        public int NumberOfPlayers;

        public FogOfWar FogOfWar;

        // TEMP
        private Texture2D temp;
        public Space(int GenerationSeed, GameOptions gameOptions, int numberOfPlayers, GraphicsDeviceManager graphics)
        {
            Random = new Random(GenerationSeed);
            GameOptions = gameOptions;

            NumberOfPlayers = numberOfPlayers;

            GenerateSolarSystems(graphics, GenerationSeed);

            temp = new Texture2D(graphics.GraphicsDevice, 1, 1);

            Color[] data = new Color[1];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            temp.SetData(data);

            FogOfWar = new FogOfWar(GenerationSeed, 1000, 1000, graphics);
        }

        private void GenerateSolarSystems(GraphicsDeviceManager graphics, int seed)
        {
            int n = ((int)GameOptions.NumberOfSolarSystems + 1) * NumberOfPlayers;
            SolarSystems = new SolarSystem[n];
            Stack systemsStack = new Stack();
            Stack connectionsStack = new Stack();
            int cn = 0, cd = 0;
            

            int systemsPerSystemMax = 4;
            int systemsPerSystem = Math.Max(systemsPerSystemMax - cd, 1);

            SolarSystem[] connections1 = new SolarSystem[systemsPerSystem];
            connectionsStack.Push(connections1);

            SolarSystem solarSystem1 = new SolarSystem(Random.Next(), graphics, new Vector2(0, 0), connections1, Random.Next(1, 5));
            SolarSystems[cn] = solarSystem1;
            systemsStack.Push(solarSystem1);

            cd = 1;
            cn = 1;
            while (cn < n)
            {
                systemsPerSystem = Math.Max(systemsPerSystemMax - cd, 1);
                SolarSystem system = (SolarSystem)systemsStack.Pop();
                SolarSystem[] connections = (SolarSystem[])connectionsStack.Pop();

                for (int i = 0; i < systemsPerSystem && cn < n; i++)
                {
                    Vector2 newPositon = Vector2.Transform(system.Position + new Vector2(0, Random.Next(100, 600)), Matrix.CreateRotationZ((float)(Random.NextDouble() * Math.PI)));

                    SolarSystem[] newConnections = new SolarSystem[systemsPerSystem];
                    SolarSystem newSystem = new SolarSystem(Random.Next(), graphics, newPositon, newConnections, Random.Next(1, 5));
                    connections[i] = newSystem;
                    SolarSystems[cn] = newSystem;
                    cn++;

                    systemsStack.Push(newSystem);
                    connectionsStack.Push(newConnections);
                }


            }
        }

        public void Update()
        {
            for (int i = 0; i < SolarSystems.Length; i++)
            {
                SolarSystems[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            //FogOfWar.Draw(spriteBatch, graphics, camera);
            ChunksDrawn = 0;

            if (Global.SelectedSolarSystem > -1)
            {
                SolarSystems[Global.SelectedSolarSystem].Draw(spriteBatch, graphics, camera);
            }
            else
            {
                for (int i = 0; i < SolarSystems.Length; i++)
                {
                    if (SolarSystems[i] != null)
                    {
                        spriteBatch.Draw(temp, SolarSystems[i].Position, new Rectangle(0, 0, 1, 1), i == 0 ? Color.Green : Color.Blue, 0f, Vector2.Zero, 10f, SpriteEffects.None, 1);
                        foreach (var SolarSystem in SolarSystems[i].Connections)
                        {
                            if (SolarSystem != null)
                            {
                                float angle = (float)Math.Atan((SolarSystems[i].Position.Y - SolarSystem.Position.Y) / (SolarSystems[i].Position.X - SolarSystem.Position.X));
                                //Console.WriteLine(angle);
                                spriteBatch.Draw(temp,
                                    SolarSystem.Position,
                                    //SolarSystems[i].Position + new Vector2(500, 500),
                                    new Rectangle(0, 0, 1, 1),
                                    Color.Green,
                                    angle,
                                    Vector2.Zero,
                                    new Vector2((SolarSystems[i].Position.Y - SolarSystem.Position.Y), 1),// (SolarSystems[i].Position.Y - SolarSystem.Position.Y)),
                                    SpriteEffects.None,
                                    1f);
                                    
                            }
                        }
                    }

                }
            }


        }
    }
}
