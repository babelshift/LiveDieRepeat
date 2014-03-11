using LiveDieRepeat.Content;
using LiveDieRepeat.Screens;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public class MainGame : Game
	{
		public static readonly int SCREEN_WIDTH_LOGICAL = 1280;
		public static readonly int SCREEN_HEIGHT_LOGICAL = 720;

		private bool isMouseInsideWindowBounds = true;
		private bool isWindowFocused = true;
		private ContentManager contentManager;
		private ScreenManager screenManager;
		private ScreenFactory screenFactory;
		private GameState currentGameState;

		private GameState CurrentGameState
		{
			get { return currentGameState; }
			set { currentGameState = value; }
		}

		public MainGame()
		{
			WindowEntered += (sender, e) => isMouseInsideWindowBounds = true;
			WindowLeave += (sender, e) => isMouseInsideWindowBounds = false;
			WindowFocusLost += (sender, e) => isWindowFocused = false;
			WindowFocusGained += (sender, e) => isWindowFocused = true;
			TextInputting += (sender, e) => screenManager.HandleTextInputtingEvent(sender, e);
			MouseButtonReleased += (sender, e) => screenManager.HandleMouseButtonReleasedEvent(sender, e);
			MouseButtonPressed += (sender, e) => screenManager.HandleMouseButtonPressedEvent(sender, e);
			MouseMoving += (sender, e) => screenManager.HandleMouseMovingEvent(sender, e);
			KeyPressed += (sender, e) => screenManager.HandleKeyPressedEvent(sender, e);
			KeyReleased += (sender, e) => screenManager.HandleKeyReleasedEvent(sender, e);
		}

		protected override void Initialize()
		{
			base.Initialize();

			CreateWindow("Live. Die. Repeat.", 100, 100, SCREEN_WIDTH_LOGICAL, SCREEN_HEIGHT_LOGICAL, WindowFlags.Shown);// | WindowFlags.GrabbedInputFocus);
			CreateRenderer(RendererFlags.RendererAccelerated | RendererFlags.RendererPresentVSync | RendererFlags.SupportRenderTargets);
			Renderer.SetRenderLogicalSize(SCREEN_WIDTH_LOGICAL, SCREEN_HEIGHT_LOGICAL);
			
			contentManager = new ContentManager(Renderer);
			screenManager = new ScreenManager(Renderer);
			screenManager.Initialize();
			screenFactory = new ScreenFactory();

			LoadSplashMenuScreen();
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (isWindowFocused)
			{
				base.Update(gameTime);
				screenManager.Update(gameTime, !isWindowFocused, isMouseInsideWindowBounds);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			if (isWindowFocused)
			{
				base.Draw(gameTime);
				screenManager.Draw(gameTime, Renderer);
				Renderer.RenderPresent();
			}
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		private void LoadScreen(Screen screen)
		{
			if (screen != null)
				LoadingScreen.Load(contentManager, screenManager, false, screen);
		}

		private SplashMenuScreen CreateSplashMenuScreen()
		{
			SplashMenuScreen splashMenuScreen = new SplashMenuScreen(contentManager);
			splashMenuScreen.QuitClicked += (sender, e) => Quit();
			splashMenuScreen.StartClicked += (sender, e) => LoadGameScreen();
			return splashMenuScreen;
		}

		private GameScreen CreateGameScreen()
		{
			GameScreen gameScreen = new GameScreen(contentManager);
			return gameScreen;
		}

		private void LoadSplashMenuScreen()
		{
			CurrentGameState = GameState.Splash;
			Screen screen = CreateSplashMenuScreen();
			LoadScreen(screen);
		}

		private void LoadGameScreen()
		{
			CurrentGameState = GameState.InGame;
			Screen screen = CreateGameScreen();
			LoadScreen(screen);
		}
	}
}
