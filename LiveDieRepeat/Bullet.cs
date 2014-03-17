using LiveDieRepeat.BulletSystem;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;

namespace LiveDieRepeat
{
	public class Bullet : Agent, ICollidable
	{
		private Icon icon;
		private double angle;

		public Guid ID { get; private set; }

		public Vector Speed { get; private set; }

		public int Width { get { return icon.Width; } }

		public int Height { get { return icon.Height; } }

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

		private Vector Direction
		{
			get
			{
				float radians = MathExtensions.ToRadians(angle);
				float xCoord = (float)Math.Cos(radians);
				float yCoord = (float)Math.Sin(radians);

				return new Vector(xCoord, yCoord);
			}
		}

		public Bullet(Vector speed, Icon icon)
		{
			Speed = speed;
			this.icon = icon;
			ID = Guid.NewGuid();
		}

		public override void Update(GameTime gameTime)
		{
			icon.Update(gameTime);

			Move(gameTime);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			icon.Draw(gameTime, renderer);
		}

		public void RotateTo(double angle)
		{
			this.angle = angle;
		}

		private void Move(GameTime gameTime)
		{
			Vector direction = Direction;
			double dt = gameTime.ElapsedGameTime.TotalSeconds;
			Position += new Vector((float)(direction.X * Speed.X * dt), (float)(direction.Y * Speed.Y * dt));
		}

		public void ResolveCollision(ICollidable collidable)
		{
			if (!IsDead)
				if (collidable is BulletMover)
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