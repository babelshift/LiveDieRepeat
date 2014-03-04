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
    public class GameOver : MenuScreen
    {
        public event EventHandler<EventArgs> RestartButtonClicked;
        public event EventHandler<EventArgs> MainMenuButtonClicked;
        public event EventHandler<EventArgs> ExitButtonClicked;

        public GameOver()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                // load the textures
                MenuHeaderImage = Content.Load<Texture2D>("User Interface/GameOver");

                Texture2D tryAgainButtonText = Content.Load<Texture2D>("User Interface/Controls/GameOverMenuTryAgainButton");
                Texture2D mainMenuButtonText = Content.Load<Texture2D>("User Interface/Controls/OptionsMenuMainMenuButton");

                Texture2D buttonExtraLargeNormal = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDefault");
                Texture2D buttonExtraLargeSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeSelected");
                Texture2D buttonExtraLargeHover = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeHover");
                Texture2D buttonExtraLargeDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDisabled");

                MenuButton tryAgainButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, tryAgainButtonText);
                MenuButton mainMenuButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, mainMenuButtonText);

                Texture2D menuBackgroundImage = Content.Load<Texture2D>("Backgrounds/Background_Menu_Pattern3");

                tryAgainButton.Selected += new EventHandler<EventArgs>(restartButton_Selected);
                mainMenuButton.Selected += new EventHandler<EventArgs>(mainMenuButton_Selected);

                AddMenuControl(tryAgainButton);
                AddMenuControl(mainMenuButton);

                // load the background
                MenuBackground = new MenuBackground(menuBackgroundImage, new Rectangle(0, 0, Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height));

                ButtonSpacing = 15;
            }
        }

        private void exitButton_Selected(object sender, EventArgs e)
        {
            OnExitButtonClicked(sender, e);
        }

        private void mainMenuButton_Selected(object sender, EventArgs e)
        {
            OnMainMenuButtonClicked(sender, e);
        }

        private void restartButton_Selected(object sender, EventArgs e)
        {
            OnRestartButtonClicked(sender, e);
        }

        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            if (ExitButtonClicked != null)
                ExitButtonClicked(sender, e);
        }

        private void OnMainMenuButtonClicked(object sender, EventArgs e)
        {
            if (MainMenuButtonClicked != null)
                MainMenuButtonClicked(sender, e);
        }

        private void OnRestartButtonClicked(object sender, EventArgs e)
        {
            if (RestartButtonClicked != null)
                RestartButtonClicked(sender, e);
        }
    }
}
