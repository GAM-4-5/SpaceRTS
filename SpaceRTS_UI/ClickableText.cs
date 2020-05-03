using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace SpaceRTS_UI
{
    public class ClickableText
    {
        Point position;
        string text;
        Color color;
        Point padding;
        SpriteFont spriteFont;
        Action<GameTime> onClick;

        Rectangle boundingBox;
        bool mouseOver;
        double clickColldown;

        public ClickableText(Point position, string text, SpriteFont spriteFont, Color color, Point padding, Action<GameTime> onClick)
        {
            this.position = position;
            this.text = text;
            this.color = color;
            this.spriteFont = spriteFont;
            this.padding = padding;
            this.onClick = onClick;

            boundingBox = new Rectangle(position, spriteFont.MeasureString(text).ToPoint() + padding + padding);
        }

        public ClickableText(Point position, string text, SpriteFont spriteFont, Color color, Action<GameTime> onClick) : this(position, text, spriteFont, color, Point.Zero, onClick)
        {
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if(boundingBox.Contains(mouseState.Position))
            {
                mouseOver = true;
                if(mouseState.LeftButton == ButtonState.Pressed && clickColldown <= 0)
                {
                    clickColldown = 500;
                    onClick(gameTime);
                }
            }

            if (clickColldown > 0)
                clickColldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            if(mouseOver)
                spriteBatch.DrawString(spriteFont, text, position.ToVector2() + padding.ToVector2(), color, 0f, Vector2.Zero, 1.125f, SpriteEffects.None, 1f);
            else
                spriteBatch.DrawString(spriteFont, text, position.ToVector2() + padding.ToVector2(), color);
        }
    }
}
