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
		private Queue<int> path = new Queue<int>();
		private List<Queue<int>> paths = new List<Queue<int>>();
		private int moveDistanceX = 3;
		private bool isMovingToNode = false;

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

			AddNodeToPathQueue(10);
			AddNodeToPathQueue(100);
		}

		public override void Update(GameTime gameTime)
		{
			icon.Update(gameTime);

			//if (icon.Bounds.Right >= MainGame.SCREEN_WIDTH_LOGICAL)
			//{
			//	moveDistanceX *= -1;
			//	Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL - icon.Width - 1, Position.Y);
			//}
			//else if (Position.X <= 0)
			//	moveDistanceX = 3;

			Position += new Vector(moveDistanceX, 0);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			icon.Draw(gameTime, renderer);
		}

		private void AddNodeToPathQueue(int coordinateX)
		{
			path.Enqueue(coordinateX);
		}

		private int GetNextPathNode()
		{
			return path.Dequeue();
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