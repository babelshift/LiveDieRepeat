using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletMLLib;
using Microsoft.Xna.Framework.Graphics;
using ContentPipelineExtensionLibrary;

namespace LiveDieRepeat.Engine.BulletSystem
{
    /// <summary>
    /// 弾や敵オブジェクト（自身が弾源になる場合も、弾源から呼び出される場合もあります）
    /// </summary>
    class BulletMover : IBulletMLBulletInterface, ICollidable
    {
        public BulletMLBullet mlBullet;
        public bool used;
        public bool bulletRoot;
        private Vector2 pos;
        private SpriteSheet spriteSheet;
        private Sprite sprite;
        private List<ICollidable> collidableComponents = new List<ICollidable>();

        public Vector2 Position
        {
            get { return this.pos; }
            set { this.pos = value; }
        }

        public virtual IList<ICollidable> CollidableComponents { get { return collidableComponents; } }

        public Rectangle CollisionBox
        {
            get
            {
                int rectX = (int)pos.X - (int)((sprite.FrameWidth * sprite.ScaleFactor) / 2);
                int rectY = (int)pos.Y - (int)((sprite.FrameHeight * sprite.ScaleFactor) / 2);
                int rectWidth = (int)(sprite.FrameWidth * sprite.ScaleFactor);
                int rectHeight = (int)(sprite.FrameHeight * sprite.ScaleFactor);

                return new Rectangle(rectX, rectY, rectWidth, rectHeight);
            }
        }

        // 座標、向き、速度のプロパティを実装します。
        public float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }

        public float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }

        public float Dir { get; set; }

        public float Speed { get; set; }

        public void ResolveCollision(ICollidable collidableEntity)
        {
            used = false;
        }

        public void Init(SpriteSheet spriteSheet, Sprite sprite)
        {
            this.spriteSheet = spriteSheet;
            this.sprite = sprite;
            used = true;
            bulletRoot = false;
            mlBullet = new BulletMLBullet(this);
            //collidableComponents.Add(this);
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            if (camera.IsInView(pos, sprite.FrameWidth, sprite.FrameHeight))
            {
                sprite.Update(gameTime);

                //BulletMLで自分を動かす
                if (mlBullet.Run()) //自分が弾の発信源なら、処理終了後に自動的に消える
                    if (bulletRoot)
                        used = false;
            }
            else used = false;

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectTexture)
        {
            sprite.Draw(spriteBatch, spriteSheet.Texture, pos, true);
            RectangleExtensions.DrawRectangle(CollisionBox, rectTexture, Color.Red, spriteBatch, false, 1);
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
            BulletMover bulletMover = BulletMoverManager.CreateBulletMover(spriteSheet, sprite);
            return bulletMover.mlBullet;
        }

        /// <summary>
        /// 弾が消えたときにライブラリから呼び出される
        /// </summary>
        public void Vanish()
        {
            used = false;
        }
    }
}
