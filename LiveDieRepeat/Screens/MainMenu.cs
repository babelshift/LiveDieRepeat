using System;
using LiveDieRepeat.Engine;
using LiveDieRepeat.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Screens
{
    public class MainMenu : MenuScreen
    {
        public event EventHandler<EventArgs> PlayButtonClicked;
        public event EventHandler<EventArgs> OptionsButtonClicked;
        public event EventHandler<EventArgs> ExitButtonClicked;

        public MainMenu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                // load textures
                MenuHeaderImage = Content.Load<Texture2D>("User Interface/ChainsOfAcadia3");

                Texture2D startButtonText = Content.Load<Texture2D>("User Interface/Controls/MainMenuStartButton");
                Texture2D optionsButtonText = Content.Load<Texture2D>("User Interface/Controls/MainMenuOptionsButton");
                Texture2D exitButtonText = Content.Load<Texture2D>("User Interface/Controls/MainMenuExitButton");

                Texture2D buttonExtraLargeNormal = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDefault");
                Texture2D buttonExtraLargeSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeSelected");
                Texture2D buttonExtraLargeHover = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeHover");
                Texture2D buttonExtraLargeDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDisabled");

                Texture2D menuBackgroundImage = Content.Load<Texture2D>("Backgrounds/Background_Menu_Pattern3");

                MenuButton startButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, startButtonText);
                MenuButton optionsButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, optionsButtonText);
                MenuButton exitButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, exitButtonText);

                startButton.Selected += new EventHandler<EventArgs>(playButton_Selected);
                optionsButton.Selected += new EventHandler<EventArgs>(optionsButton_Selected);
                exitButton.Selected += new EventHandler<EventArgs>(exitButton_Selected);

                // disabled for windows store because the only thing this menu gives is the ability to change resolutions (which is pointless for a windows store app)
                optionsButton.IsEnabled = false;

                AddMenuControl(startButton);
                AddMenuControl(optionsButton);
                AddMenuControl(exitButton);

                // load background
                MenuBackground = new MenuBackground(menuBackgroundImage, new Rectangle(0, 0, Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height));

                ButtonSpacing = 15;

                SpriteFont fontLabel = Content.Load<SpriteFont>("Fonts/ResagokrBold16");
                
                Label labelVersion = new Label(fontLabel, new Vector2(0, Resolution.VirtualViewport.Height), Color.White);
                labelVersion.Text = GetParsedAssemblyVersion();
                labelVersion.Position = new Vector2(labelVersion.Position.X + 5, labelVersion.Position.Y - labelVersion.Height);

                Label labelName = new Label(fontLabel, new Vector2(Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height), Color.White);
                labelName.Text = "(C) 2012 Justin Skiles";
                labelName.Position = new Vector2(labelName.Position.X - labelName.Width - 5, labelName.Position.Y - labelName.Height);

                AddMiscControl(labelVersion);
                AddMiscControl(labelName);
            }
        }

        public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

            PulseMenuHeaderSize();
        }

        #region Button Events

        /// <summary>When the exit button is clicked, call the local event to bubble up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_Selected(object sender, EventArgs e)
        {
            OnExitButtonClicked(sender, e);
        }

        /// <summary>When the options button is clicked, call the local event to bubble up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsButton_Selected(object sender, EventArgs e)
        {
            OnOptionsButtonClicked(sender, e);
        }

        /// <summary>When the play button is clicked, call the local event to bubble up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Selected(object sender, EventArgs e)
        {
            OnPlayButtonClicked(sender, e);
        }

        private void OnPlayButtonClicked(object sender, EventArgs e)
        {
            if (PlayButtonClicked != null)
                PlayButtonClicked(sender, e);
        }

        private void OnOptionsButtonClicked(object sender, EventArgs e)
        {
            if (OptionsButtonClicked != null)
                OptionsButtonClicked(sender, e);
        }

        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            if (ExitButtonClicked != null)
                ExitButtonClicked(sender, e);
        }

        // todo: best place for this?
        private String GetParsedAssemblyVersion()
        {
            //var version = Windows.ApplicationModel.Package.Current.Id.Version;
            //return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString();
            //return Windows.Storage.ApplicationData.Current.Version.ToString();
			String assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			String[] splitVersion = assemblyVersion.Split('.');
			String displayedVersion = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2] + "a_" + splitVersion[3];
			return displayedVersion;
            //return String.Empty;
        }

        #endregion
    }
}
