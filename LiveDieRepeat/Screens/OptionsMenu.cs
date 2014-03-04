using System;
using System.Collections.Generic;
using System.Linq;
using LiveDieRepeat.Engine;
using LiveDieRepeat.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Screens
{
    public class OptionsMenu : MenuScreen
    {
        private Checkbox checkBoxWindowDisplayType;
        private Spinbox spinboxResolution;

        public event EventHandler<EventArgs> BackButtonClicked;

        private List<Vector2> supportedResolutions;

        public OptionsMenu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                MenuHeaderImage = Content.Load<Texture2D>("User Interface/ChainsOfAcadia3");

                #region Load Normal Buttons

                Texture2D applyButtonText = Content.Load<Texture2D>("User Interface/Controls/OptionsMenuApplyButton");
                Texture2D mainMenuButtonText = Content.Load<Texture2D>("User Interface/Controls/OptionsMenuMainMenuButton");

                Texture2D buttonExtraLargeNormal = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDefault");
                Texture2D buttonExtraLargeSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeSelected");
                Texture2D buttonExtraLargeHover = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeHover");
                Texture2D buttonExtraLargeDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonExtraLargeDisabled");

                Texture2D menuBackgroundImage = Content.Load<Texture2D>("Backgrounds/Background_Menu_Pattern3");

                #endregion

                #region Load Checkbox

                SpriteFont buttonFont = Content.Load<SpriteFont>("Fonts/ResagokrBold18");
                Texture2D checkBoxDefault = Content.Load<Texture2D>("User Interface/Controls/CheckboxDefault");
                Texture2D checkBoxChecked = Content.Load<Texture2D>("User Interface/Controls/CheckboxChecked");
                checkBoxWindowDisplayType = new Checkbox("Fullscreen:", buttonFont, checkBoxDefault, checkBoxChecked, new Vector2(100, 100));
                checkBoxWindowDisplayType.IsChecked = Resolution.IsFullScreen;

                #endregion

                #region Load Spinbox

                // get supported resolutions and current resolution
                supportedResolutions = GetSupportedResolutions();
                Vector2 currentResolution = new Vector2(Resolution.Viewport.Width, Resolution.Viewport.Height);
                int currentSupportedResolutionIndex = supportedResolutions.IndexOf(currentResolution);

                // build list of resolutions
                List<String> resolutions = new List<string>();
                foreach(var supportedResolution in supportedResolutions)
                    resolutions.Add(GetResolutionText(supportedResolutions.IndexOf(supportedResolution)));

                Texture2D buttonLeftDefault = Content.Load<Texture2D>("User Interface/Controls/ButtonLeftDefault");
                Texture2D buttonLeftHovered = Content.Load<Texture2D>("User Interface/Controls/ButtonLeftHovered");
                Texture2D buttonLeftSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonLeftSelected");
                Texture2D buttonLeftDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonLeftDisabled");
                
                Texture2D buttonRightDefault = Content.Load<Texture2D>("User Interface/Controls/ButtonRightDefault");
                Texture2D buttonRightHovered = Content.Load<Texture2D>("User Interface/Controls/ButtonRightHovered");
                Texture2D buttonRightSelected = Content.Load<Texture2D>("User Interface/Controls/ButtonRightSelected");
                Texture2D buttonRightDisabled = Content.Load<Texture2D>("User Interface/Controls/ButtonRightDisabled");

                spinboxResolution = new Spinbox("Resolution:", buttonFont, resolutions, currentSupportedResolutionIndex, buttonLeftDefault, buttonLeftSelected, buttonLeftHovered, buttonLeftDisabled, buttonRightDefault, buttonRightSelected, buttonRightHovered, buttonRightDisabled);

                #endregion

                MenuButton applyButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, applyButtonText);
                MenuButton mainMenuButton = new MenuButton(buttonExtraLargeNormal, buttonExtraLargeHover, buttonExtraLargeSelected, buttonExtraLargeDisabled, mainMenuButtonText);

                mainMenuButton.Selected += new EventHandler<EventArgs>(backButton_Selected);
                applyButton.Selected += new EventHandler<EventArgs>(applyButton_Selected);

                AddMenuControl(spinboxResolution);
                AddMenuControl(checkBoxWindowDisplayType);
                AddMenuControl(applyButton);
                AddMenuControl(mainMenuButton);

                // load background
                MenuBackground = new MenuBackground(menuBackgroundImage, new Rectangle(0, 0, Resolution.VirtualViewport.Width, Resolution.VirtualViewport.Height));

                ButtonSpacing = 15;
            }
        }

        #region Button Events

        private void applyButton_Selected(object sender, EventArgs e)
        {
            Vector2 selectedResolution = new Vector2(supportedResolutions[spinboxResolution.CurrentIndex].X, supportedResolutions[spinboxResolution.CurrentIndex].Y);
            Resolution.SetResolution((int)selectedResolution.X, (int)selectedResolution.Y, checkBoxWindowDisplayType.IsChecked);
        }

        private void backButton_Selected(object sender, EventArgs e)
        {
            OnBackButtonClicked(sender, e);
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (BackButtonClicked != null)
                BackButtonClicked(sender, e);
        }

        #endregion

        #region Helper Methods

        private String GetResolutionText(int resolutionIndex)
        {
            Vector2 selectedResolution = new Vector2(supportedResolutions[resolutionIndex].X, supportedResolutions[resolutionIndex].Y);
            return String.Format("{0} x {1}", selectedResolution.X, selectedResolution.Y);
        }

        //todo: move to static resolution class?
        /// <summary>Returns an array of supported display modes by the default graphics adapter. Also removes duplicate resolutions.
        /// </summary>
        /// <returns></returns>
        private List<Vector2> GetSupportedResolutions()
        {
            List<DisplayMode> supportedDisplayModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToList<DisplayMode>();
            List<Vector2> displayModes = new List<Vector2>();
            foreach (DisplayMode displayMode in supportedDisplayModes)
            {
                Vector2 supportedResolution = new Vector2(displayMode.Width, displayMode.Height);
                if (!displayModes.Contains(supportedResolution))
                    displayModes.Add(supportedResolution);
            }

            return displayModes;
        }

        #endregion
    }
}