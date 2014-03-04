using System;
using LiveDieRepeat.Engine;
using LiveDieRepeat.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InGame : Screen
    {
        #region General Members

        private Camera camera;
        private ContentManager content;

        #endregion

        #region Child Components

        private UserInterfaceManager uiManager;
        private EntityManager entityManager;
        private ExplosionParticleSystem explosionSystem;
        private MapCollection currentLevel;

        #endregion

        #region User Interface
        private SpriteFont pointsFont;

        private const int POINT_INCREMENT_MONEY_RED = 100;
        private const int POWER_INCREMENT = 1;
        private const int SHIELD_GAIN_INCREMENT = 10;
        private const int SHIELD_HIT_DECREMENT = 1;
        private const int PLAYER_HIT_DECREMENT = 1;

        #endregion

        #region Misc

        private KeyboardState previousKeyboardState;
        private int score = 0;
        private float pauseAlpha;

        #endregion

        #region Event Handlers

        public event EventHandler<EventArgs> ReturnToMainMenu;

        #endregion

        #region Constructors

        public InGame(EntityManager entityManager, ExplosionParticleSystem explosionSystem, UserInterfaceManager uiManager, Camera camera)
        {
            this.camera = camera;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // TODO: Construct any child components here
            this.entityManager = entityManager;
            this.explosionSystem = explosionSystem;
            this.uiManager = uiManager;

            this.entityManager.PlayerHitEvent += new EventHandler<PlayerHitEventArgs>(entityManager_PlayerHitEvent);
            this.entityManager.PlayerDeathEvent += new EventHandler<PlayerDeathEventArgs>(entityManager_PlayerDeathEvent);
            this.entityManager.PlayerShieldedEvent += new EventHandler(entityManager_PlayerShieldedEvent);
            this.entityManager.PlayerPickedUpMoneyEvent += new EventHandler(entityManager_PlayerPickedUpMoneyEvent);
            this.entityManager.PlayerReceivedItemEvent += new EventHandler<PlayerReceivedItemEventArgs>(entityManager_PlayerPickedUpWeaponEvent);

            this.uiManager.WeaponChangedEvent += new EventHandler<WeaponChangedEventArgs>(uiManager_WeaponChangedEvent);
        }

        #endregion

        #region Game Loop

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                Vector2 playerSpawnPoint = new Vector2(Resolution.VirtualViewport.Width / 2, Resolution.VirtualViewport.Height / 2);
                PlayerEntity player = new PlayerEntity(playerSpawnPoint);

                currentLevel = content.Load<MapCollection>("Levels/L1");
                currentLevel.Initialize(content, player);

                camera.Position = currentLevel.CurrentMap.FocusPosition;
                camera.Focus = currentLevel.CurrentMap;

                uiManager.Initialize(ScreenManager.Game.Services, ScreenManager.GraphicsDevice);

                entityManager.Initialize(ScreenManager.Game.Services, camera, currentLevel, player);
                explosionSystem.Initialize();

                // points/money section
                pointsFont = content.Load<SpriteFont>("Fonts/Points");

                // simulate loading
                //Thread.Sleep(1000);
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Unload()
        {
            entityManager.Unload();
            content.Unload();
            uiManager.UnloadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                //scoreText.DisplayedText = String.Format("Points: {0}", score);

                camera.Update(gameTime);
                entityManager.Update(gameTime);
                explosionSystem.Update(gameTime);
                currentLevel.Update(gameTime, camera);
                uiManager.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform * Resolution.getTransformationMatrix());

            currentLevel.Draw(spriteBatch, camera);

            spriteBatch.End();

            entityManager.Draw(gameTime);
            explosionSystem.Draw(gameTime, camera);

            // need to use a separate begin/end for the UI because it should not be transformed by the camera's translation matrix
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());

            uiManager.Draw(spriteBatch, gameTime, TransitionAlpha);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion

        #region Helper Methods

        public override void HandleInput(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
                ScreenManager.AddScreen(CreatePauseScreen());

            previousKeyboardState = currentKeyboardState;
        }

        private PauseScreen CreatePauseScreen()
        {
            PauseScreen pauseScreen = new PauseScreen();
            pauseScreen.MenuButtonClicked += new EventHandler<EventArgs>(pauseScreen_MenuButtonClicked);
            return pauseScreen;
        }

        #endregion

        #region Events

        private void pauseScreen_MenuButtonClicked(object sender, EventArgs e)
        {
            OnReturnToMainMenu(sender, e);
        }

        private void OnReturnToMainMenu(object sender, EventArgs e)
        {
            if (ReturnToMainMenu != null)
                ReturnToMainMenu(sender, e);
        }

        private void entityManager_PlayerHitEvent(object sender, PlayerHitEventArgs e)
        {
            uiManager.UpdateShieldBar(e.ShieldPercentRemaining);
            uiManager.UpdateHealthBar(e.HealthPercentRemaining);
        }

        private void entityManager_PlayerDeathEvent(object sender, EventArgs e)
        {
            uiManager.ResetHealth();
            score = 0;
        }

        private void entityManager_PlayerShieldedEvent(object sender, EventArgs e)
        {
            uiManager.UpdateShieldBar(100);
        }

        private void entityManager_PlayerPickedUpMoneyEvent(object sender, EventArgs e)
        {
            score += POINT_INCREMENT_MONEY_RED;
        }

        private void entityManager_PlayerPickedUpWeaponEvent(object sender, PlayerReceivedItemEventArgs e)
        {
            uiManager.AddWeaponSlot(e.ItemReceived.WeaponAssociated, e.ItemReceived);
        }

        private void uiManager_WeaponChangedEvent(object sender, WeaponChangedEventArgs e)
        {
            entityManager.SwitchPlayerWeapon(e.WeaponSlotIndex);
        }

        #endregion
    }
}
