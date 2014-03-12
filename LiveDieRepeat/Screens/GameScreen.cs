using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
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

			//AdjustTileOffset();

			this.gameTime = this.gameTime.Add(gameTime.ElapsedGameTime);

			timeValue.Text = String.Format("{0:00}.{1:00}", (int)this.gameTime.TotalSeconds, this.gameTime.Milliseconds);

			angle += 5f;
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			for (int x = -1; x <= MainGame.SCREEN_WIDTH_LOGICAL / textureBackgroundTile.Width; x++)
				for (int y = -1; y <= MainGame.SCREEN_HEIGHT_LOGICAL / textureBackgroundTile.Height; y++)
					textureBackgroundTile.Draw((x + tileOffset) * textureBackgroundTile.Width, (y + tileOffset) * textureBackgroundTile.Height);

			
			base.Draw(gameTime, renderer);

			iconPlayer.TextureFrame.Draw((int)iconPlayer.Position.X, (int)iconPlayer.Position.Y, angle, new Vector(0, 0));
		}

		float angle = 0;

		private void AdjustTileOffset()
		{
			tileOffset += .005f;
			if (tileOffset >= 1)
				tileOffset = 0;
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
