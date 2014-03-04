using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Entities
{
    public class Fireball : ProjectileEntity
    {
        private static String ENTITY_DATA = "Entities/Fireball";

        public Fireball(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 playerPosition)
        {
            base.Update(gameTime, playerPosition);

            if (Vector2.Distance(position, playerPosition) > 150)
                Die();
        }
    }
}
