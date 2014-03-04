using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Entities
{
    public class WeaponUpgradePickup : ItemEntity
    {
        //private int secondsLeft;
        //private double timeSinceLastSecond;
        //private const int DEFAULT_TIME = 5;  // figure out how to move this to an .xml file
        private static String ENTITY_DATA = "Entities/WeaponUpgradePickup";
        private bool isPickedUp;

        //public event EventHandler EndPowerEvent;

        public bool IsPickedUp
        {
            set { isPickedUp = value; }
        }

        public WeaponUpgradePickup(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            //this.secondsLeft = DEFAULT_TIME;
            //this.timeSinceLastSecond = 0;
        }

        //private void CalculateEndEvent(GameTime gameTime)
        //{
        //    if (isPickedUp)
        //    {
        //        if (gameTime.TotalGameTime.TotalSeconds - timeSinceLastSecond >= 1)
        //        {
        //            timeSinceLastSecond = gameTime.TotalGameTime.TotalSeconds;
        //            secondsLeft -= 1;
        //        }

        //        if (secondsLeft <= 0)
        //            OnEndPowerEvent(EventArgs.Empty);
        //    }
        //}

        //private void OnEndPowerEvent(EventArgs e)
        //{
        //    if (EndPowerEvent != null)
        //        EndPowerEvent(this, e);
        //}
    }
}
