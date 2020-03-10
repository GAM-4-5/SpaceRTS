using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceRts
{
    public class Camera
    {
        // We need this to calculate the aspectRatio
        // in the ProjectionMatrix property.
        GraphicsDevice graphicsDevice;

        public Vector3 position = new Vector3(20,20,20);
        // public Vector3 position = new Vector3(1111,  1151,  2500);  
        public Vector3 lookAtVector = new Vector3(0, 0, 0);
        /*(8, 8, 15);*/ //(700, 700, 0); //Vector3.Zero; //new Vector3(486.5f, 526.5f, 1); //.Zero;
        public Vector3 upVector = Vector3.UnitZ;
        public BoundingFrustum Frustum;

        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(
                    position, lookAtVector, upVector);
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                float nearClipPlane = 1;
                float farClipPlane = 10000;
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        private Point mouseStartCameraMovnmentPosition;
        private bool mouseMovingCamera;

        private double timeSinceLastClick = 0;

        public void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState)
        {
            Frustum = new BoundingFrustum(ViewMatrix * ProjectionMatrix * Matrix.CreateScale(0.85f, 0.85f, 1));
            Frustum.Matrix = Frustum.Matrix;

            #region Keyboard Movment
            if (keyboardState.IsKeyDown(Keys.W))
            {
                this.position.X -= 0.5f;
                this.position.Y -= 0.5f;
                lookAtVector.X -= 0.5f;
                lookAtVector.Y -= 0.5f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                this.position.X += 0.5f;
                this.position.Y += 0.5f;
                lookAtVector.X += 0.5f;
                lookAtVector.Y += 0.5f;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                this.position.X += 0.5f;
                this.position.Y -= 0.5f;
                lookAtVector.X += 0.5f;
                lookAtVector.Y -= 0.5f;

            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                this.position.X -= 0.5f;
                this.position.Y += 0.5f;
                lookAtVector.X -= 0.5f;
                lookAtVector.Y += 0.5f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                this.position.Z += 1f;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.position.Z -= 1f;
            }
            #endregion

            #region Mouse Movment

            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (!mouseMovingCamera)
                {
                    mouseMovingCamera = true;
                    mouseStartCameraMovnmentPosition = mouseState.Position;
                }
                else
                {
                    Vector3 d = new Vector3((mouseStartCameraMovnmentPosition - mouseState.Position).ToVector2(), 0) * new Vector3((float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f);
                    mouseStartCameraMovnmentPosition = mouseState.Position;
                    position -= d;
                    lookAtVector -= d;

                    mouseMovingCamera = false;
                }
            }

            #endregion

        }
    }
}