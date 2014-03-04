using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Entities
{
    public class ArrowDown : Entity
    {
        private static String ENTITY_DATA = "Entities/Objects/ArrowDown";

        protected override Vector2 Direction
        {
            get { return Vector2.Zero; }
        }

        public ArrowDown(ContentManager content)
        {
            base.Activate(content, ENTITY_DATA);
        }
    }
}
