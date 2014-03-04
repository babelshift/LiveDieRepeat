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
    public class EnemyProjectileSimple : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/MachineGunEnemyBullet";

        public EnemyProjectileSimple(ContentManager content)
            : base(content, ENTITY_DATA)
        {
        }
    }
}
