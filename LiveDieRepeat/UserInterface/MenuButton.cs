using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Screens;
using Microsoft.Xna.Framework.Graphics;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.UserInterface
{
    public class MenuButton : Control
    {
        #region Fields

        private const int BUTTON_WIDTH = 403; // to do: constants stored somewhere else?

        private Texture2D textureImage;
        private Texture2D textureImageHover;
        private Texture2D textureImageSelected;
        private Texture2D textureImageDisabled;
        private Texture2D textureImageActive;
        private Texture2D textureImageText;
        private String text;

        private SpriteFont buttonFont;
        private MouseState PreviousMouseState;

        private enum DisplayType
        {
            Text,
            Image
        }

        private DisplayType displayType;

        public override Rectangle Bounds
        {
            get
            {
                if (displayType == DisplayType.Image)
                    return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                else if (displayType == DisplayType.Text)
                    return new Rectangle((int)Position.X - ((BUTTON_WIDTH - Width) / 2), (int)Position.Y, BUTTON_WIDTH, Height);
                else
                    return Rectangle.Empty;
            }
        }

        public override int Height
        {
            get
            {
                if (displayType == DisplayType.Image)
                    return textureImage.Height;
                else if (displayType == DisplayType.Text)
                    return buttonFont.LineSpacing;
                else
                    return 0;
            }
        }

        public override int Width
        {
            get
            {
                if (displayType == DisplayType.Image)
                    return textureImage.Width;
                else if (displayType == DisplayType.Text)
                    return (int)buttonFont.MeasureString(text).X;
                else
                    return 0;
            }
        }

        #endregion

        #region Properties

        public String Text
        {
            set
            {
                if (this.displayType == DisplayType.Image)
                    throw new Exception("Do not use this property if the button is based on an image.");

                this.text = value;
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                if (isEnabled == true)
                    SetActiveTexture(textureImage);
                else
                    SetActiveTexture(textureImageDisabled);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        #endregion

        #region Initialization

        public MenuButton(Texture2D textureImage)
            : this(textureImage, null, null, null, null)
        {
            SetActiveTexture(textureImage);
        }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuButton(Texture2D textureImage, Texture2D textureImageHover, Texture2D textureImageSelected, Texture2D textureImageDisabled, Texture2D textureImageText)
        {
            this.textureImage = textureImage;
            this.textureImageHover = textureImageHover;
            this.textureImageSelected = textureImageSelected;
            this.textureImageDisabled = textureImageDisabled;
            this.textureImageText = textureImageText;
            SetActiveTexture(textureImage);
            this.displayType = DisplayType.Image;
        }

        public MenuButton(String text, SpriteFont buttonFont, Texture2D menuButtonHover)
        {
            this.buttonFont = buttonFont;
            this.text = text;
            this.textureImageHover = menuButtonHover;
            this.displayType = DisplayType.Text;
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            Point mousePosition = MouseHelper.GetCurrentMousePosition();
            MouseState currentMouseState = Mouse.GetState();

            // only bother evaluating input on the button if it is enabled
            if (IsEnabled)
            {
                // if the mouse is hovering this button, highlight it
                if (Bounds.Contains(mousePosition))
                {
                    SetActiveTexture(textureImageHover);

                    // if the mouse is clicked while hovering this button, press it
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                        SetActiveTexture(textureImageSelected);
                    // if the mouse is not clicked after being clicked, set click event (this is identical to OnMouseUp in WindowsForms)
                    else if (currentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed)
                        OnSelectEntry();
                }
                else
                {
                    if (displayType == DisplayType.Image)
                        SetActiveTexture(textureImage);
                    else if (displayType == DisplayType.Text)
                        textureImageActive = null;
                }

            }

            PreviousMouseState = currentMouseState;
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color color, float transitionAlpha)
        {
            // Modify the alpha to fade text out during transitions.
            color *= transitionAlpha;

            if (displayType == DisplayType.Image)
            {
                spriteBatch.Draw(textureImageActive, Position, color);

                if (textureImageText != null)
                {
                    Vector2 textPosition = new Vector2(Bounds.X + ((Bounds.Width / 2) - (textureImageText.Width / 2)), Bounds.Y + ((Bounds.Height / 2) - (textureImageText.Height / 2)));
                    spriteBatch.Draw(textureImageText, textPosition, color);
                }
            }
            else
            {
                spriteBatch.DrawString(buttonFont, text, Position, color);

                if (textureImageActive != null)
                {
                    Vector2 hoverImagePosition = new Vector2(Bounds.X, Position.Y + (Height / 2) - (textureImageHover.Height / 2));
                    spriteBatch.Draw(textureImageActive, hoverImagePosition, color);
                }
            }
        }

        #endregion

        /// <summary>The active texture of this button will be set to the passed texture as long as it isn't null
        /// </summary>
        /// <param name="textureActive"></param>
        private void SetActiveTexture(Texture2D textureActive)
        {
            if (textureActive != null)
                textureImageActive = textureActive;
        }
    }
}
