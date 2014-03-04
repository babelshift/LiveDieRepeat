using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Entities
{
    public class EnemyBrain : EnemyEntity
    {
        private static String ENTITY_DATA = "Entities/EnemyBrain";

        private Weapon currentWeapon;
        public Weapon CurrentWeapon
        {
            set { currentWeapon = value; }
        }

        protected override Vector2 Direction
        {
            get
            {
                // chase player
                float xCoord = (float)Math.Cos((double)spriteActive.RadiansOfRotation);
                float yCoord = (float)Math.Sin((double)spriteActive.RadiansOfRotation);

                return new Vector2(xCoord, yCoord);
            }
        }

        public EnemyBrain(ContentManager content)
            : base(content, ENTITY_DATA)
        {
        }
    }
}
