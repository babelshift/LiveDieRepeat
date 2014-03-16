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
	public class Player : Agent
	{
		private Icon icon;
		private double angle;

		public Guid ID { get; private set; }

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

		public Player(Icon icon)
		{
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
			//Vector origin = new Vector(icon.Width / 2, icon.Height / 2);
			icon.TextureFrame.Draw((int)icon.Position.X, (int)icon.Position.Y, angle, Vector.Zero);
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

		public Bullet FireWeapon(ContentManager contentManager)
		{
			Icon iconBullet = ControlFactory.CreateIcon(contentManager, "PlayerBullet");
			Bullet bullet = new Bullet(new Vector(700, 700), iconBullet);
			bullet.TeleportTo(new Vector(Position.X - bullet.Width / 2, Position.Y - bullet.Height / 2));
			bullet.RotateTo(angle + 45);
			return bullet;
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
