using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ContentPipelineExtensionLibrary;
using Microsoft.Xna.Framework.Graphics;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public class Rocket : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/Rocket";

        private int numEnemiesHit = 0;

        public Rocket(ContentManager content)
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
                            numEnemiesHit++;

                            //todo: don't hardcode this
                            if (numEnemiesHit >= 3)
                                Die();
                        }
                    }
                }
                else if (collidableEntity is MapObject)
                {
                    MapObject collidableMapObject = collidableEntity as MapObject;

                    if (collidableMapObject.IsCollidable)
                    {
                        Die();
                    }
                }
            }
        }
    }
}
