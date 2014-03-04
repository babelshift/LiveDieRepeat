using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.UserInterface
{
    public class Label : Control
    {
        private SpriteFont font;
        private Color color;

        public String Text { get; set; }

        public override Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }

        public override int Width
        {
            get { return (int)font.MeasureString(Text).X; }
        }

        public override int Height
        {
            get { return font.LineSpacing; }
        }

        public Label(SpriteFont font, Vector2 position, Color color)
        {
            this.font = font;
            this.Text = String.Empty;
            this.position = position;
            this.color = color;
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            spriteBatch.DrawString(font, Text, position, color * transitionAlpha);
        }
    }
}
