using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace LiveDieRepeat.Engine
{
    public class SpriteSheet
    {
        public Texture2D Texture;
        public int FrameWidth;
        public int FrameHeight;
        public List<Sprite> Sprites = new List<Sprite>();

        private Dictionary<int, Sprite> spriteDictionary = new Dictionary<int, Sprite>();

        /// <summary>
        /// Activates all sprites in this SpriteSheet. When sprites are found for the passed ID values, they are added to the 
        /// local dictionary for later retrieval.
        /// </summary>
        /// <param name="spriteIds"></param>
        public void Activate(List<int> spriteIds)
        {
            foreach (int spriteId in spriteIds)
            {
                Sprite sprite = null;
                sprite = Sprites.Find(s => s.Id == spriteId);
                if (sprite != null)
                {
                    sprite.Activate();

                    if (!spriteDictionary.ContainsKey(spriteId))
                        spriteDictionary.Add(sprite.Id, sprite);
                }
            }
        }

        /// <summary>
        /// Will return a shallow copy of a Sprite object that is identified by the passed spriteId.
        /// This is used when an entity needs to get a sprite from the sheet to use for display.
        /// We must return a copy because many entities will use the same sprite but manipulate the sprite independently.
        /// Returning the same reference would cause all entities to share the same instance and step on each others' toes.
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public Sprite GetSprite(int spriteId)
        {
            Sprite sprite = null;
            bool exists = spriteDictionary.TryGetValue(spriteId, out sprite);
            if (sprite != null)
            {
                if (!sprite.IsActivated)
                    throw new Exception("Sprite has not been activated. Call 'Activate' on this SpriteSheet prior to retrieving this Sprite.");

                return sprite.Copy();
            }
            else
                throw new ArgumentException("Sprite does not exist with that ID.");
        }
    }
}
