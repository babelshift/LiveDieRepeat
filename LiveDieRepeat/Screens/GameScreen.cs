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
		private TimeSpan gameTime;
		private Vector mousePosition = Vector.Zero;
		private int score = 0;
		private Random random = new Random();
		private TimeSpan timeSinceLastPlayerBullet = TimeSpan.MaxValue;
		private TimeSpan timeBetweenPlayerBullets = TimeSpan.FromSeconds(0.1);
		private bool isLeftMouseButtonDown = false;
		private BulletMLParser parser;
		private int bulletTimer = 0;
		private BulletMover bulletMover;

		private Icon gameBoard;
		private Texture textureBackgroundTile;
		private Icon metricScore;
		private Icon metricTime;

		private Label timeLabel;
		private Label timeValue;
		private Label bestTimeLabel;
		private Label bestTimeValue;
		private Label scoreLabel;
		private Label scoreValue;
		private Label bestScoreLabel;
		private Label bestScoreValue;

		private Player player;
		private Icon centerRing;

		private List<Enemy> enemies = new List<Enemy>();
		private List<Bullet> playerBullets = new List<Bullet>();

		public GameScreen(ContentManager contentManager)
			: base(contentManager)
		{
		}

		public override void Activate(Renderer renderer)
		{
			base.Activate(renderer);

			gameBoard = ControlFactory.CreateIcon(ContentManager, "InGameBoard");
			gameBoard.Position = new Vector(0, 59);

			metricScore = ControlFactory.CreateIcon(ContentManager, "InGameMetricLeft");
			metricScore.Position = new Vector(5, 5);
			metricTime = ControlFactory.CreateIcon(ContentManager, "InGameMetricRight");
			metricTime.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL - metricTime.Width - 5, 5);

			timeLabel = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "Time");
			timeLabel.Position = metricTime.Position + new Vector(metricTime.Width - timeLabel.Width - 10, 4);
			timeValue = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "00:00");
			timeValue.Position = metricTime.Position + new Vector(10, 4);

			scoreLabel = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "Score");
			scoreLabel.Position = metricScore.Position + new Vector(10, 4);
			scoreValue = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "0000");
			scoreValue.Position = metricScore.Position + new Vector(metricScore.Width - scoreValue.Width - 10, 4);

			centerRing = ControlFactory.CreateIcon(ContentManager, "CenterRing");
			centerRing.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - centerRing.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2 - centerRing.Height / 2);

			Icon iconPlayer = ControlFactory.CreateIcon(ContentManager, "Player");
			player = new Player(iconPlayer);
			player.TeleportTo(new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2));

			textureBackgroundTile = ContentManager.GetTexture("SplashBackgroundTile");

			Controls.Add(gameBoard);
			Controls.Add(metricScore);
			Controls.Add(metricTime);
			Controls.Add(timeValue);
			Controls.Add(timeLabel);
			Controls.Add(scoreLabel);
			Controls.Add(scoreValue);
			Controls.Add(centerRing);

			parser = new BulletMLParser();
			parser.ParseXML(String.Format(@"Content\BulletPatterns\{0}", "Struggling.xml"));
			BulletMLManager.Init(new MyBulletSystem(player));
			BulletMoverManager.Init(ContentManager);

			Vector bulletPosition = new Vector(400, 400);
			bulletMover = BulletMoverManager.CreateBulletMover(bulletPosition, parser.tree);
		}

		public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

			this.gameTime = this.gameTime.Add(gameTime.ElapsedGameTime);

			timeValue.Text = String.Format("{0:00}.{1:00}", (int)this.gameTime.TotalSeconds, this.gameTime.Milliseconds);

			UpdatePlayer();

			UpdateEnemies(gameTime);

			UpdatePlayerBullets(gameTime);

			ShootPlayerBullet(gameTime);

			//CreateRandomEnemy();

			UpdateBullets(gameTime);

			UpdateCollisions();

			RemoveExpiredAgents();
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			for (int x = -1; x <= MainGame.SCREEN_WIDTH_LOGICAL / textureBackgroundTile.Width; x++)
				for (int y = -1; y <= MainGame.SCREEN_HEIGHT_LOGICAL / textureBackgroundTile.Height; y++)
					textureBackgroundTile.Draw((x + tileOffset) * textureBackgroundTile.Width, (y + tileOffset) * textureBackgroundTile.Height);

			base.Draw(gameTime, renderer);

			foreach (var enemy in enemies)
				enemy.Draw(gameTime, renderer);

			player.Draw(gameTime, renderer);

			foreach (var playerBullet in playerBullets)
				playerBullet.Draw(gameTime, renderer);

			foreach (var bulletMover in BulletMoverManager.BulletMovers)
				bulletMover.Draw(gameTime, renderer);
		}

		public override void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			base.HandleMouseMovingEvent(sender, e);

			mousePosition = new Vector(e.RelativeToWindowX, e.RelativeToWindowY);
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

		public void ShootPlayerBullet(GameTime gameTime)
		{
			if (timeSinceLastPlayerBullet < TimeSpan.MaxValue)
				timeSinceLastPlayerBullet += gameTime.ElapsedGameTime;

			if (timeSinceLastPlayerBullet >= timeBetweenPlayerBullets)
			{
				if (isLeftMouseButtonDown)
				{
					Bullet bullet = player.FireWeapon(ContentManager);
					playerBullets.Add(bullet);

					timeSinceLastPlayerBullet = TimeSpan.Zero;
				}
			}
		}

		private void UpdateScore(int points)
		{
			score += points;
			scoreValue.Text = score.ToString("0000");
		}

		private void RemoveExpiredAgents()
		{
			int deadEnemyCount = enemies.Count(e => e.IsDead);
			UpdateScore(deadEnemyCount);
			playerBullets.RemoveAll(pb => pb.IsDead);
			enemies.RemoveAll(e => e.IsDead);
		}

		private void UpdateCollisions()
		{
			CollisionManager.HandleCollisions(enemies, playerBullets);
		}

		private void UpdatePlayerBullets(GameTime gameTime)
		{
			foreach (var playerBullet in playerBullets)
				playerBullet.Update(gameTime);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			foreach (var enemy in enemies)
				enemy.Update(gameTime);

		}

		private void UpdateBullets(GameTime gameTime)
		{
			bulletTimer++;
			if (bulletTimer > 60)
			{
				bulletTimer = 0;
				bulletMover.IsUsed = false;

				if (bulletMover.IsUsed == false)
				{
					Vector bulletPosition = new Vector(400, 400);
					bulletMover = BulletMoverManager.CreateBulletMover(bulletPosition, parser.tree);
				}
			}

			BulletMoverManager.Update(gameTime);
			BulletMoverManager.FreeBulletMovers();
		}

		private void CreateRandomEnemy()
		{
			if (enemies.Count < 500)
			{
				int randomX = random.Next(0, 1200);
				int randomY = random.Next(100, 700);
				Vector randomPosition = new Vector(randomX, randomY);
				Icon iconEnemy = ControlFactory.CreateIcon(ContentManager, "Enemy");
				Enemy newEnemy = new Enemy(new Vector(400, 400), iconEnemy);
				newEnemy.TeleportTo(randomPosition);
				enemies.Add(newEnemy);
			}
		}

		private void DisposeDeadBullets(List<Bullet> deadBullets)
		{
			foreach (var deadBullet in deadBullets)
			{
				playerBullets.RemoveAll(pb => pb.ID == deadBullet.ID);
				deadBullet.Dispose();
			}
		}

		private static void MarkDeadBullets(List<Bullet> deadBullets, Bullet playerBullet)
		{
			if (playerBullet.Position.Y <= 59)
			{
				playerBullet.Die();
				deadBullets.Add(playerBullet);
			}
		}

		private void UpdatePlayer()
		{
			float angle = (float)Math.Atan2((double)(mousePosition.Y - player.Position.Y), (double)(mousePosition.X - player.Position.X));
			angle = MathExtensions.ToDegrees(angle);
			angle -= 45;
			player.RotateTo(angle);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}