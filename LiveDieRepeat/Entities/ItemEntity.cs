using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;
using LiveDieRepeat.Items;
using System.Collections.Generic;

namespace LiveDieRepeat.Entities
{
    public abstract class ItemEntity : Entity, ICollidable
    {
        #region Members

        //private int activatedDuration;
        private bool isChasingPlayer = false;

        private List<ICollidable> collidableComponents = new List<ICollidable>();

        private ScalingDirection scalingDirection;
        private enum ScalingDirection
        {
            Increasing,
            Decreasing
        }

        #endregion

        #region Properties

        public Weapon WeaponAssociated { get; protected set; }

        public virtual IList<ICollidable> CollidableComponents { get { return collidableComponents; } }

        protected override Vector2 Direction
        {
            get
            {
                float xCoord = (float)Math.Cos((double)spriteActive.RadiansOfRotation);
                float yCoord = (float)Math.Sin((double)spriteActive.RadiansOfRotation);

                return new Vector2(xCoord, yCoord);
            }
        }

        #endregion

        #region Constructors

        public ItemEntity(ContentManager content, String entityData)
        {
            base.Activate(content, entityData);
            this.scalingDirection = ScalingDirection.Increasing;
            //collidableComponents.Add(this);
        }
        
        #endregion

        #region Game Loop

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            PulseSize();

            if (isChasingPlayer)
            {
                // rotate towards player
                spriteActive.RadiansOfRotation = (float)Math.Atan2((double)(playerPosition.Y - position.Y), (double)(playerPosition.X - position.X));
                Vector2 direction = Direction;
                double dt = gameTime.ElapsedGameTime.TotalSeconds;
                position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));
            }

            base.Update(gameTime, playerPosition);
        }

        #endregion

        #region Helper Methods

        public void ChasePlayer()
        {
            this.isChasingPlayer = true;
        }

        protected void PulseSize()
        {
            if (spriteActive.ScaleFactor <= 1.2 && scalingDirection == ScalingDirection.Increasing)
                spriteActive.ScaleFactor += .005f;
            else if (spriteActive.ScaleFactor >= 1 && scalingDirection == ScalingDirection.Decreasing)
                spriteActive.ScaleFactor -= 0.005f;

            if (spriteActive.ScaleFactor >= 1.2)
                scalingDirection = ScalingDirection.Decreasing;
            else if (spriteActive.ScaleFactor <= 1)
                scalingDirection = ScalingDirection.Increasing;
        }

        public virtual void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
                if (collidableEntity is PlayerEntity)
                    Die();
        }

        #endregion
    }
}
