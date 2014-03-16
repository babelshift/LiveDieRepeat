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
	public class BulletMover : IBulletMLBulletInterface, ICollidable
	{
		public BulletMLBullet mlBullet;
		private bool used;
		public bool bulletRoot;
		private Vector pos;
		private Icon icon;
		private List<ICollidable> collidableComponents = new List<ICollidable>();

		public bool IsUsed
		{
			get { return used; }
			set { used = value; }
		}

		public Vector Position
		{
			get { return this.pos; }
			set 
			{ 
				this.pos = value;
				icon.Position = this.pos;
			}
		}

		public virtual IReadOnlyList<ICollidable> CollidableComponents { get { return collidableComponents; } }

		public Rectangle CollisionBox { get { return icon.Bounds; } }

		// 座標、向き、速度のプロパティを実装します。
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

		public void ResolveCollision(ICollidable collidableEntity)
		{
			IsUsed = false;
		}

		public void Init(Icon icon)
		{
			this.icon = icon;
			IsUsed = true;
			bulletRoot = false;
			mlBullet = new BulletMLBullet(this);
			//collidableComponents.Add(this);
		}

		public void Update(GameTime gameTime)
		{
			icon.Update(gameTime);

			//BulletMLで自分を動かす
			if (mlBullet.Run()) //自分が弾の発信源なら、処理終了後に自動的に消える
				if (bulletRoot)
					IsUsed = false;
		}

		public void Draw(GameTime gameTime, Renderer renderer)
		{
			icon.Draw(gameTime, renderer);
		}

		/// BulletMLの弾幕定義を自分にセット
		public void SetBullet(BulletMLTree tree)
		{
			mlBullet.InitTop(tree);
		}

		///以下、BulletMLLibに必要なインターフェイスを実装します

		/// <summary>
		/// 新しい弾(Mover)を作成するときライブラリから呼ばれる
		/// </summary>
		public BulletMLBullet GetNewBullet()
		{
			bulletRoot = true;
			BulletMover bulletMover = BulletMoverManager.CreateBulletMover();
			return bulletMover.mlBullet;
		}

		/// <summary>
		/// 弾が消えたときにライブラリから呼び出される
		/// </summary>
		public void Vanish()
		{
			IsUsed = false;
		}
	}
}