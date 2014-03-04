using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Entities
{
    public class WallHugger : EnemyEntity
    {
        private static String ENTITY_DATA = "Entities/WallHugger1";

        private Vector2 currentDestination;

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

        public WallHugger(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            IsChasingPlayer = true;
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            Move(gameTime, playerPosition);

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

        private void Move(GameTime gameTime, Vector2 playerPosition)
        {
            // todo: any way to move this to enemy entity? it seems generic enough
            if (currentDestination == Vector2.Zero || CollisionBox.Contains((int)currentDestination.X, (int)currentDestination.Y))
            {
                if (path.Count > 0)
                    currentDestination = path.Dequeue();
                else
                    currentDestination = playerPosition;
            }

            spriteActive.RadiansOfRotation = (float)Math.Atan2((double)(currentDestination.Y - position.Y), (double)(currentDestination.X - position.X));

            Vector2 previousPosition = position;
            Vector2 direction = Direction;
            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));

            SetMovingDirections(previousPosition);
        }
    }
}
