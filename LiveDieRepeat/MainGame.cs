using System;
using LiveDieRepeat.Engine;
using LiveDieRepeat.Entities;
using LiveDieRepeat.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        private const int DEFAULT_ACTUAL_RESOLUTION_WIDTH = 1280;
        private const int DEFAULT_ACTUAL_RESOLUTION_HEIGHT = 720;
        private const int VIRTUAL_RESOLUTION_WIDTH = 1280;
        private const int VIRTUAL_RESOLUTION_HEIGHT = 720;
		private const bool DEFAULT_IS_FULL_SCREEN = true;

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        Camera camera = new Camera();

        private GameState currentGameState;
        private enum GameState
        {
            Title,
            Options,
            MainMenu,
            InGame,
            Paused,
            GameOver
        }

        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;
            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT);
            Resolution.SetResolution(DEFAULT_ACTUAL_RESOLUTION_WIDTH, DEFAULT_ACTUAL_RESOLUTION_HEIGHT, DEFAULT_IS_FULL_SCREEN);

            Window.Title = "Chains of Acadia";
            Content.RootDirectory = "Content";

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            //currentGameState = GameState.MainMenu;
            //MainMenu mainMenuScreen = CreateMainMenuScreen();

            currentGameState = GameState.Title;
            TitleScreen titleScreen = CreateTitleScreeen();
            screenManager.AddScreen(titleScreen);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera.Initialize();

            // i want to see my crosshairs!
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            Point mousePosition = MouseHelper.GetCurrentMousePosition();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Resolution.BeginDraw();

            base.Draw(gameTime);
        }

        #region Helper Functions

        //  a handy little function that gives a random float between two
        // values. This will be used in several places in the sample, in particilar in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private void SetGameState(GameState gameState)
        {
            currentGameState = gameState;

            if (gameState == GameState.InGame)
            {
                InGame inGameScreen = CreateInGameScreen();
                LoadingScreen.Load(screenManager, true, inGameScreen);
            }
            else if (gameState == GameState.MainMenu)
            {
                MainMenu mainMenuScreen = CreateMainMenuScreen();
                LoadingScreen.Load(screenManager, false, mainMenuScreen);
            }
            else if (gameState == GameState.GameOver)
            {
                GameOver gameOverScreen = CreateGameOverScreen();
                LoadingScreen.Load(screenManager, false, gameOverScreen);
            }
            else if (gameState == GameState.Title)
            {
                TitleScreen titleScreen = CreateTitleScreeen();
                LoadingScreen.Load(screenManager, false, titleScreen);
            }
            else if (gameState == GameState.Options)
            {
                OptionsMenu optionsScreen = CreateOptionsScreen();
                LoadingScreen.Load(screenManager, false, optionsScreen);
            }
        }

        private GameOver CreateGameOverScreen()
        {
            GameOver gameOver = new GameOver();
            gameOver.RestartButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        SetGameState(GameState.InGame);
                    });

            gameOver.MainMenuButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        SetGameState(GameState.MainMenu);
                    });

            gameOver.ExitButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        Exit();
                    });

            return gameOver;
        }

        private InGame CreateInGameScreen()
        {
            // setup the explosion component
            ExplosionParticleSystem explosionSystem = new ExplosionParticleSystem(this, 1);

            // setup the entity manager
            EntityManager entityManager = new EntityManager(graphics.GraphicsDevice);
            entityManager.EnemyKilledEvent +=
                new EventHandler<EnemyKilledEventArgs>(
                    delegate(object sender, EnemyKilledEventArgs e)
                    {
                        //explosionSystem.AddParticles(e.Position);
                    }
                );

            entityManager.PlayerDeathEvent +=
                new EventHandler<PlayerDeathEventArgs>(
                    delegate(object sender, PlayerDeathEventArgs e)
                    {
                        if (e.PlayerLives <= 1)
                            SetGameState(GameState.GameOver);
                    }
                );

            UserInterfaceManager uiManager = new UserInterfaceManager();

            // setup the in game screen
            InGame inGameScreen = new InGame(entityManager, explosionSystem, uiManager, camera);

            inGameScreen.ReturnToMainMenu +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        SetGameState(GameState.MainMenu);
                    }
                );

            return inGameScreen;
        }

        private MainMenu CreateMainMenuScreen()
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.PlayButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        SetGameState(GameState.InGame);
                    });

            mainMenu.OptionsButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        SetGameState(GameState.Options);
                    });

            mainMenu.ExitButtonClicked +=
                new EventHandler<EventArgs>(
                    delegate(object sender, EventArgs e)
                    {
                        Exit();
                    });

            return mainMenu;
        }

        private TitleScreen CreateTitleScreeen()
        {
            TitleScreen titleScreen = new TitleScreen();
            titleScreen.PressAnyKeyEvent += new EventHandler<EventArgs>(
                delegate(object sender, EventArgs e)
                {
                    SetGameState(GameState.MainMenu);
                });
            return titleScreen;
        }

        private OptionsMenu CreateOptionsScreen()
        {
            OptionsMenu optionsScreen = new OptionsMenu();
            optionsScreen.BackButtonClicked += new EventHandler<EventArgs>(
                delegate(object sender, EventArgs e)
                {
                    SetGameState(GameState.MainMenu);
                });
            return optionsScreen;
        }

        #endregion
    }
}
