using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.UserInterface
{
    public class Checkbox : Control
    {
        private Texture2D textureUnchecked;
        private Texture2D textureChecked;
        private Texture2D textureActive;
        private String label;
        private SpriteFont labelFont;
        private int checkboxPositionOffsetX;

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;

                if (isChecked)
                    textureActive = textureChecked;
                else
                    textureActive = textureUnchecked;
            }
        }

        public override Rectangle Bounds
        {
            get { return new Rectangle((int)(Position.X + checkboxPositionOffsetX), (int)Position.Y, textureActive.Width, textureActive.Height); }
        }

        private int LabelWidth { get { return (int)labelFont.MeasureString(label).X; } }
        private int LabelHeight { get { return labelFont.LineSpacing; } }

        public override int Width
        {
            get { return textureActive.Width + (int)LabelWidth; }
        }

        public override int Height
        {
            get { return Math.Max(textureActive.Height, LabelHeight); }
        }

        private MouseState previousMouseState;

        public Checkbox(String label, SpriteFont labelFont, Texture2D textureUnchecked, Texture2D textureChecked, Vector2 position)
        {
            this.label = label;
            this.labelFont = labelFont;
            this.textureUnchecked = textureUnchecked;
            this.textureChecked = textureChecked;
            this.Position = position;
            this.textureActive = textureUnchecked;

            checkboxPositionOffsetX = LabelWidth + (textureActive.Width / 2);
        }

        private void Toggle()
        {
            IsChecked = !IsChecked;

            if (IsChecked)
                textureActive = textureChecked;
            else
                textureActive = textureUnchecked;
        }

        public override void Update(GameTime gameTime)
        {
            Point mousePosition = MouseHelper.GetCurrentMousePosition();
            MouseState mouseState = Mouse.GetState();

            if (previousMouseState == null)
                previousMouseState = mouseState;

            if (Bounds.Contains(mousePosition))
                if (mouseState.LeftButton == ButtonState.Pressed && !mouseState.LeftButton.Equals(previousMouseState.LeftButton))
                    Toggle();

            previousMouseState = mouseState;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color color, float transitionAlpha)
        {
            color *= transitionAlpha;

            Vector2 checkboxPosition = new Vector2(Position.X + checkboxPositionOffsetX, Position.Y);

            spriteBatch.DrawString(labelFont, label, Position, color);
            spriteBatch.Draw(textureActive, checkboxPosition, color);
        }
    }
}
