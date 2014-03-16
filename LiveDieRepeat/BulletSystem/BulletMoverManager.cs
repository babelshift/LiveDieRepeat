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
		private static ContentManager contentManager;

		//todo: should this really be static?
		public static List<BulletMover> BulletMovers = new List<BulletMover>(); //Emitterのリスト

		public static void Init(ContentManager contentManagerRef)
		{
			contentManager = contentManagerRef;
		}

		/// <summary>
		/// 新しいEmitterを作成
		/// </summary>
		static public BulletMover CreateBulletMover(Vector position, BulletMLLib.BulletMLTree tree)
		{
			BulletMover BulletMover = CreateBulletMover();
			BulletMover.Position = position;
			BulletMover.SetBullet(tree);
			return BulletMover;
		}

		static public BulletMover CreateBulletMover()
		{
			Icon iconBullet = ControlFactory.CreateIcon(contentManager, "PlayerBullet");
			BulletMover BulletMover = new BulletMover();
			BulletMovers.Add(BulletMover); //Emitterを登録
			BulletMover.Init(iconBullet); //初期化
			return BulletMover;
		}

		/// <summary>
		/// すべてのEmitterの行動を実行する
		/// </summary>
		static public void Update(GameTime gameTime)
		{
			for(int i = 0; i < BulletMovers.Count; i++)
				BulletMovers[i].Update(gameTime);
		}

		/// <summary>
		/// Used when we need to quickly delete all bullets (usually when the screen is exiting or a level reset occurs)
		/// </summary>
		static public void ClearBulletMovers()
		{
			BulletMovers.Clear();
		}

		/// <summary>
		/// Get rid of all bullet movers that are no longer used
		/// </summary>
		static public void FreeBulletMovers()
		{
			BulletMovers.RemoveAll(bm => !bm.IsUsed);
		}
	}
}