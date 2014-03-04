using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LiveDieRepeat.UserInterface;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Screens
{
    public class TitleScreen : MenuScreen
    {
        private MenuButton pressAnyKey;

        public event EventHandler<EventArgs> PressAnyKeyEvent;

        public TitleScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                // load textures
                MenuHeaderImage = Content.Load<Texture2D>("User Interface/ChainsOfAcadia3");
                Texture2D pressAnyKeyImage = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDefault");
                Texture2D pressAnyKeyText = Content.Load<Texture2D>("User Interface/Controls/TitleScreenAnyKeyButton");
                Texture2D menuBackgroundImage = Content.Load<Texture2D>("Backgrounds/Background_Menu_Pattern3");

                pressAnyKey = new MenuButton(pressAnyKeyImage, null, null, null, pressAnyKeyText);

                MenuControls.Add(pressAnyKey);

                // load background
                MenuBackground = new MenuBackground(menuBackgroundImage, new Rectangle(0, 0, Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height));
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

            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyboardState.GetPressedKeys().Length > 0
                || mouseState.LeftButton == ButtonState.Pressed
                || mouseState.RightButton == ButtonState.Pressed
                || mouseState.MiddleButton == ButtonState.Pressed)
                OnPressAnyKeyEvent(this, EventArgs.Empty);

            PulseMenuHeaderSize();
        }

        //public override void Draw(GameTime gameTime)
        //{
        //    //ScreenManager.GraphicsDevice.Clear(new Color(10, 13, 25));
        //    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        //    // transition the logo on and off based on the transition position
        //    float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
        //    Vector2 titlePosition = new Vector2(Resolution.VirtualViewport.Width / 2 - mainMenuLogo.Width / 2, 80);
        //    titlePosition.Y -= transitionOffset * 100;
        //    Color transitionColor = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

        //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());

        //    menuBackground.Draw(spriteBatch, transitionColor);
        //    spriteBatch.Draw(mainMenuLogo, titlePosition, transitionColor);

        //    pressAnyKey.Draw(spriteBatch, gameTime, transitionColor, TransitionAlpha);

        //    spriteBatch.End();
        //}

        #region Button Events

        private void OnPressAnyKeyEvent(object sender, EventArgs e)
        {
            if (PressAnyKeyEvent != null)
                PressAnyKeyEvent(sender, e);
        }

        #endregion
    }
}