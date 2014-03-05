using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.UserInterface
{
	public class Icon : Control
	{
		private Texture textureFrame;

		public Texture TextureFrame
		{
			get { return textureFrame; }
			private set
			{
				textureFrame = value;
				Width = textureFrame.Width;
				Height = textureFrame.Height;
			}
		}

		public Icon(Texture textureFrame)
		{
			TextureFrame = textureFrame;
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			if (Visible)
				textureFrame.Draw(Position.X, Position.Y);
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (textureFrame != null)
				textureFrame.Dispose();
		}
	}
}
