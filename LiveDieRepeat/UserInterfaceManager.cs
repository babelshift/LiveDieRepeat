using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public class UserInterfaceManager : IDisposable
	{
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

		private ContentManager contentManager;

		private List<Control> controls = new List<Control>();

		public UserInterfaceManager(ContentManager contentManager)
		{
			this.contentManager = contentManager;

			metricScore = ControlFactory.CreateIcon(contentManager, "InGameMetricLeft");
			metricScore.Position = new Vector(5, 5);
			metricTime = ControlFactory.CreateIcon(contentManager, "InGameMetricRight");
			metricTime.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL - metricTime.Width - 5, 5);

			timeLabel = ControlFactory.CreateLabel(contentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "Time");
			timeLabel.Position = metricTime.Position + new Vector(metricTime.Width - timeLabel.Width - 10, 4);
			timeValue = ControlFactory.CreateLabel(contentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "00:00");
			timeValue.Position = metricTime.Position + new Vector(10, 4);

			scoreLabel = ControlFactory.CreateLabel(contentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "Score");
			scoreLabel.Position = metricScore.Position + new Vector(10, 4);
			scoreValue = ControlFactory.CreateLabel(contentManager, Styles.Fonts.Anton, 28, Styles.Colors.White, "0000");
			scoreValue.Position = metricScore.Position + new Vector(metricScore.Width - scoreValue.Width - 10, 4);

			controls.Add(metricScore);
			controls.Add(metricTime);
			controls.Add(timeValue);
			controls.Add(timeLabel);
			controls.Add(scoreLabel);
			controls.Add(scoreValue);
		}

		public void Update(GameTime gameTime, TimeSpan inGameTime)
		{
			timeValue.Text = String.Format("{0:00}.{1:00}", (int)inGameTime.TotalSeconds, inGameTime.Milliseconds);

			foreach (var control in controls)
				control.Update(gameTime);
		}

		public void Draw(GameTime gameTime, Renderer renderer)
		{
			foreach (var control in controls)
				control.Draw(gameTime, renderer);
		}

		public void UpdateScoreText(int score)
		{
			scoreValue.Text = score.ToString("0000");
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			foreach (var control in controls)
				control.Dispose();
		}
	}
}
