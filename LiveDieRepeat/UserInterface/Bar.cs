using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Screens;

namespace LiveDieRepeat.UserInterface
{
    public abstract class Bar : Control
    {
        private float MAX_PERCENT_FILLED = 100;
        private float MIN_PERCENT_FILLED = 0;

        private float percentFilled;
        public float PercentFilled
        {
            get { return percentFilled; }
            set { percentFilled = (MathHelper.Clamp(value, MIN_PERCENT_FILLED, MAX_PERCENT_FILLED) / 100); }
        }

        public override Rectangle Bounds { get { return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height); } }

        public override int Width { get { return texture.Width; } }
        public override int Height { get { return texture.Height; } }

        private Texture2D texture;

        public Bar(ContentManager content, String textureName, Vector2 position)
            : base(position)
        {
            texture = content.Load<Texture2D>(textureName);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            if(PercentFilled > 0)
                spriteBatch.Draw(texture, Position, new Rectangle(0, 0, (int)(texture.Width * PercentFilled), texture.Height), transitionColor * transitionAlpha);
        }

        public abstract void Reset();
    }
}
