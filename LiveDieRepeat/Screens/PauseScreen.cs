using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LiveDieRepeat.Engine;
using LiveDieRepeat.UserInterface;

namespace LiveDieRepeat.Screens
{
    /// <summary>
    /// todo: inherit from MenuScreen
    /// </summary>
    public class PauseScreen : MenuScreen
    {
        #region Fields

        public event EventHandler<EventArgs> MenuButtonClicked;

        private KeyboardState previousKeyboardState;

        private Texture2D pauseBackground;

        private Rectangle backgroundRectangle;

        double sumElapsedTimeAlive = 0;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public PauseScreen()
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                Vector2 viewportSize = new Vector2(Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height);

                pauseBackground = Content.Load<Texture2D>("User Interface/Controls/PauseMenuBackground");

                backgroundRectangle = new Rectangle((int)viewportSize.X / 2 - pauseBackground.Width / 2,
                                                             (int)viewportSize.Y / 2 - pauseBackground.Height / 2,
                                                             pauseBackground.Width,
                                                             pauseBackground.Height);

                Texture2D resumeButtonText = Content.Load<Texture2D>("User Interface/Controls/PauseMenuResumeButton");
                Texture2D optionsButtonText = Content.Load<Texture2D>("User Interface/Controls/PauseMenuOptionsButton");
                Texture2D mainMenuButtonText = Content.Load<Texture2D>("User Interface/Controls/PauseMenuMainMenuButton");

                Texture2D buttonLargeNormal = Content.Load<Texture2D>("User Interface/Controls/ButtonLargeDefault");
                Texture2D buttonLargeSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonLargeSelected");
                Texture2D buttonLargeHover = Content.Load<Texture2D>("User Interface/Controls/ButtonLargeHover");
                Texture2D buttonLargeDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonLargeDisabled");

                MenuButton resumeButton = new MenuButton(buttonLargeNormal, buttonLargeHover, buttonLargeSelected, buttonLargeDisabled, resumeButtonText);
                MenuButton optionsButton = new MenuButton(buttonLargeNormal, buttonLargeHover, buttonLargeSelected, buttonLargeDisabled, optionsButtonText);
                MenuButton mainMenuButton = new MenuButton(buttonLargeNormal, buttonLargeHover, buttonLargeSelected, buttonLargeDisabled, mainMenuButtonText);

                resumeButton.Selected += new EventHandler<EventArgs>(resumeButton_Selected);
                mainMenuButton.Selected += new EventHandler<EventArgs>(menuButton_Selected);
                optionsButton.Selected += new EventHandler<EventArgs>(optionsButton_Selected);

                AddMenuControl(resumeButton);
                AddMenuControl(optionsButton);
                AddMenuControl(mainMenuButton);

                optionsButton.IsEnabled = false;

                ButtonSpacing = 3;
            }
        }

        #endregion

        #region Game Loop

        public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

            KeyboardState currentKeyboardState = Keyboard.GetState();
            sumElapsedTimeAlive += gameTime.ElapsedGameTime.TotalSeconds;

            // only accept input from keyboard if the screen has been alive for half of a second
            // this prevents the window immediately closing because of the ESC key being pressed
            if (sumElapsedTimeAlive > .5)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
                    OnResumeButtonClicked(this, EventArgs.Empty);
            }

            previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.


            // The background includes a border somewhat larger than the text itself.


            // Fade the popup alpha during transitions.

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());

            // Draw the background rectangle.
            spriteBatch.Draw(pauseBackground, backgroundRectangle, Color.White * TransitionAlpha);

            foreach (Control button in MenuControls)
                button.Draw(spriteBatch, gameTime, Color.White, TransitionAlpha);

            spriteBatch.End();
        }

        protected override void UpdateButtonMenuLocations()
        {
            int totalButtonHeight = 0;
            int buttonSectionOffsetY = 37;

            // start the position of the menu items at the top of the background
            Vector2 position = new Vector2(0, backgroundRectangle.Y + (backgroundRectangle.Height / 2));

            foreach (Control menuButton in MenuControls)
                totalButtonHeight += ButtonSpacing;

            // update each menu entry's location in turn
            foreach (Control menuButton in MenuControls)
            {
                // first button starts at the top
                if (MenuControls.IndexOf(menuButton) == 0)
                    position.Y = backgroundRectangle.Y + buttonSectionOffsetY;

                // center the button horizontally relative to the background image
                position.X = backgroundRectangle.X + (backgroundRectangle.Width / 2) - (menuButton.Width / 2);

                menuButton.Position = position;

                // add vertical spacing between this and the next button
                position.Y += menuButton.Height + ButtonSpacing;
            }
        }

        #endregion

        #region Events

        private void menuButton_Selected(object sender, EventArgs e)
        {
            OnMenuButtonClicked(sender, e);
        }

        private void resumeButton_Selected(object sender, EventArgs e)
        {
            OnResumeButtonClicked(sender, e);
        }

        private void optionsButton_Selected(object sender, EventArgs e)
        {
        }

        private void OnMenuButtonClicked(object sender, EventArgs e)
        {
            if (MenuButtonClicked != null)
                MenuButtonClicked(sender, e);
        }

        private void OnResumeButtonClicked(object sender, EventArgs e)
        {
            ExitScreen();
        }

        #endregion
    }
}
