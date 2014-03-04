using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentPipelineExtensionLibrary;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Items
{
    public class MachineGun : Weapon
    {
        private static String WEAPON_DATA = "Item/MachineGun";

        public MachineGun(ContentManager content, Type owner)
            : base(content, WEAPON_DATA, owner)
        {
        }

        public override void Reload(int ammunitionCount)
        {
            throw new NotImplementedException();
        }

        public override void Upgrade(EntityId.WeaponUpgrade upgrade)
        {
            if (upgrade == EntityId.WeaponUpgrade.DoubleShot)
                SetProjectileType(EntityId.Projectile.MachineGunDouble);
            else if (upgrade == EntityId.WeaponUpgrade.TripleShot)
                SetProjectileType(EntityId.Projectile.MachineGunTriple);
        }

        //public override void Reload(int ammunitionCount)
        //{
        //    int ammunitionCountDifference = MAX_AMMUNITION - this.ammunitionCount;

        //    if(this.ammunitionCount < MAX_AMMUNITION)
        //        this.ammunitionCount += ammunitionCountDifference;
        //}
    }
}
