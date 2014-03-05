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
	public class SplashMenuScreen : Screen
	{
		private Label labelTitle;
		private Button buttonStart;

		public SplashMenuScreen(ContentManager contentManager)
			: base(contentManager)
		{
		}

		public event EventHandler StartClicked;

		public override void Activate(Renderer renderer)
		{
			base.Activate(renderer);

			labelTitle = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 60, Styles.Colors.White, "Live.Die.Repeat.");

			buttonStart = ControlFactory.CreateButton(ContentManager, "ButtonSplash");
			buttonStart.ButtonType = ButtonType.TextOnly;
			buttonStart.Label = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 30, Styles.Colors.White, "Click to Start");
			buttonStart.Position = new Vector(200, 200);

			AddControl(labelTitle);
			AddControl(buttonStart);
		}
	}
}
