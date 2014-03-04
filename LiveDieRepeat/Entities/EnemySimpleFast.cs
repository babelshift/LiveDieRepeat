using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ContentPipelineExtensionLibrary;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public class EnemySimpleFast : EnemyEntity
    {
        private static String ENTITY_DATA = "Entities/Enemy2";

        private Weapon currentWeapon;
        public Weapon CurrentWeapon
        {
            set { currentWeapon = value; }
        }

        protected override Vector2 Direction
        {
            get
            {
                float xCoord = (float)Math.Cos((double)spriteActive.RadiansOfRotation);
                float yCoord = (float)Math.Sin((double)spriteActive.RadiansOfRotation);

                return new Vector2(xCoord, yCoord);
            }
        }

        public EnemySimpleFast(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            IsChasingPlayer = true;
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            Move(gameTime);

            base.Update(gameTime, playerPosition);
        }

        //public ProjectileEntity FireWeapon(TimeSpan timeOfShot)
        //{
        //    return currentWeapon.Fire(position, timeOfShot);
        //}

        public void Reload(int ammunitionCount)
        {
            currentWeapon.Reload(ammunitionCount);
        }

        private void Move(GameTime gameTime)
        {
            Vector2 previousPosition = position;
            Vector2 direction = Direction;
            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));

            SetMovingDirections(previousPosition);
        }
    }
}
