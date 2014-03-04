using LiveDieRepeat.Entities;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.UserInterface
{
    public class WeaponSlot : Slot
    {
        public Weapon WeaponAssociated { get; private set; }

        public WeaponSlot(Texture2D textureSlotImage, Texture2D textureSlotImageSelected, GraphicsDevice graphicsDevice)
            : base(textureSlotImage, textureSlotImageSelected, graphicsDevice)
        {
        }

        public void SetAssociatedWeapon(Weapon weapon, ItemEntity itemAssociated)
        {
            WeaponAssociated = weapon;
            this.itemAssociated = itemAssociated;
        }
    }
}
