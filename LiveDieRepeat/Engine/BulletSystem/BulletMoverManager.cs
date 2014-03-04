using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ContentPipelineExtensionLibrary;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Engine.BulletSystem
{
    /// <summary>
    /// オブジェクトを一括管理する
    /// </summary>
    class BulletMoverManager
    {
        //todo: should this really be static?
        public static List<BulletMover> BulletMovers = new List<BulletMover>(); //Emitterのリスト

        /// <summary>
        /// 新しいEmitterを作成
        /// </summary>
        static public BulletMover CreateBulletMover(SpriteSheet spriteSheet, Sprite sprite, Vector2 position, BulletMLLib.BulletMLTree tree)
        {
            BulletMover BulletMover = CreateBulletMover(spriteSheet, sprite);
            BulletMover.Position = position;
            BulletMover.SetBullet(tree);
            return BulletMover;
        }

        static public BulletMover CreateBulletMover(SpriteSheet spriteSheet, Sprite sprite)
        {
            BulletMover BulletMover = new BulletMover();
            BulletMovers.Add(BulletMover); //Emitterを登録
            BulletMover.Init(spriteSheet, sprite); //初期化
            return BulletMover;
        }

        /// <summary>
        /// すべてのEmitterの行動を実行する
        /// </summary>
        static public void Update(GameTime gameTime, Camera camera)
        {
            foreach (var bulletMover in BulletMovers)
                bulletMover.Update(gameTime, camera);
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
            BulletMovers.RemoveAll(bm => !bm.used);
        }
    }
}
