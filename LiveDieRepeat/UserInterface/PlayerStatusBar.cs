using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.UserInterface
{
    public class PlayerStatusBar : Control
    {
        private Texture2D texture;

        public override Rectangle Bounds { get { return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height); } }

        public override int Width { get { return texture.Width; } }
        public override int Height { get { return texture.Height; } }

        public PlayerStatusBar(ContentManager content, Vector2 position)
            : base(position)
        {
            this.texture = content.Load<Texture2D>("User Interface/Controls/BarHpMpXp");
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            spriteBatch.Draw(texture, Position, transitionColor * transitionAlpha);
        }
    }
}
