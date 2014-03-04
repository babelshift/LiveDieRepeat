using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Entities
{
    //todo: add an expirable entity for death animations
    public class ExplosionMedium : Entity
    {
        private static String ENTITY_DATA = "Entities/ExplosionMedium";

        protected override Vector2 Direction
        {
            get { return Vector2.Zero; }
        }

        public ExplosionMedium(ContentManager content)
        {
            base.Activate(content, ENTITY_DATA);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // kill the entity after the animation is done
            if (!spriteActive.IsAnimating)
                Die();
        }
    }
}
