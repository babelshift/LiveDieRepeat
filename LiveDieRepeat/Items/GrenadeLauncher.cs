using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Items
{
    public class GrenadeLauncher : Weapon
    {
        private static String WEAPON_DATA = "Item/GrenadeLauncher";

        public GrenadeLauncher(ContentManager content, Type owner)
            : base(content, WEAPON_DATA, owner)
        {
        }

        public override void Reload(int ammunitionCount)
        {
        }

        public override void Upgrade(Entities.EntityId.WeaponUpgrade upgrade)
        {
        }
    }
}
