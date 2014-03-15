using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;

namespace LiveDieRepeat
{
	public class Enemy : Agent, ICollidable
	{
		private Icon icon;
		private double angle;

		public Guid ID { get; private set; }

		public Vector Speed { get; private set; }

		public IReadOnlyList<ICollidable> CollidableComponents
		{
			get { return new List<ICollidable>(); }
		}

		public Rectangle CollisionBox
		{
			get { return icon.Bounds; }
		}

		public override Vector Position
		{
			get { return base.Position; }
			protected set
			{
				base.Position = value;
				icon.Position = base.Position;
			}
		}

		public Enemy(Vector speed, Icon icon)
		{
			Speed = speed;
			this.icon = icon;
			ID = Guid.NewGuid();
		}

		public override void Update(GameTime gameTime)
		{
			icon.Update(gameTime);

			RotateTo(angle + 3);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			Vector origin = new Vector(icon.Width / 2, icon.Height / 2);
			icon.TextureFrame.Draw((int)icon.Position.X, (int)icon.Position.Y, angle, origin);
			//icon.Draw(gameTime, renderer);
		}

		public void RotateTo(double angle)
		{
			this.angle = angle;
		}

		public void ResolveCollision(ICollidable collidable)
		{
			if (!IsDead)
				if (collidable is Bullet)
					if (CollisionBox.Intersects(collidable.CollisionBox))
						Die();
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