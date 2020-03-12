using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceRts;
using SpaceRts.Util;
using SpaceRtsClient.Util;

namespace SpaceRtsClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Space Space;
        Model Cube;
        public static Camera Camera;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Coms.Initilize();

            //Thread.Sleep(1000);
            //Console.WriteLine("Starting lobby");

            //Coms.CreateLobby(CreateLoby);

            GameOptions gameOptions = new GameOptions(NumberOfSolarSystems.Normal, NumberOfPlantes.Normal, GameSpeed.Normal);
            Space = new Space(12201, gameOptions, 3, graphics);

            Camera = new Camera(GraphicsDevice);

            Cube = Content.Load<Model>("Models/Cube");
            base.Initialize();
        }


        private void CreateLoby(object obj)
        {

        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            FogOfWar.LoadContent(Content);
            Planet.LoadContent(Content);
            PlanetChunk.LoadContent(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            Camera.Update(gameTime, mouseState, keyboardState);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            spriteBatch.Begin(transformMatrix: Camera.ProjectionMatrix);
            //spriteBatch.Begin();
            Space.Draw(spriteBatch, graphics, Camera);
            spriteBatch.End();

            DrawModel(Cube, Vector3.Zero);

            base.Draw(gameTime);
        }

        public static void DrawModel(Model model, Vector3 position)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = Matrix.CreateTranslation(position);
                    effect.View = Camera.ViewMatrix;
                    effect.Projection = Camera.ProjectionMatrix;
                }

                mesh.Draw();
            }
        }
    }
}
