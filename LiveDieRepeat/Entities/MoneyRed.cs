using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using ContentPipelineExtensionLibrary;

namespace LiveDieRepeat.Entities
{
    public class MoneyRed : ItemEntity
    {
        private const int worth = 10;

        private static String ENTITY_DATA = "Entities/MoneyRed";

        public MoneyRed(ContentManager content)
            : base(content, ENTITY_DATA)
        {
        }
    }
}
