using System;
using System.Collections.Generic;
using LiveDieRepeat.Engine;
using LiveDieRepeat.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.Screens
{
    public class MenuScreen : Screen
    {
        protected ContentManager Content { get; private set; }
        protected MenuBackground MenuBackground { get; set; }
        protected MouseState PreviousMouseState { get; set; }
        protected List<Control> MenuControls { get; private set; }
        protected Texture2D MenuHeaderImage { get; set; }
        protected int ButtonSpacing { get; set; }
        private List<Control> MiscControls { get; set; }
        protected float MenuHeaderImageScale { get; set; }

        private ScalingDirection scalingDirection;
        private enum ScalingDirection
        {
            Increasing,
            Decreasing
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (Content == null)
                    Content = new ContentManager(ScreenManager.Game.Services, "Content");

                MenuControls = new List<Control>();
                MiscControls = new List<Control>();
                ButtonSpacing = 0;
                MenuHeaderImageScale = 1f;
            }
        }

        public override void Unload()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            foreach (Control button in MenuControls)
                button.Update(gameTime);

            UpdateButtonMenuLocations();
        }

        public override void Draw(GameTime gameTime)
        {
            //ScreenManager.GraphicsDevice.Clear(new Color(10, 13, 25));
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());

            Color transitionColor = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);
            MenuBackground.Draw(spriteBatch, Color.White);

            // transition the logo on and off based on the transition position
            if (MenuHeaderImage != null)
            {
                float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
                Vector2 titlePosition = new Vector2(Resolution.VirtualViewport.Width / 2, 175);
                titlePosition.Y -= transitionOffset * 100;
                Vector2 titleOrigin = new Vector2(MenuHeaderImage.Bounds.Center.X, MenuHeaderImage.Bounds.Center.Y);
                spriteBatch.Draw(MenuHeaderImage, titlePosition, null, transitionColor * TransitionAlpha, 0, titleOrigin, MenuHeaderImageScale, SpriteEffects.None, 0);
            }

            foreach (Control button in MenuControls)
                button.Draw(spriteBatch, gameTime, transitionColor, TransitionAlpha);

            foreach (Control control in MiscControls)
                control.Draw(spriteBatch, gameTime, transitionColor, TransitionAlpha);

            spriteBatch.End();
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateButtonMenuLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, Resolution.VirtualViewport.Height / 1.75f);

            // update each menu entry's location in turn
            foreach (Control buttonMenu in MenuControls)
            {
                // each entry is to be centered horizontally
                position.X = Resolution.VirtualViewport.Width / 2 - buttonMenu.Width / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                buttonMenu.Position = position;

                // move down for the next entry the size of this entry
                position.Y += buttonMenu.Height + ButtonSpacing;
            }
        }

        public void AddMenuControl(Control control)
        {
            MenuControls.Add(control);
        }

        public void AddMiscControl(Control control)
        {
            MiscControls.Add(control);
        }

        protected void PulseMenuHeaderSize()
        {
            if (MenuHeaderImageScale <= 1.025 && scalingDirection == ScalingDirection.Increasing)
                MenuHeaderImageScale += .0002f;
            else if (MenuHeaderImageScale >= 1 && scalingDirection == ScalingDirection.Decreasing)
                MenuHeaderImageScale -= 0.0002f;

            if (MenuHeaderImageScale >= 1.025)
                scalingDirection = ScalingDirection.Decreasing;
            else if (MenuHeaderImageScale <= 1)
                scalingDirection = ScalingDirection.Increasing;
        }
    }
}
