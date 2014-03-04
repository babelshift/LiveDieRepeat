using System;
using LiveDieRepeat.Entities;
using LiveDieRepeat.Items;
using LiveDieRepeat.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.Engine
{
    public class UserInterfaceManager
    {
        #region Prerequisistes

        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        #endregion

        #region UI Elements

        private PlayerStatusBar playerStatusBar;
        private DialogBox dialogBox;
        private Portrait portrait;
        private HealthBar healthBar;
        private ShieldBar shieldBar;
        private PowerBar experienceBar;
        private WeaponSlotBar weaponSlots;
        private Label labelFps;

        #endregion

        #region Positioning Variables

        private static Vector2 playerStatusBarPosition = new Vector2(10, 10);
        private static Vector2 healthBarPosition = new Vector2(playerStatusBarPosition.X + 30, playerStatusBarPosition.Y + 11);
        private static Vector2 shieldBarPosition = new Vector2(playerStatusBarPosition.X + 29, playerStatusBarPosition.Y + 24);
        private static Vector2 powerBarPosition = new Vector2(playerStatusBarPosition.X + 29, playerStatusBarPosition.Y + 38);

        private const int spaceBetweenPlayerStatusAndSlots = 10;

        #endregion

        #region Event Handlers

        public event EventHandler<WeaponChangedEventArgs> WeaponChangedEvent;

        #endregion

        #region Game Loop

        public void Initialize(GameServiceContainer gameServices, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            LoadContent(gameServices);
        }

        public void LoadContent(GameServiceContainer gameServices)
        {
            if (content == null)
                content = new ContentManager(gameServices, "Content");

            Texture2D portraitImage = content.Load<Texture2D>("User Interface/Portraits/Robot01");

            playerStatusBar = CreatePlayerStatusBar();
            dialogBox = CreateDialogBox();
            portrait = CreatePortrait(portraitImage, "Adam");

            dialogBox.Position = new Vector2(portrait.Position.X + portrait.Bounds.Width + 3, Resolution.VirtualViewport.Height);
            dialogBox.AddMessageText("Initializing walk sequence... walk sequence engaged.");
            dialogBox.AddMessageText("Initializing morality sequence... morality sequence missing.");
            dialogBox.AddMessageText("Initializing battle sequence... battle sequence engaged.");
            //dialogBox.AddMessageText("Ha! They're working! They're working!");
            //dialogBox.AddMessageText("Wait until my colleagues finally realize I'm not crazy!");

            healthBar = new HealthBar(content, healthBarPosition);
            shieldBar = new ShieldBar(content, shieldBarPosition);
            experienceBar = new PowerBar(content, powerBarPosition, null);

            Vector2 weaponSlotsPosition = new Vector2(playerStatusBar.Position.X + playerStatusBar.Width + spaceBetweenPlayerStatusAndSlots, playerStatusBar.Position.Y);
            weaponSlots = new WeaponSlotBar(content, weaponSlotsPosition, graphicsDevice);
            weaponSlots.WeaponChangedEvent += new EventHandler<EventArgs>(weaponSlots_WeaponChangedEvent);

            SpriteFont fontLabel = content.Load<SpriteFont>("Fonts/ResagokrBold16");
            labelFps = new Label(fontLabel, Vector2.Zero, Color.White);
            labelFps.Position = new Vector2(5, Resolution.VirtualViewport.Height - labelFps.Height);
        }

        public void UnloadContent()
        {
            content.Unload();
        }

        public void Update(GameTime gameTime)
        {
            // calculate frames per second
            // convert time since last update to seconds and divide 1 second by that amount to get the number of updates in a second
            labelFps.Text = Math.Round((1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000)), 4).ToString() + " fps";

            portrait.Update(gameTime);
            dialogBox.Update(gameTime);
            weaponSlots.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float transitionAlpha)
        {
            Color transitionColor = new Color(transitionAlpha, transitionAlpha, transitionAlpha);

            //portrait.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            //dialogBox.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            playerStatusBar.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            healthBar.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            shieldBar.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            experienceBar.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            weaponSlots.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
            labelFps.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
        }

        #endregion

        #region Create UI Elements

        private Portrait CreatePortrait(Texture2D portraitImage, String portraitTitle)
        {
            Portrait newPortrait = new Portrait(content, portraitImage, portraitTitle, Vector2.Zero);
            newPortrait.Position = new Vector2(Resolution.VirtualViewport.Width / 2 - (newPortrait.Bounds.Width + dialogBox.Bounds.Width) / 2, Resolution.VirtualViewport.Height);
            return newPortrait;
        }

        private DialogBox CreateDialogBox()
        {
            dialogBox = new DialogBox(content);
            return dialogBox;
        }

        private PlayerStatusBar CreatePlayerStatusBar()
        {
            playerStatusBar = new PlayerStatusBar(content, playerStatusBarPosition);
            return playerStatusBar;
        }

        #endregion

        #region Update UI Elements

        /// <summary>Sets the health bar to fill its bubble with color up to the percent passed. This is used when health is either gained or lost.
        /// </summary>
        /// <param name="healthPercentRemaining"></param>
        public void UpdateHealthBar(float healthPercentRemaining)
        {
            healthBar.PercentFilled = healthPercentRemaining;
        }

        public void ResetHealth()
        {
            healthBar.Reset();
        }

        /// <summary>Sets the shield bar to fill its bubble with color up to the percent passed. This is used when shields are either gained or lost.
        /// </summary>
        /// <param name="shieldPercentRemaining"></param>
        public void UpdateShieldBar(float shieldPercentRemaining)
        {
            shieldBar.PercentFilled = shieldPercentRemaining;
        }

        public void ResetShield()
        {
            shieldBar.Reset();
        }

        /// <summary>Sets the experience bar to fill its bubble with color up to the percent passed. This is used when experience for a weapon is either gained or lost.
        /// </summary>
        /// <param name="experiencePercentGained"></param>
        public void UpdateExperienceBar(float experiencePercentGained)
        {
            experienceBar.PercentFilled = experiencePercentGained;
        }

        public void ResetPower()
        {
            experienceBar.Reset();
        }

        /// <summary>Adds a weapon slot to the UI weapon slot bar based on the passed weapon and item. This is used when the player received a new item with an associated weapon.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="item"></param>
        public void AddWeaponSlot(Weapon weapon, ItemEntity item)
        {
            weapon.WeaponGainedExperienceEvent += new EventHandler<WeaponGainedExperienceEventArgs>(weapon_WeaponGainedExperienceEvent);
            weaponSlots.AddWeapon(weapon, item);
        }

        #endregion

        #region Events

        /// <summary>When the player changes weapons, change the experience bar to reflect the new weapon's experience. We also fire an event that will end up notifying the player
        /// that he should swap to a weapon at a specific index in the weapon collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWeaponChangedEvent(object sender, WeaponChangedEventArgs e)
        {
            UpdateExperienceBar(weaponSlots.SelectedWeapon.PowerUpPercentThisLevel);

            if (WeaponChangedEvent != null)
                WeaponChangedEvent(sender, e);
        }

        /// <summary>When the player does something that causes the weapon to gain experience, update the experience bar to reflect the new percent of experience gained.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void weapon_WeaponGainedExperienceEvent(object sender, WeaponGainedExperienceEventArgs e)
        {
            UpdateExperienceBar(e.PowerPercentGained);
        }

        private void weaponSlots_WeaponChangedEvent(object sender, EventArgs e)
        {
            OnWeaponChangedEvent(sender, new WeaponChangedEventArgs(weaponSlots.SelectedSlotIndex));
        }

        #endregion
    }

    /// <summary>When a weapon is changed by the player, pass the index of this weapon to the player so that it can take action to switch to the correct weapon in his list of weapons
    /// </summary>
    public class WeaponChangedEventArgs : EventArgs
    {
        public int WeaponSlotIndex { get; private set; }

        public WeaponChangedEventArgs(int weaponSlotIndex)
        {
            WeaponSlotIndex = weaponSlotIndex;
        }
    }
}
