using System;
using ContentPipelineExtensionLibrary;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace LiveDieRepeat.Entities
{
    public abstract class ProjectileEntity : Entity, ICollidable
    {
        //private int damage;
        //private int timeToLive;

        private List<ICollidable> collidableComponents = new List<ICollidable>();

        private Type owner;
        public Type Owner { get { return this.owner; } }

        public bool HasOwner()
        {
            if (owner != null)
                return true;
            else
                return false;
        }

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

        public ProjectileEntity(ContentManager content, String entityDataPath)
        {
            base.Activate(content, entityDataPath);
            EntityData entityData = content.Load<EntityData>(entityDataPath);
            //collidableComponents.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);

            base.Update(gameTime);
        }

        public void Spawn(Vector2 position, float radiansOfRotation, Type owner)
        {
            this.position = position;
            this.spriteActive.RadiansOfRotation = radiansOfRotation;
            this.owner = owner;
        }

        /// <summary>
        /// TODO: I'm afraid this might be in the wrong class. Not all ProjectileEntity objects should be going towards the mouse pointer.
        /// Maybe it should take in a destination position instead of assuming the mouse position? (would need to be made 
        /// </summary>
        private void Move(GameTime gameTime)
        {
            Vector2 direction = Direction;
            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));
        }

        public virtual void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is PlayerEntity)
                {
                    if (HasOwner())
                    {
                        if (owner.Equals(typeof(EnemyEntity)))
                        {
                            Die();
                        }
                    }
                }
                else if (collidableEntity is IEnemy)
                {
                    if (HasOwner())
                    {
                        if (owner.Equals(typeof(PlayerEntity)))
                        {
                            Die();
                        }
                    }
                }
                else if (collidableEntity is MapObject)
                {
                    MapObject collidableMapObject = collidableEntity as MapObject;

                    if (collidableMapObject.IsCollidable)
                    {
                        Die();
                    }
                }
            }
        }
    }
}
