using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.UserInterface
{
    public class WeaponSlotBar
    {
        private GraphicsDevice graphicsDevice;

        private Texture2D textureSlot;
        private Texture2D textureSlotSelected;
        private List<WeaponSlot> slots;
        private WeaponSlot CurrentSlot { get { return slots[SelectedSlotIndex]; } }

        private int previousScrollWheelValue;
        private KeyboardState previousKeyboardState;

        private int slotWidth = 0;
        private Vector2 position;
        private const int spaceBetweenSlots = 2;

        public int SelectedSlotIndex { get; private set; }
        public Weapon SelectedWeapon { get { return CurrentSlot.WeaponAssociated; } }

        public event EventHandler<EventArgs> WeaponChangedEvent;

        #region Game Loop

        public WeaponSlotBar(ContentManager content, Vector2 position, GraphicsDevice graphicsDevice)
        {
            this.slots = new List<WeaponSlot>();
            this.textureSlot = content.Load<Texture2D>("User Interface/Controls/SlotSmall");
            this.textureSlotSelected = content.Load<Texture2D>("User Interface/Controls/SlotSmallSelected");
            this.graphicsDevice = graphicsDevice;
            this.position = position;
        }

        /// <summary>When updating the weapon slot bar, update every slot that is contained and handle any input from the user.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach (WeaponSlot slot in slots)
                slot.Update(gameTime);

            HandleInput();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            foreach (WeaponSlot slot in slots)
                slot.Draw(spriteBatch, gameTime, transitionColor, transitionAlpha);
        }

        /// <summary>When the user scrolls down, move right in the list. When the user scrolls up, move left in the list. When the user picks a specific number, jump to that index.
        /// </summary>
        private void HandleInput()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            int scrollWheelValue = mouseState.ScrollWheelValue;
            if (scrollWheelValue < previousScrollWheelValue)
                GotoNextSlot();
            else if (scrollWheelValue > previousScrollWheelValue)
                GotoPreviousSlot();

            previousScrollWheelValue = scrollWheelValue;

            if (previousKeyboardState == null)
                previousKeyboardState = keyboardState;

            if (previousKeyboardState != keyboardState)
            {
                List<Keys> pressedKeys = new List<Keys>(keyboardState.GetPressedKeys());

                if (pressedKeys.Contains(Keys.D1))
                    GotoSlot(0);

                if (pressedKeys.Contains(Keys.D2))
                    GotoSlot(1);

                if (pressedKeys.Contains(Keys.D3))
                    GotoSlot(2);

                if (pressedKeys.Contains(Keys.D4))
                    GotoSlot(3);

                if (pressedKeys.Contains(Keys.D5))
                    GotoSlot(4);
            }

            previousKeyboardState = keyboardState;
        }

        #endregion

        #region Helper Methods

        /// <summary>Jump to the index in the weapon slot bar indicated by the passed value, set it to selected, and inform subscribers (the player) that the player has changed weapons to another slot.
        /// </summary>
        /// <param name="slotIndex"></param>
        private void GotoSlot(int slotIndex)
        {
            UnselectAllSlots();

            SelectedSlotIndex = slotIndex;

            CurrentSlot.IsSelected = true;

            OnWeaponChangedEvent(this, EventArgs.Empty);
        }

        /// <summary>Move to the next index in the weapon slot bar, set it to selected, and inform subscribers (the player) that the player has changed weapons to another slot. If the next slot is outside
        /// the bounds of the weapon slot bar, it will roll over to the other end of the array.
        /// </summary>
        private void GotoNextSlot()
        {
            UnselectAllSlots();

            if (SelectedSlotIndex >= slots.Count - 1)
                SelectedSlotIndex = 0;
            else
                SelectedSlotIndex++;

            CurrentSlot.IsSelected = true;

            OnWeaponChangedEvent(this, EventArgs.Empty);
        }

        /// <summary>Move to the previous index in the weapon slot bar, set it to selected, and inform subscribers (the player) that the player has changed weapons to another slot. If the next slot is outside
        /// the bounds of the weapon slot bar, it will roll over to the other end of the array.
        /// </summary>
        private void GotoPreviousSlot()
        {
            UnselectAllSlots();

            if (SelectedSlotIndex <= 0)
                SelectedSlotIndex = slots.Count - 1;
            else
                SelectedSlotIndex--;

            CurrentSlot.IsSelected = true;

            OnWeaponChangedEvent(this, EventArgs.Empty);
        }

        /// <summary>Set all slots to de-selected. This is useful when you want to set a new selected slot.
        /// </summary>
        private void UnselectAllSlots()
        {
            foreach (WeaponSlot slot in slots)
                slot.IsSelected = false;
        }

        /// <summary>Add a new slot to the slot bar so that we can add a weapon to it.
        /// </summary>
        private void AddNewSlot()
        {
            int numSlots = slots.Count;
            WeaponSlot weaponSlot = new WeaponSlot(textureSlot, textureSlotSelected, graphicsDevice);

            if (slots.Count == 0)
            {
                weaponSlot.Position = position;
                slotWidth = weaponSlot.Width;
            }
            else
                weaponSlot.Position = new Vector2(slots[numSlots - 1].Position.X + slotWidth + spaceBetweenSlots, slots[numSlots - 1].Position.Y);

            slots.Add(weaponSlot);

            CurrentSlot.IsSelected = true;
        }

        /// <summary>Adds a weapon to the slot bar by creating a new slot for it and setting the new slots associated weapon and item to the passed values
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="item"></param>
        public void AddWeapon(Weapon weapon, ItemEntity item)
        {
            if (!HasWeapon(weapon))
            {
                AddNewSlot();
                slots[slots.Count - 1].SetAssociatedWeapon(weapon, item);
            }
        }

        /// <summary>Checks if the user already has this weapon registered with the weapon slot bar. This is to prevent double adding already obtained weapons.
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        private bool HasWeapon(Weapon weapon)
        {
            bool hasWeapon = false;
            foreach (WeaponSlot slot in slots)
            {
                if (slot.WeaponAssociated != null)
                    if (slot.WeaponAssociated.GetType().Equals(weapon.GetType()))
                        hasWeapon = true;
            }
            return hasWeapon;
        }

        #endregion

        #region Events

        /// <summary>When the user changes weapons, notify subscribers (the player) to switch the active weapon to the new selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWeaponChangedEvent(object sender, EventArgs e)
        {
            if (WeaponChangedEvent != null)
                WeaponChangedEvent(sender, e);
        }

        #endregion
    }
}
