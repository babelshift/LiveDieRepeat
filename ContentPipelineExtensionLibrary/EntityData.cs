using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ContentPipelineExtensionLibrary
{
    public class EntityData
    {
        [ContentSerializer]
        public int CollisionOffset;

        [ContentSerializer]
        public float HorizontalSpeed;

        [ContentSerializer]
        public float VerticalSpeed;

        [ContentSerializer(Optional = true)]
        public int Health;

        [ContentSerializer(Optional=true)]
        public String SoundActivated;

        [ContentSerializer(Optional = true)]
        public String SoundHit;

        [ContentSerializer(Optional = true)]
        public String SoundDeath;

        public String SpriteSheet;

        public int SpriteIdFacingDown;

        [ContentSerializer(Optional = true)]
        public int SpriteIdFacingUp;

        [ContentSerializer(Optional = true)]
        public int SpriteIdFacingLeft;

        [ContentSerializer(Optional = true)]
        public int SpriteIdFacingRight;

        [ContentSerializer(Optional = true)]
        public String SpriteSheetShield;

        [ContentSerializer(Optional = true)]
        public int SpriteIdShield;
    }
}
