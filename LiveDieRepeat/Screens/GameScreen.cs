using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.Screens
{
	public class GameScreen : Screen
	{
		float tileOffset = 0;
		private TimeSpan gameTime;
		float angle = 0;
		Vector mousePosition = Vector.Zero;


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

		private Icon iconPlayer;
		private Icon centerRing;

		private List<Icon> enemies = new List<Icon>();
		private List<Icon> bullets = new List<Icon>();

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

			iconPlayer = ControlFactory.CreateIcon(ContentManager, "Player");
			iconPlayer.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2);

			textureBackgroundTile = ContentManager.GetTexture("SplashBackgroundTile");

			Controls.Add(gameBoard);
			Controls.Add(metricScore);
			Controls.Add(metricTime);
			Controls.Add(timeValue);
			Controls.Add(timeLabel);
			Controls.Add(scoreLabel);
			Controls.Add(scoreValue);
			Controls.Add(centerRing);
		}

		public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

			this.gameTime = this.gameTime.Add(gameTime.ElapsedGameTime);

			timeValue.Text = String.Format("{0:00}.{1:00}", (int)this.gameTime.TotalSeconds, this.gameTime.Milliseconds);

			PointPlayerAtMouse();

			foreach (var enemy in enemies)
				enemy.Update(gameTime);

			foreach (var bullet in bullets)
				bullet.Update(gameTime);

			int randomX = random.Next(0, 1200);
			int randomY = random.Next(100, 700);
			Vector randomPosition = new Vector(randomX, randomY);
			Icon newEnemy = ControlFactory.CreateIcon(ContentManager, "Enemy");
			newEnemy.Position = randomPosition;
			enemies.Add(newEnemy);
		}

		private Random random = new Random();

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			for (int x = -1; x <= MainGame.SCREEN_WIDTH_LOGICAL / textureBackgroundTile.Width; x++)
				for (int y = -1; y <= MainGame.SCREEN_HEIGHT_LOGICAL / textureBackgroundTile.Height; y++)
					textureBackgroundTile.Draw((x + tileOffset) * textureBackgroundTile.Width, (y + tileOffset) * textureBackgroundTile.Height);

			
			base.Draw(gameTime, renderer);

			foreach (var enemy in enemies)
				enemy.Draw(gameTime, renderer);

			foreach (var bullet in bullets)
				bullet.Draw(gameTime, renderer);

			iconPlayer.TextureFrame.Draw((int)iconPlayer.Position.X, (int)iconPlayer.Position.Y, angle, Vector.Zero);
		}

		public override void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			base.HandleMouseMovingEvent(sender, e);

			mousePosition = new Vector(e.RelativeToWindowX, e.RelativeToWindowY);
		}

		public override void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e)
		{
			base.HandleMouseButtonPressedEvent(sender, e);

			Icon bullet = ControlFactory.CreateIcon(ContentManager, "PlayerBullet");
			bullet.Position = new Vector(e.RelativeToWindowX, e.RelativeToWindowY);
			bullets.Add(bullet);
		}

		private void PointPlayerAtMouse()
		{
			angle = (float)Math.Atan2((double)(mousePosition.Y - iconPlayer.Position.Y), (double)(mousePosition.X - iconPlayer.Position.X));
			angle *= (float)(180 / Math.PI);
			angle -= 45;
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
