using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Entities
{
    public class Grenade : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/Grenade";

        private const int secondsToLive = 2;
        private const float maxDistanceFromPlayer = 2000;
        private double secondsAlive = 0;
        private bool isReflecting = false;

        public Grenade(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;
        }

        //todo: don't hardcode all this logic
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 playerPosition)
        {
            secondsAlive += gameTime.ElapsedGameTime.TotalSeconds;

            float currentDistanceFromPlayer = Vector2.Distance(position, playerPosition);

            if ((int)speed.X > 0 && (int)speed.Y > 0)
                speed = Vector2.Lerp(speed, Vector2.Zero, currentDistanceFromPlayer / maxDistanceFromPlayer);
            else if ((int)speed.X == 0 && (int)speed.Y == 0)
                speed = Vector2.Zero;

            if (secondsAlive > secondsToLive)
                Die();

            if (isReflecting)
            {
                spriteActive.RadiansOfRotation += (float)Math.PI;
                isReflecting = false;
            }

            base.Update(gameTime, playerPosition);
        }

        public override void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is PlayerEntity)
                {
                    if (HasOwner())
                    {
                        if (Owner.Equals(typeof(EnemyEntity)))
                        {
                            Die();
                        }
                    }
                }
                else if (collidableEntity is IEnemy)
                {
                    if (HasOwner())
                    {
                        if (Owner.Equals(typeof(PlayerEntity)))
                        {
                            Die();
                        }
                    }
                }
                else if (collidableEntity is MapObject)
                {
                    MapObject collidableMapObject = collidableEntity as MapObject;

                    if (collidableMapObject.IsCollidable)
                    {
                        isReflecting = true;
                    }
                }
            }
        }
    }
}
