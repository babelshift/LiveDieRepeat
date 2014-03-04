using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Screens
{
    public class MenuBackground
    {
        private Texture2D backgroundTile;
        private int tileWidth;
        private int tileHeight;
        private int numTilesX;
        private int numTilesY;
        private Vector2 positionOffset;

        public MenuBackground(Texture2D backgroundTile, Rectangle background)
        {
            this.backgroundTile = backgroundTile;
            this.tileWidth = backgroundTile.Width;
            this.tileHeight = backgroundTile.Height;
            this.numTilesX = (background.Width / tileWidth) + 1;
            this.numTilesY = (background.Height / tileHeight) + 1;
            this.positionOffset = new Vector2(background.X, background.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Color transitionColor)
        {
            for(int i = 0; i < numTilesX; i++)
                for(int j = 0; j < numTilesY; j++)
                    spriteBatch.Draw(backgroundTile, new Rectangle(i * tileWidth + (int)positionOffset.X, j * tileHeight + (int)positionOffset.Y, tileWidth, tileHeight), transitionColor);
        }
    }
}
