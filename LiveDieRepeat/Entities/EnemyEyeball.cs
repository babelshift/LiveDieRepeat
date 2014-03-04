using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public class EnemyEyeball : EnemyEntity
    {
        private static String ENTITY_DATA = "Entities/EnemyEyeball";

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

        public EnemyEyeball(ContentManager content)
            : base(content, ENTITY_DATA)
        {
        }

        public override void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is ProjectileEntity)
                {
                    ProjectileEntity projectile = collidableEntity as ProjectileEntity;

                    if (projectile.HasOwner())
                    {
                        if (projectile.Owner.Equals(typeof(PlayerEntity)))
                        {
                            // only count as a hit if the projectile is alive and I'm alive
                            if (!projectile.IsDead)
                            {
                                if (TakeDamage(1))
                                    Die();
                            }
                        }
                    }
                }
            }
        }
    }
}
