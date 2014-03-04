using System;
using ContentPipelineExtensionLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LiveDieRepeat.Entities
{
    public abstract class Entity
    {
        #region Constants

        //private const float MAX_SCALE_AMOUNT = 1.2f;
        //private const float MIN_SCALE_AMOUNT = 1f;
        //private const float SCALE_AMOUNT = .009f;
        //private const float MAX_SHIELD_ALPHA = 1;
        //private const float MIN_SHIELD_ALPHA = 0;
        //private const float ALPHA_AMOUNT = .04f;

        #endregion

        #region Sprites, Positions, Speeds, and Entity Data

        public enum FacingDirection
        {
            Down,
            Up,
            Left,
            Right
        }

        private EntityData entityData;
        protected int deathEntityId;

        protected Vector2 speed;
        protected Vector2 position;
        protected int collisionOffset;
        protected SoundEffect activatedSound;
        protected SoundEffect hitSound;
        protected SoundEffect deathSound;
        protected int health;
        protected int totalHealth;

        private SpriteSheet spriteSheet;
        private Sprite spriteFacingDown;
        private Sprite spriteFacingUp;
        private Sprite spriteFacingLeft;
        private Sprite spriteFacingRight;
        protected Sprite spriteActive;

        private SpriteSheet spriteSheetShield;
        protected Sprite spriteShield;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // fix this garbage
        protected abstract Vector2 Direction { get; }

        public event EventHandler<EntityDeathEventArgs> EntityDeathEvent;

        public bool IsAnimating
        {
            get { return spriteActive.IsAnimating; }
            set { spriteActive.IsAnimating = value; }
        }

        #endregion

        #region Helper Members

        private bool IsShielded = false;
        //private float shieldAlpha = 1;

        //private ScalingDirection scalingDirection;
        //private enum ScalingDirection
        //{
        //    Increasing,
        //    Decreasing
        //}

        public bool IsDead { get; set; }

        /// <summary>Used to determine the collision box bounds associated with the sprite based on a collision offset.
        /// </summary>
        public virtual Rectangle CollisionBox
        {
            get
            {
                int rectX = (int)position.X - (spriteSheet.FrameWidth / 2) - (collisionOffset / 2);
                int rectY = (int)position.Y - (spriteSheet.FrameHeight / 2) - (collisionOffset / 2);
                int rectWidth = spriteSheet.FrameWidth + collisionOffset;
                int rectHeight = spriteSheet.FrameHeight + collisionOffset;

                return new Rectangle(rectX, rectY, rectWidth, rectHeight);
            }
        }

        #endregion

        #region Constructors

        public Entity()
        {
        }

        /// <summary>Default constructor
        /// </summary>
        /// <param name="content">Content manager used to load entity, texture, and sound data and content</param>
        /// <param name="entityDataPath">Path to the XML content containing entity information</param>
        /// <param name="position">Position at which to place the entity upon instantiation</param>
        public Entity(Vector2 position)
        {
            this.position = position;
        }

        protected void Activate(ContentManager content, String entityDataPath)
        {
            this.entityData = content.Load<EntityData>(entityDataPath);

            this.speed = new Vector2(entityData.HorizontalSpeed, entityData.VerticalSpeed);
            this.collisionOffset = entityData.CollisionOffset;

            // load sprite sheet, activate the sprites within it, and get the sprite for this entity
            this.spriteSheet = content.Load<SpriteSheet>(entityData.SpriteSheet);
            int spritesheethash = spriteSheet.GetHashCode();
            List<int> spriteIds = new List<int>();
            spriteIds.Add(entityData.SpriteIdFacingDown);
            spriteIds.Add(entityData.SpriteIdFacingUp);
            spriteIds.Add(entityData.SpriteIdFacingLeft);
            spriteIds.Add(entityData.SpriteIdFacingRight);
            this.spriteSheet.Activate(spriteIds);
            this.spriteFacingDown = this.spriteSheet.GetSprite(entityData.SpriteIdFacingDown);

            if(entityData.SpriteIdFacingUp > 0)
                this.spriteFacingUp = this.spriteSheet.GetSprite(entityData.SpriteIdFacingUp);

            if (entityData.SpriteIdFacingLeft > 0)
                this.spriteFacingLeft = this.spriteSheet.GetSprite(entityData.SpriteIdFacingLeft);

            if (entityData.SpriteIdFacingRight > 0)
                this.spriteFacingRight = this.spriteSheet.GetSprite(entityData.SpriteIdFacingRight);

            this.spriteActive = spriteFacingDown;

            if (!String.IsNullOrEmpty(entityData.SpriteSheetShield))
            {
                this.spriteSheetShield = content.Load<SpriteSheet>(entityData.SpriteSheetShield);
                spriteIds.Clear();
                if (entityData.SpriteIdShield > 0)
                    spriteIds.Add(entityData.SpriteIdShield);
                this.spriteSheetShield.Activate(spriteIds);
                this.spriteShield = spriteSheetShield.GetSprite(entityData.SpriteIdShield);
            }

            this.health = entityData.Health;
            this.totalHealth = entityData.Health;

            if (entityData.SoundActivated != null)
                this.activatedSound = content.Load<SoundEffect>(entityData.SoundActivated);

            if (entityData.SoundHit != null)
                this.hitSound = content.Load<SoundEffect>(entityData.SoundHit);

            if (entityData.SoundDeath != null)
                this.deathSound = content.Load<SoundEffect>(entityData.SoundDeath);
        }

        #endregion

        #region Game Loop Methods

        public virtual void Update(GameTime gameTime)
        {
            if (IsShielded)
            {
                //PulseAndFadeShield();
                spriteShield.Update(gameTime);
            }

            spriteActive.Update(gameTime);
        }

        public virtual void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Update(gameTime);
        }

        public virtual void Update(GameTime gameTime, Vector2 playerPosition)
        {
            Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D rectTexture)
        {
            Draw(spriteBatch, rectTexture, true);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D rectTexture, bool drawShadow)
        {
            if (IsShielded)
            {
                //todo: don't hardcode shield offset
                Vector2 shieldPosition = new Vector2(position.X, position.Y + 18);
                spriteShield.Draw(spriteBatch, spriteSheetShield.Texture, shieldPosition, false);
            }

            spriteActive.Draw(spriteBatch, spriteSheet.Texture, position, drawShadow, 1);

            // causes performance issues on MonoGame Windows 8
            //RectangleExtensions.DrawRectangle(CollisionBox, rectTexture, Color.Red, spriteBatch, false, 1);
        }

        #endregion

        #region Special Behavior Methods

        protected void SetFacingDirection(FacingDirection facingDirection)
        {
            if (facingDirection == FacingDirection.Down)
                spriteActive = spriteFacingDown;
            else if (facingDirection == FacingDirection.Left)
                spriteActive = spriteFacingLeft;
            else if (facingDirection == FacingDirection.Right)
                spriteActive = spriteFacingRight;
            else if (facingDirection == FacingDirection.Up)
                spriteActive = spriteFacingUp;
        }

        // TODO: shot sound should come from weapon, not bullet. move to Weapon class.
        public void PlayActivatedSound()
        {
            //if (activatedSound != null)
            //    activatedSound.Play(0.1f, 1f, 0f);
        }

        public void PlayHitSound()
        {
            //if (hitSound != null)
            //    hitSound.Play(0.4f, 0f, 0f);
        }

        public virtual void Die()
        {
            OnEntityDeathEvent(this, new EntityDeathEventArgs(deathEntityId, position));
        }

        protected void ResetHealth()
        {
            this.health = entityData.Health;
        }

        protected void ActivateShield()
        {
            IsShielded = true;
        }

        protected void DeactivateShield()
        {
            IsShielded = false;
        }

        //private void PulseAndFadeShield()
        //{
        //    if (spriteShield.ScaleFactor <= MAX_SCALE_AMOUNT && scalingDirection == ScalingDirection.Increasing)
        //    {
        //        spriteShield.ScaleFactor += SCALE_AMOUNT;
        //        shieldAlpha -= ALPHA_AMOUNT;
        //    }
        //    else if (spriteShield.ScaleFactor >= MIN_SCALE_AMOUNT && scalingDirection == ScalingDirection.Decreasing)
        //    {
        //        spriteShield.ScaleFactor -= SCALE_AMOUNT;
        //        shieldAlpha += ALPHA_AMOUNT;
        //    }

        //    shieldAlpha = MathHelper.Clamp(shieldAlpha, MIN_SHIELD_ALPHA, MAX_SHIELD_ALPHA);

        //    if (spriteShield.ScaleFactor >= MAX_SCALE_AMOUNT)
        //        scalingDirection = ScalingDirection.Decreasing;
        //    else if (spriteShield.ScaleFactor <= MIN_SCALE_AMOUNT)
        //        scalingDirection = ScalingDirection.Increasing;
        //}

        private void OnEntityDeathEvent(object sender, EntityDeathEventArgs e)
        {
            IsDead = true;

            if (EntityDeathEvent != null)
                EntityDeathEvent(sender, e);
        }

        #endregion
    }

    public class EntityDeathEventArgs : EventArgs
    {
        public int DeathEntityId { get; private set; }
        public Vector2 Position { get; private set; }

        public EntityDeathEventArgs(int deathEntityId, Vector2 position)
        {
            DeathEntityId = deathEntityId;
            Position = position;
        }
    }
}
