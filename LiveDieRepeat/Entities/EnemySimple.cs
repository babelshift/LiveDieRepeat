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
    public class EnemySimple : EnemyEntity
    {
        private static String ENTITY_DATA = "Entities/Enemy1";

        private Weapon currentWeapon;
        public Weapon CurrentWeapon
        {
            set { currentWeapon = value; }
        }

        protected override Vector2 Direction
        {
            get
            {
                // chase player
                float xCoord = (float)Math.Cos((double)spriteActive.RadiansOfRotation);
                float yCoord = (float)Math.Sin((double)spriteActive.RadiansOfRotation);

                return new Vector2(xCoord, yCoord);
            }
        }

        public EnemySimple(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            IsChasingPlayer = true;
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {          
            Move(gameTime);

            base.Update(gameTime, playerPosition);
        }

        public List<ProjectileEntity> FireWeapon(TimeSpan timeOfShot)
        {
            List<ProjectileEntity> shots = new List<ProjectileEntity>();

            if (currentWeapon != null)
            {
                int maxBullets = 25;
                float interval = (float)(2 * Math.PI / maxBullets);
                for (int i = 0; i < maxBullets; i++)
                {
                    float angle = i * interval;
                    shots.AddRange(currentWeapon.Fire(position, angle, timeOfShot));
                }
            }

            return shots;
        }

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

        /// <summary>todo: move this somewhere else
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="centerOfCircle"></param>
        /// <returns></returns>
        private Vector2 GetNextCircularPosition(GameTime gameTime, Vector2 centerOfCircle)
        {
            float centerX = centerOfCircle.X;
            float centerY = centerOfCircle.Y;
            int radius = 200; // todo: don't hardcode
            int speed = 2; // todo: don't hardcode
            double speedScale = (0.001 * 2 * Math.PI) / speed;
            double angle = gameTime.TotalGameTime.TotalMilliseconds * speedScale;

            float xCoord = centerX + (float)Math.Sin(angle) * radius;
            float yCoord = centerY + (float)Math.Cos(angle) * radius;

            return new Vector2(xCoord, yCoord);
        }
    }
}
