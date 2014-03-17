using BulletMLLib;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.BulletSystem
{
	public class BulletMover : IBulletMLBulletInterface, ICollidable, IDisposable
	{
		private BulletMoverManager myManager;
		private Vector pos;
		private Icon icon;
		private List<ICollidable> collidableComponents = new List<ICollidable>();

		public virtual IReadOnlyList<ICollidable> CollidableComponents { get { return collidableComponents; } }

		public Rectangle CollisionBox { get { return icon.Bounds; } }

		public bool IsBulletRoot { get; set; }

		public BulletMLBullet MLBullet { get; set; }

		public bool IsUsed { get; set; }

		public Vector Position
		{
			get { return this.pos; }
			set
			{
				this.pos = value;
				icon.Position = this.pos;
			}
		}

		public float X
		{
			get { return pos.X; }
			set 
			{
				pos = new Vector(value, pos.Y);
				icon.Position = pos;
			}
		}

		public float Y
		{
			get { return pos.Y; }
			set 
			{
				pos = new Vector(pos.X, value);
				icon.Position = pos;
			}
		}

		public float Dir { get; set; }

		public float Speed { get; set; }

		public void Init(BulletMoverManager myManager, Icon icon)
		{
			this.myManager = myManager;
			this.icon = icon;
			IsUsed = true;
			IsBulletRoot = false;
			MLBullet = new BulletMLBullet(this);
			//collidableComponents.Add(this);
		}

		public void Update(GameTime gameTime)
		{
			angle += 5;
			icon.Update(gameTime);

			//BulletMLで自分を動かす
			if (MLBullet.Run()) //自分が弾の発信源なら、処理終了後に自動的に消える
				if (IsBulletRoot)
					IsUsed = false;

			if (!MainGame.Viewport.Contains(Position))
				IsUsed = false;
		}

		public void Draw(GameTime gameTime, Renderer renderer)
		{
			icon.TextureFrame.Draw((int)icon.Position.X, (int)icon.Position.Y, angle, new Vector(icon.Width / 2, icon.Height / 2));
		}

		private double angle = 0;

		public void SetBullet(BulletMLTree tree)
		{
			MLBullet.InitTop(tree);
		}

		/// <summary>
		/// Called from BulletMLLib when a new bullet needs to be created in the tree
		/// </summary>
		public BulletMLBullet GetNewBullet()
		{
			IsBulletRoot = true;
			BulletMover bulletMover = myManager.CreateBulletMover();
			return bulletMover.MLBullet;
		}

		/// <summary>
		/// Called from BulletMLLib when a bullet is gone and unused
		/// </summary>
		public void Vanish()
		{
			IsUsed = false;
		}

		public void ResolveCollision(ICollidable collidableEntity)
		{
			if(IsUsed)
				if(collidableEntity is Bullet)
					IsUsed = false;
		}

		public void Dispose()
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