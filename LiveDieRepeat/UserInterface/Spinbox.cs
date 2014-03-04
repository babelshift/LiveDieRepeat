using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.UserInterface
{
    public class Spinbox : Control
    {
        #region Members

        private MenuButton buttonLeft;
        private MenuButton buttonRight;
        private String label;
        private SpriteFont labelFont;
        private List<String> items;
        private int currentItemIndex;

        private String CurrentValue { get { return items[currentItemIndex]; } }
        private int LabelWidth { get { return (int)labelFont.MeasureString(label).X; } }
        private int ValueWidth { get { return (int)labelFont.MeasureString(CurrentValue).X; } }
        private int LabelHeight { get { return labelFont.LineSpacing; } }

        #endregion

        #region Properties

        public int CurrentIndex { get { return currentItemIndex; } }

        public override Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }

        /// <summary>The width of a spinbox is the total width of the two buttons, the label, and the currently selected value font
        /// </summary>
        public override int Width
        {
            get { return buttonLeft.Width + buttonRight.Width + LabelWidth + ValueWidth; }
        }

        /// <summary>The height of a spinbox is the maximum of the buttons, the label, and the currently selected value font
        /// </summary>
        public override int Height
        {
            get { return Math.Max(Math.Max(buttonLeft.Height, LabelHeight), Math.Max(buttonRight.Height, LabelHeight)); }
        }

        #endregion

        #region Constructors

        /// <summary>A spinbox is a horizontally oriented control that allows a user to click left and right to move forward and backward in an array of values
        /// </summary>
        /// <param name="label">Label of the spinbox</param>
        /// <param name="labelFont">Font to use for the label and the value</param>
        /// <param name="items">Array of items to use as the spinbox's source of data</param>
        /// <param name="currentItemIndex">If a current selected item is known, provide it</param>
        /// <param name="buttonLeftDefault">Texture for the left button's default state</param>
        /// <param name="buttonLeftSelected">Texture for the left button's selected state</param>
        /// <param name="buttonLeftHovered">Texture for the left button's hovered state</param>
        /// <param name="buttonLeftDisabled">Texture for the left button's disabled state</param>
        /// <param name="buttonRightDefault">Texture for the right button's default state</param>
        /// <param name="buttonRightSelected">Texture for the right button's selected state</param>
        /// <param name="buttonRightHovered">Texture for the right button's hovered state</param>
        /// <param name="buttonRightDisabled">Texture for the right button's disabled state</param>
        public Spinbox(String label, SpriteFont labelFont, List<String> items, int currentItemIndex, 
            Texture2D buttonLeftDefault, Texture2D buttonLeftSelected, Texture2D buttonLeftHovered, Texture2D buttonLeftDisabled,
            Texture2D buttonRightDefault, Texture2D buttonRightSelected, Texture2D buttonRightHovered, Texture2D buttonRightDisabled)
        {
            this.label = label;
            this.labelFont = labelFont;
            this.items = items;
            this.currentItemIndex = currentItemIndex;
            buttonLeft = new MenuButton(buttonLeftDefault, buttonLeftHovered, buttonLeftSelected, buttonLeftDisabled, null);
            buttonRight = new MenuButton(buttonRightDefault, buttonRightHovered, buttonRightSelected, buttonRightDisabled, null);
            buttonLeft.Selected += new EventHandler<EventArgs>(buttonLeft_Selected);
            buttonRight.Selected += new EventHandler<EventArgs>(buttonRight_Selected);
        }

        #endregion

        #region Game Loop

        public override void Update(GameTime gameTime)
        {
            buttonLeft.Update(gameTime);
            buttonRight.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            Vector2 labelPosition = new Vector2(Position.X, Position.Y);
            buttonLeft.Position = new Vector2(Position.X + LabelWidth, Position.Y);
            Vector2 valuePosition = new Vector2(Position.X + LabelWidth + buttonLeft.Width, Position.Y);
            buttonRight.Position = new Vector2(Position.X + LabelWidth + buttonLeft.Width + ValueWidth, Position.Y);

            spriteBatch.DrawString(labelFont, label, labelPosition, transitionColor * transitionAlpha);
            buttonLeft.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            spriteBatch.DrawString(labelFont, CurrentValue, valuePosition, transitionColor * transitionAlpha);
            buttonRight.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
        }

        #endregion

        #region Events

        /// <summary>When the left button is selected, move to the previous item in the list. If the previous item is outside the bounds of the array, move to the other end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLeft_Selected(object sender, EventArgs e)
        {
            if (currentItemIndex > 0)
                currentItemIndex--;
            else
                currentItemIndex = items.Count - 1;
        }

        /// <summary>
        /// <summary>When the right button is selected, move to the next item in the list. If the next item is outside the bounds of the array, move to the other end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRight_Selected(object sender, EventArgs e)
        {
            if (currentItemIndex < items.Count - 1)
                currentItemIndex++;
            else
                currentItemIndex = 0;
        }

        #endregion
    }
}
