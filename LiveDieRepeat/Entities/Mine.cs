using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public class Mine : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/Mine";

        public Mine(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;
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
            }
        }
    }
}
