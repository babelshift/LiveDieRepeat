using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDieRepeat.Entities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Items
{
    public class MachineGunPickup : ItemEntity
    {
        private static String ENTITY_DATA = "Item/MachineGunPickup";

        public MachineGunPickup(ContentManager content)
            : base(content, ENTITY_DATA)
        {
            WeaponAssociated = Factory<Weapon>.Create((int)EntityId.Weapon.MachineGun);
        }

        public override void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is PlayerEntity)
                {
                    PlayerEntity player = collidableEntity as PlayerEntity;

                    // create the weapon associated with this item pickup and give it to the player
                    player.GiveItem(this);

                    Die();
                }
            }
        }
    }
}
