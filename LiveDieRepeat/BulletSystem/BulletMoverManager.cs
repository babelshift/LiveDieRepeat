using LiveDieRepeat.Content;
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
	public class BulletMoverManager
	{
		private ContentManager contentManager;

		private List<BulletMover> emitters = new List<BulletMover>();

		public IReadOnlyList<BulletMover> Emitters { get { return emitters; } }

		public BulletMoverManager(ContentManager contentManager)
		{
			this.contentManager = contentManager;
		}

		/// <summary>
		/// Creating a new Emitter
		/// </summary>
		public BulletMover CreateBulletMover(Vector position, BulletMLLib.BulletMLTree tree)
		{
			BulletMover newBulletMover = CreateBulletMover();
			newBulletMover.Position = position;
			newBulletMover.SetBullet(tree);
			return newBulletMover;
		}

		public BulletMover CreateBulletMover()
		{
			Icon iconBullet = ControlFactory.CreateIcon(contentManager, "Square");
			BulletMover newBulletMover = new BulletMover();
			emitters.Add(newBulletMover);
			newBulletMover.Init(this, iconBullet);
			return newBulletMover;
		}

		/// <summary>
		/// Update all Emitters
		/// </summary>
		public void Update(GameTime gameTime)
		{
			for (int i = 0; i < emitters.Count; i++)
				emitters[i].Update(gameTime);

			FreeBulletMovers();
		}

		public void Draw(GameTime gameTime, Renderer renderer)
		{
			foreach (var emitter in emitters)
				emitter.Draw(gameTime, renderer);
		}

		/// <summary>
		/// Used when we need to quickly delete all bullets (usually when the screen is exiting or a level reset occurs)
		/// </summary>
		public void ClearBulletMovers()
		{
			emitters.Clear();
		}

		/// <summary>
		/// Get rid of all bullet movers that are no longer used
		/// </summary>
		private void FreeBulletMovers()
		{
			foreach (var emitter in emitters)
				if (!emitter.IsUsed)
					emitter.Dispose();
			emitters.RemoveAll(bm => !bm.IsUsed);
		}
	}
}