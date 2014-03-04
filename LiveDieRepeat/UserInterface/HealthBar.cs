using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.UserInterface
{
    public class HealthBar : Bar
    {
        private const String TEXTURE_NAME = "User Interface/Controls/BarHealth";

        public HealthBar(ContentManager content, Vector2 position)
            : base(content, TEXTURE_NAME, position)
        {
            PercentFilled = 100;
        }

        public override void Reset()
        {
            PercentFilled = 100;
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
