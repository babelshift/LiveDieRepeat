using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ContentPipelineExtensionLibrary
{
    public class WeaponData
    {
        [ContentSerializer]
        public int ProjectileType;

        [ContentSerializer]
        public int MaxAmmo;

        [ContentSerializer]
        public float SecondsBetweenShots;
    }
}
