using LiveDieRepeat.Content;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.Screens
{
	public class DeathScreen : Screen
	{
		public DeathScreen(ContentManager contentManager)
			: base(contentManager)
		{
			IsPopup = true;
		}

		public override void Activate(Renderer renderer)
		{
			base.Activate(renderer);
		}

		public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			base.Draw(gameTime, renderer);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
