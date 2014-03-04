using System;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Entities
{
    public class Spread : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/Spread";

        public Spread(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;
        }
    }
}
