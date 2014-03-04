using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Entities
{
    public class ShieldPickup : ItemEntity
    {
        public int ChargesRemaining { get; private set; }
        public int TotalCharges { get; private set; }

        private const int DEFAULT_CHARGES = 10;  // figure out how to move this to an .xml file
        private static String ENTITY_DATA = "Entities/ShieldPickup";

        public ShieldPickup(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            ChargesRemaining = DEFAULT_CHARGES;
            TotalCharges = DEFAULT_CHARGES;
        }

        public int RemoveCharge(int numberOfChargesToRemove)
        {
            ChargesRemaining -= numberOfChargesToRemove;
            return ChargesRemaining;
        }
    }
}
