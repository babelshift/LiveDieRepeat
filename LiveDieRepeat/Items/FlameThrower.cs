using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.Items
{
    public class FlameThrower : Weapon
    {
        private static String WEAPON_DATA = "Item/FlameThrower";

        public FlameThrower(ContentManager content, Type owner)
            : base(content, WEAPON_DATA, owner)
        {
        }

        public override void Reload(int ammunitionCount)
        {
        }

        public override void Upgrade(EntityId.WeaponUpgrade upgrade)
        {
        }
    }
}
