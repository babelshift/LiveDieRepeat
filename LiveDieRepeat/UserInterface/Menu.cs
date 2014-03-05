using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;

namespace LiveDieRepeat.UserInterface
{
	public class Menu : Control
	{
		protected readonly string defaultText = "N/A";
		private List<Control> controls = new List<Control>();

		protected IList<Control> Controls
		{
			get { return controls; }
		}

		public override void Update(GameTime gameTime)
		{
			if (Visible)
			{
				base.Update(gameTime);

				foreach (var control in controls)
					if (control != null)
						control.Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			if (Visible)
			{
				foreach (var control in controls)
					if (control != null)
						control.Draw(gameTime, renderer);
			}
		}

		public override void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e)
		{
			if (Visible)
			{
				foreach (var control in controls)
					if (control != null)
						control.HandleMouseButtonPressedEvent(sender, e);
			}
		}

		public override void HandleMouseButtonReleasedEvent(object sender, MouseButtonEventArgs e)
		{
			if (Visible)
			{
				foreach (var control in controls)
					if (control != null)
						control.HandleMouseButtonReleasedEvent(sender, e);
			}
		}

		public override void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			if (Visible)
			{
				foreach (var control in controls)
					if (control != null)
						control.HandleMouseMovingEvent(sender, e);
			}
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			foreach (var control in controls)
				if (control != null)
					control.Dispose();
			controls.Clear();
		}
	}
}