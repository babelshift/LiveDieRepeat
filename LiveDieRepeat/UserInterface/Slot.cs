using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using LiveDieRepeat.Items;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.UserInterface
{
    public abstract class Slot : Control
    {
        #region Members

        Texture2D rectTexture;
        private Texture2D textureSlotImage;
        private Texture2D textureSlotImageSelected;
        private Texture2D textureSlotActive;
        private bool isSelected = false;
        protected ItemEntity itemAssociated;

        #endregion

        #region Propertes

        public override Rectangle Bounds { get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); } }
        public override int Width { get { return textureSlotActive.Width; } }
        public override int Height { get { return textureSlotActive.Height; } }

        /// <summary>If this slot is selected, change its texture to the selected texture, otherwise, change it to the default texture
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;

                if (isSelected)
                    textureSlotActive = textureSlotImageSelected;
                else
                    textureSlotActive = textureSlotImage;
            }
        }

        #endregion

        public Slot(Texture2D textureSlotImage, Texture2D textureSlotImageSelected, GraphicsDevice graphicsDevice)
        {
            this.textureSlotImage = textureSlotImage;
            this.textureSlotImageSelected = textureSlotImageSelected;
            this.textureSlotActive = textureSlotImage;
            rectTexture = new Texture2D(graphicsDevice, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            Color color = transitionColor * transitionAlpha;

            spriteBatch.Draw(textureSlotActive, Position, color);

            if (itemAssociated != null)
            {
                itemAssociated.Position = new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
                itemAssociated.Draw(spriteBatch, rectTexture);
            }
        }
    }
}
