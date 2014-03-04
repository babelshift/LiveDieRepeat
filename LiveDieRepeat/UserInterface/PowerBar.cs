using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.UserInterface
{
    public class PowerBar : Bar
    {
        private const String TEXTURE_NAME = "User Interface/Controls/BarExperience";

        private SpriteFont multiplierFont;
        private int multiplier;
        private Color multiplierFontColor = new Color(242, 242, 242);

        public PowerBar(ContentManager content, Vector2 position, SpriteFont multiplierFont)
            : base(content, TEXTURE_NAME, position)
        {
            this.multiplierFont = multiplierFont;
            this.multiplier = 0;
            PercentFilled = 0;
        }

        //public override int Add(int count)
        //{
        //    int remainingSegmentCount = base.Add(count);

        //    if (remainingSegmentCount == MAX_SEGMENTS)
        //        Rollover();

        //    return remainingSegmentCount;
        //}

        public override void Reset()
        {
            PercentFilled = 0;
        }

        private void Rollover()
        {
            multiplier++;
            PercentFilled = 0;
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    if (multiplier > 0)
        //        spriteBatch.DrawString(multiplierFont, "x" + multiplier.ToString(), new Vector2(position.X + MULTIPLIER_OFFSET_X, position.Y + MULTIPLIER_OFFSET_Y), multiplierFontColor);

        //    base.Draw(spriteBatch);
        //}

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
