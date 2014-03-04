using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Entities
{
    public class ArrowRight : Entity
    {
        private static String ENTITY_DATA = "Entities/Objects/ArrowRight";

        protected override Vector2 Direction
        {
            get { return Vector2.Zero; }
        }

        public ArrowRight(ContentManager content)
        {
            base.Activate(content, ENTITY_DATA);
        }
    }
}
