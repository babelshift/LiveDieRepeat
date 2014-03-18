using LiveDieRepeat.BulletSystem;
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
	public class Opponent : Agent
	{
		private Icon icon;

		public Guid ID { get; private set; }

		public Vector Speed { get; private set; }

		public BulletMover BulletMover { get; set; }

		public override Vector Position
		{
			get { return base.Position; }
			protected set
			{
				base.Position = value;
				icon.Position = base.Position;
			}
		}

		public Opponent(Vector speed, Icon icon)
		{
			Speed = speed;
			this.icon = icon;
			ID = Guid.NewGuid();
		}

		int moveX = 3;

		public override void Update(GameTime gameTime)
		{
			icon.Update(gameTime);

			if (icon.Bounds.Right >= MainGame.SCREEN_WIDTH_LOGICAL)
			{
				moveX *= -1;
				Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL - icon.Width - 1, Position.Y);
			}
			else if (Position.X <= 0)
				moveX = 3;

			Position += new Vector(moveX, 0);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			icon.Draw(gameTime, renderer);
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			icon.Dispose();
		}
	}
}