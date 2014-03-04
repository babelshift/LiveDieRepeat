using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.Items
{
    public class SpreadGun : Weapon
    {
        private static String WEAPON_DATA = "Item/SpreadGun";

        public SpreadGun(ContentManager content, Type owner)
            : base(content, WEAPON_DATA, owner)
        {
        }

        public override void Reload(int ammunitionCount)
        {
        }

        public override void Upgrade(Entities.EntityId.WeaponUpgrade upgrade)
        {
        }

        public override List<ProjectileEntity> Fire(Vector2 position, float radians, TimeSpan timeOfShot)
        {
            List<ProjectileEntity> newProjectiles = new List<ProjectileEntity>();

            // this produces an explosive circle of bullets
            //int maxBullets = 10;
            //float interval = (float)(2 * Math.PI / maxBullets);
            //for (int i = 0; i < maxBullets; i++)
            //{
            //    float angle = i * interval;
            //    newProjectiles.AddRange(base.Fire(position, angle, timeOfShot));
            //}

            float angle = 0f;
            for (float angleOffset = 0; angleOffset < .6; angleOffset += .3f)
            {
                angle = radians + angleOffset;
                newProjectiles.AddRange(base.Fire(position, angle, timeOfShot));
                angle = radians - angleOffset;
                newProjectiles.AddRange(base.Fire(position, angle, timeOfShot));
            }

            return newProjectiles;
        }
    }
}
