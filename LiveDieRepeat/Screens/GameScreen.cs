using BulletMLLib;
using LiveDieRepeat.BulletSystem;
using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveDieRepeat.Screens
{
	public class GameScreen : Screen
	{
		private int tileOffset = 0;
		private TimeSpan inGameTime;
		private Vector mousePosition = Vector.Zero;
		private int score = 0;

		private int bulletTimer = 0;
		private BulletMLParser parser;
		private BulletMoverManager bulletMoverManager;

		private Icon gameBoard;
		private Texture textureBackgroundTile;

		private bool isArrowLeftDown = false;
		private bool isArrowRightDown = false;
		private bool isLeftMouseButtonDown = false;
		private bool isSpacebarDown = false;

		private UserInterfaceManager userInterfaceManager;
		private AgentManager agentManager;

		private Opponent opponent;

		public GameScreen(ContentManager contentManager)
			: base(contentManager)
		{
		}

		public override void Activate(Renderer renderer)
		{
			base.Activate(renderer);

			userInterfaceManager = new UserInterfaceManager(ContentManager);
			agentManager = new AgentManager(ContentManager);
			agentManager.ScoreNeedsUpdate += agentManager_ScoreNeedsUpdate;

			gameBoard = ControlFactory.CreateIcon(ContentManager, "InGameBoard");
			gameBoard.Position = new Vector(0, 59);

			textureBackgroundTile = ContentManager.GetTexture("SplashBackgroundTile");

			Controls.Add(gameBoard);

			bulletMoverManager = new BulletMoverManager(ContentManager);

			parser = new BulletMLParser();
			parser.ParseXML(String.Format(@"Content\BulletPatterns\{0}", "[Psyvariar]_X-B_colony_shape_satellite.xml"));

			Icon iconOpponent = ControlFactory.CreateIcon(ContentManager, "Enemy");
			opponent = new Opponent(Vector.Zero, iconOpponent);
			opponent.TeleportTo(new Vector(100, 100));
			opponent.BulletMover = bulletMoverManager.CreateBulletMover(opponent.Position, parser.tree);
		}

		private void agentManager_ScoreNeedsUpdate(object sender, ScoreNeedsUpdateEventArgs e)
		{
			UpdateScore(e.DeadEnemyCount);
		}

		public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

			inGameTime = inGameTime.Add(gameTime.ElapsedGameTime);

			userInterfaceManager.Update(gameTime, inGameTime);

			agentManager.Update(gameTime, isArrowLeftDown, isArrowRightDown, isLeftMouseButtonDown, isSpacebarDown);

			opponent.Update(gameTime);

			UpdateBullets(gameTime);

			UpdateCollisions();
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			for (int x = -1; x <= MainGame.SCREEN_WIDTH_LOGICAL / textureBackgroundTile.Width; x++)
				for (int y = -1; y <= MainGame.SCREEN_HEIGHT_LOGICAL / textureBackgroundTile.Height; y++)
					textureBackgroundTile.Draw((x + tileOffset) * textureBackgroundTile.Width, (y + tileOffset) * textureBackgroundTile.Height);

			base.Draw(gameTime, renderer);

			agentManager.Draw(gameTime, renderer);

			opponent.Draw(gameTime, renderer);

			bulletMoverManager.Draw(gameTime, renderer);

			userInterfaceManager.Draw(gameTime, renderer);
		}

		public override void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			base.HandleMouseMovingEvent(sender, e);

			//agentManager.HandleMouseMovingEvent(sender, e);
		}

		public override void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e)
		{
			base.HandleMouseButtonPressedEvent(sender, e);

			if (e.MouseButton == SharpDL.Input.MouseButtonCode.Left)
				isLeftMouseButtonDown = true;
		}

		public override void HandleMouseButtonReleasedEvent(object sender, MouseButtonEventArgs e)
		{
			base.HandleMouseButtonReleasedEvent(sender, e);

			if (e.MouseButton == SharpDL.Input.MouseButtonCode.Left)
				isLeftMouseButtonDown = false;
		}

		public override void HandleKeyPressedEvent(object sender, KeyboardEventArgs e)
		{
			base.HandleKeyPressedEvent(sender, e);

			var keyPressed = e.KeyInformation.VirtualKey;

			if (keyPressed == SharpDL.Input.VirtualKeyCode.ArrowLeft)
				isArrowLeftDown = true;
			else if (keyPressed == SharpDL.Input.VirtualKeyCode.ArrowRight)
				isArrowRightDown = true;

			if (keyPressed == SharpDL.Input.VirtualKeyCode.Space)
				isSpacebarDown = true;
		}

		public override void HandleKeyReleasedEvent(object sender, KeyboardEventArgs e)
		{
			base.HandleKeyReleasedEvent(sender, e);

			var keyReleased = e.KeyInformation.VirtualKey;

			if (keyReleased == SharpDL.Input.VirtualKeyCode.ArrowLeft)
				isArrowLeftDown = false;
			if (keyReleased == SharpDL.Input.VirtualKeyCode.ArrowRight)
				isArrowRightDown = false;
			if (keyReleased == SharpDL.Input.VirtualKeyCode.Space)
				isSpacebarDown = false;
		}

		private void UpdateScore(int points)
		{
			score += points;
			userInterfaceManager.UpdateScoreText(score);
		}

		private void UpdateCollisions()
		{
			CollisionManager.HandleCollisions(bulletMoverManager.Emitters, agentManager.PlayerBullets);
		}

		private void UpdateBullets(GameTime gameTime)
		{
			bulletTimer++;
			if (bulletTimer > 60)
			{
				bulletTimer = 0;
				opponent.BulletMover.IsUsed = false;
				opponent.BulletMover = bulletMoverManager.CreateBulletMover(opponent.Position, parser.tree);
			}

			bulletMoverManager.Update(gameTime);
		}

		public override void Dispose()
		{
			base.Dispose();

			gameBoard.Dispose();
			textureBackgroundTile.Dispose();

			agentManager.Dispose();
			userInterfaceManager.Dispose();
			bulletMoverManager.Dispose();
		}
	}
}