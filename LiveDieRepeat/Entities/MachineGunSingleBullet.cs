using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentPipelineExtensionLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public class MachineGunSingleBullet : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/MachineGunSingleBullet";

        public MachineGunSingleBullet(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;
        }
    }
}
