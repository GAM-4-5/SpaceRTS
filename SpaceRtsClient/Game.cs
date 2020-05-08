using System;
using System.Threading;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceRts;
using SpaceRts.Map;
using SpaceRts.Util;
using SpaceRtsClient.Util;
using SpaceRTS_UI;
using System.Linq;

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

            IsMouseVisible = true;
        }

        Space Space;
        Model Cube;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Coms.Initilize();

            //Thread.Sleep(1000);
            //Console.WriteLine("Starting lobby");

            //Coms.CreateLobby(CreateLoby);


            Cube = Content.Load<Model>("Models/Cube");
            base.Initialize();
        }


        private void CreateLoby(object obj)
        {

        }

        ClickableText PlayButton;
        bool PlayButtonClicked;
        double TimePlayClicked;

        public static GameSteates GameSteate = GameSteates.Game;

        static Point PlayPosition = new Point(256, 512);
        static Point SettingsPosition = new Point(256, 512 + 128);
        static Point ExitPosition = new Point(256, 512 + 256);

        RenderTarget2D _planetRenderTarget;

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
            //.LoadContent(Content);
            Chunk.LoadContent(Content);
            NoiseGenerator.LoadTextures(Content);

            Space.spriteFont = Content.Load<SpriteFont>("Fonts/MainFont");

            GameOptions gameOptions = new GameOptions(NumberOfSolarSystems.Normal, NumberOfPlantes.Normal, GameSpeed.Normal);

            //Fonts.Load(Content); TODO Implement
            Models.Load(Content);
            SpaceRts.Models.Base = Models.Zhus_Base;
            Textures.Load(Content);
            Shaders.Load(Content);

            Space = new Space(1423, gameOptions, 3, graphics);

            Global.Camera = new Camera(GraphicsDevice);

            PlayButton = new ClickableText(PlayPosition, "Play", Space.spriteFont, Color.White, new Point(0, 0), (GameTime gameTime) => {
                PlayButtonClicked = true;
                TimePlayClicked = gameTime.TotalGameTime.TotalSeconds;
            });

            _planetRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(GraphicsDevice.Viewport.Width * 0.625f + 1), GraphicsDevice.Viewport.Height / 2, false, SurfaceFormat.Color, DepthFormat.None);

            Shaders.MainMenu.Parameters["resolution"].SetValue(new Vector2(_planetRenderTarget.Width * 1.25f, _planetRenderTarget.Height));

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

        enum CameraPerspectives
        {
            closeUp,
            middle,
            map,
        }

        CameraPerspectives cameraPerspective = CameraPerspectives.closeUp;

        bool goneFullScreen = false;

        FrameCounter _frameCounter = new FrameCounter();

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

            if(GameSteate == GameSteates.MainMenu)
            {
                PlayButton.Update(gameTime, mouseState);

                if (PlayButtonClicked && TimePlayClicked - gameTime.TotalGameTime.TotalSeconds < -Math.PI * 0.75)
                {
                    GameSteate = GameSteates.MainMenu;
                }
            }
            else if (GameSteate == GameSteates.Game)
            {
                Global.MouseState = mouseState;
                Global.KeyboardState = keyboardState;

                Vector2 mousePosition = mouseState.Position.ToVector2();

                Vector3 nearPoint = new Vector3(mousePosition, 0);
                Vector3 farPoint = new Vector3(mousePosition, 1);

                nearPoint = GraphicsDevice.Viewport.Unproject(nearPoint, Global.Camera.ProjectionMatrix, Global.Camera.ViewMatrix, Matrix.Identity);
                farPoint = GraphicsDevice.Viewport.Unproject(farPoint, Global.Camera.ProjectionMatrix, Global.Camera.ViewMatrix, Matrix.Identity);

                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();

                Global.ClickRay = new Ray(nearPoint, direction);

                Space.Update();



                if (keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    Global.Camera.position = new Vector3(30, 30, 40);
                    Global.Camera.lookAtVector = new Vector3(0, 0, 0);
                    cameraPerspective = CameraPerspectives.closeUp;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad2))
                {
                    Global.Camera.position = new Vector3(600, 600, 600);
                    Global.Camera.lookAtVector = new Vector3(80, 80, 0);
                    cameraPerspective = CameraPerspectives.closeUp;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad3))
                {
                    Global.Camera.position = new Vector3(2100, 1800, 1800);
                    Global.Camera.lookAtVector = new Vector3(300, 300, 0);
                    cameraPerspective = CameraPerspectives.map;
                }

                Global.Camera.Update(gameTime, mouseState, keyboardState);
            }




            if (keyboardState.IsKeyDown(Keys.F12))
            {
                goneFullScreen = true;

                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                _planetRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width), GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.None);
                Shaders.MainMenu.Parameters["resolution"].SetValue(new Vector2(_planetRenderTarget.Width * 1.25f, _planetRenderTarget.Height));


                graphics.ToggleFullScreen();
                graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        public enum GameSteates
        {
            MainMenu,
            Settings,
            Lobby,
            Game,
        }


        public void InitRenderTargets()
        {

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);


            if (GameSteate == GameSteates.MainMenu)
            {
                Shaders.MainMenu.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

                //_planetRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(GraphicsDevice.Viewport.Width * 0.625f + 1), GraphicsDevice.Viewport.Height / 2, false, SurfaceFormat.Color, DepthFormat.None);
                spriteBatch.GraphicsDevice.SetRenderTarget(_planetRenderTarget);
                spriteBatch.GraphicsDevice.Clear(new Color(0, 0, 0, 0));

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Shaders.MainMenu);

                spriteBatch.Draw(_planetRenderTarget, new Rectangle(0, 0, _planetRenderTarget.Width, _planetRenderTarget.Height), Color.White);
                //_mainMenuEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.End();


                if (PlayButtonClicked)
                {
                    Shaders.BlackHoleTransition.Parameters["time"].SetValue((float)(TimePlayClicked - gameTime.TotalGameTime.TotalSeconds));
                    Shaders.BlackHoleTransition.Parameters["resolution"].SetValue(new Vector2(_planetRenderTarget.Width, _planetRenderTarget.Height));

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Shaders.BlackHoleTransition);
                    spriteBatch.Draw(_planetRenderTarget, new Rectangle(0, 0, _planetRenderTarget.Width, _planetRenderTarget.Height), Color.White);
                    spriteBatch.End();
                }


                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                spriteBatch.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin();

                spriteBatch.Draw(_planetRenderTarget, new Vector2(0, 0), new Rectangle(0, 0, _planetRenderTarget.Width, _planetRenderTarget.Height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                PlayButton.Draw(graphics, spriteBatch);
                spriteBatch.DrawString(Space.spriteFont, fps, new Vector2(50,50), Color.White);

                spriteBatch.End();


                /*spriteBatch.Begin(
                    rasterizerState: RasterizerState.CullNone,
                    effect: _mainMenuEffect
                );

                

                PlayButton.Draw(graphics, spriteBatch);

                

                PlayButton.Draw(graphics, spriteBatch);

                spriteBatch.End();*/
                /*

                spriteBatch.Begin();
                PlayButton.Draw(graphics, spriteBatch);
                spriteBatch.Draw(testTexture, new Vector2(50, 50), Color.White);
                spriteBatch.End();*/
            }
            else if (GameSteate == GameSteates.Settings)
            {

            }
            else if (GameSteate == GameSteates.Lobby)
            {

            }
            else if (GameSteate == GameSteates.Game)
            {
                spriteBatch.Begin(transformMatrix: Global.Camera.ProjectionMatrix);
                //spriteBatch.Begin();
                Space.Draw(spriteBatch, graphics, Global.Camera);
                spriteBatch.End();

                DrawModel(Cube, Vector3.Zero);
            }

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
                    effect.View = Global.Camera.ViewMatrix;
                    effect.Projection = Global.Camera.ProjectionMatrix;
                }

                mesh.Draw();
            }
        }
    }
}
