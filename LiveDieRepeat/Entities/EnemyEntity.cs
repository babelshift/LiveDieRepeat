using System;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace LiveDieRepeat.Entities
{
    public abstract class EnemyEntity : Entity, IEnemy
    {
        #region Behavior Variables (Determines Random Walk Behaviors)

        protected int durationOfDiversionInSeconds = 2;
        protected int secondsBetweenDiversions = 2;
        protected double timeSinceDiversionStarted = 0;
        protected double timeSinceDiversionEnded = 0;
        protected bool isDiverting = false;
        protected float maxDiversionRadianOffset = .5f;
        protected float minDiversionRadianOffset = .3f;
        private static Random random = new Random();
        protected Queue<Vector2> path = new Queue<Vector2>();
        private List<ICollidable> collidableComponents = new List<ICollidable>();

        #endregion

        public virtual IList<ICollidable> CollidableComponents { get { return collidableComponents; } }

        public bool IsChasingPlayer { get; set; }

        private bool isMovingDown;
        private bool isMovingUp;
        private bool isMovingLeft;
        private bool isMovingRight;

        private Rectangle previousCollisionBox;

        public event EventHandler<EnemyKilledEventArgs> EnemyKilledEvent;

        public EnemyEntity(ContentManager content, String entityData)
        {
            base.Activate(content, entityData);
            //collidableComponents.Add(this);
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            // no path defined, rotate to player
            if (path.Count == 0 && IsChasingPlayer)
                path.Enqueue(playerPosition);

            if(IsChasingPlayer)
                Rotate(playerPosition, gameTime);

            base.Update(gameTime, playerPosition);
        }

        /// <summary>
        /// Rotate our direction based on several factors. If behavior is set to divert, pick a random offset from pointing at the player and move that direction until diversion duration ends.
        /// If behavior is set to chase player, get an angle to the player and move that direction.
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <param name="gameTime"></param>
        private void Rotate(Vector2 playerPosition, GameTime gameTime)
        {
            if (isDiverting)
            {
                timeSinceDiversionStarted += gameTime.ElapsedGameTime.TotalSeconds;

                if (timeSinceDiversionStarted >= durationOfDiversionInSeconds)
                {
                    timeSinceDiversionEnded = 0;
                    timeSinceDiversionStarted = 0;
                    isDiverting = false;
                }
            }
            else
            {
                // by default, let's chase the player
                spriteActive.RadiansOfRotation = (float)Math.Atan2((double)(playerPosition.Y - position.Y), (double)(playerPosition.X - position.X));

                timeSinceDiversionEnded += gameTime.ElapsedGameTime.TotalSeconds;

                if (timeSinceDiversionEnded >= secondsBetweenDiversions)
                {
                    // get a random double between maxDiversionRadianOffset and minDiversionRadianOffset
                    float randomDiversionRotation = (float)random.NextDouble() * (maxDiversionRadianOffset - minDiversionRadianOffset) + maxDiversionRadianOffset;

                    // pick a random + or - direction to divert
                    if (random.Next(0, 2) == 1)
                        spriteActive.RadiansOfRotation += randomDiversionRotation;
                    else
                        spriteActive.RadiansOfRotation -= randomDiversionRotation;

                    isDiverting = true;
                }
            }
        }

        protected void SetMovingDirections(Vector2 previousPosition)
        {
            isMovingDown = false;
            isMovingLeft = false;
            isMovingRight = false;
            isMovingUp = false;

            // moving down-left
            if (position.X < previousPosition.X && position.Y > previousPosition.Y)
            {
                isMovingLeft = true;
                isMovingDown = true;
            }
            // moving left
            else if (position.X < previousPosition.X && position.Y == previousPosition.Y)
                isMovingLeft = true;
            // moving up-left
            else if (position.X < previousPosition.X && position.Y < previousPosition.Y)
            {
                isMovingLeft = true;
                isMovingUp = true;
            }
            // moving down-right
            else if (position.X > previousPosition.X && position.Y > previousPosition.Y)
            {
                isMovingRight = true;
                isMovingDown = true;
            }
            // moving up-right
            else if (position.X > previousPosition.X && position.Y == previousPosition.Y)
                isMovingRight = true;
            else if (position.X > previousPosition.X && position.Y < previousPosition.Y)
            {
                isMovingRight = true;
                isMovingUp = true;
            }
        }

        public void SetPath(IEnumerable<Vector2> path)
        {
            foreach (Vector2 destination in path)
                this.path.Enqueue(destination);
        }

        public virtual void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is PlayerEntity)
                {
                    OnEnemyKilledEvent(true);
                }
                else if (collidableEntity is ProjectileEntity)
                {
                    ProjectileEntity projectile = collidableEntity as ProjectileEntity;

                    if (projectile.HasOwner())
                    {
                        if (projectile.Owner.Equals(typeof(PlayerEntity)))
                        {
                            // only count as a hit if the projectile is alive and I'm alive
                            if (!projectile.IsDead)
                            {
                                //todo: get the death entity id from the projectile itself?
                                if (projectile is Rocket)
                                    deathEntityId = (int)EntityId.DeathAnimation.ExplosionMedium;

                                // ack, we've been hit, notify our subscribers
                                OnEnemyKilledEvent(false);
                            }
                        }
                    }
                }
                else if (collidableEntity is MapObject)
                {
                    MapObject collidableMapObject = collidableEntity as MapObject;

                    if (collidableMapObject.IsCollidable)
                    {
                        // calculate how deep the intersection is
                        Vector2 collisionDepth = CollisionBox.GetIntersectionDepth(collidableMapObject.CollisionBox);

                        // no intersection, so no collision!
                        if (collisionDepth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(collisionDepth.X);
                            float absDepthY = Math.Abs(collisionDepth.Y);

                            // this offset will push the entity 1px beyond the depth correction
                            float offsetY = collisionDepth.Y < 0 ? -1f : 1f;
                            float offsetX = collisionDepth.X < 0 ? -1f : 1f;

                            // this vector resolves collision in the Y direction
                            // entity keeps same X coordinate but moves from current Y to Y + the intersection depth correction factor + the offset
                            Vector2 resolutionPositionY = new Vector2(position.X, position.Y + collisionDepth.Y + offsetY);

                            // this vector resolves collision in the X direction
                            // entity keeps same Y coordinate but moves from current X to X + the intersection depth correction factor + the offset
                            Vector2 resolutionPositionX = new Vector2(position.X + collisionDepth.X + offsetX, position.Y);

                            // collision is less severe in the Y direction, so correct in favor of Y
                            if (absDepthY < absDepthX)
                                position = resolutionPositionY;
                            // collision is less sever in the X direction, so correct in favor of X
                            else if (absDepthX < absDepthY)
                                position = resolutionPositionX;
                            // collision is equally severe in both the X and the Y directions, so we need to determine which direction the player is moving and
                            // on which side of the boxes the collision is occurring
                            else
                            {
                                if (isMovingDown && isMovingLeft)
                                {
                                    // our bottom passed the top of a tile, we hit the floor, correct us above the floor
                                    if (previousCollisionBox.Bottom <= collidableMapObject.CollisionBox.Top)
                                        position = resolutionPositionY;
                                    // our left passed the right of a tile, we hit the left wall, correct us to the right of the wall
                                    else if (previousCollisionBox.Left <= collidableMapObject.CollisionBox.Right)
                                        position = resolutionPositionX;
                                }
                                else if (isMovingUp && isMovingLeft)
                                {
                                    // our top passed the bottom of a tile, we hit the ceiling, correct us below the ceiling
                                    if (previousCollisionBox.Top <= collidableMapObject.CollisionBox.Bottom)
                                        position = resolutionPositionY;
                                    // our left passed the right of a tile, we hit the left wall, correct us to the right of the wall
                                    else if (previousCollisionBox.Left <= collidableMapObject.CollisionBox.Right)
                                        position = resolutionPositionX;
                                }
                                else if (isMovingUp && isMovingRight)
                                {
                                    // our bottom passed the top of a tile, we hit the floor, correct us above the floor
                                    if (previousCollisionBox.Top <= collidableMapObject.CollisionBox.Bottom)
                                        position = resolutionPositionY;
                                    // our right passed the left of a tile, we hit the right wall, correct us to the left of the wall
                                    else if (previousCollisionBox.Right >= collidableMapObject.CollisionBox.Left)
                                        position = resolutionPositionX;
                                }
                                else if (isMovingDown && isMovingRight)
                                {
                                    // our top passed the bottom of a tile, we hit the ceiling, correct us below the ceiling
                                    if (previousCollisionBox.Bottom <= collidableMapObject.CollisionBox.Top)
                                        position = resolutionPositionY;
                                    // our right passed the left of a tile, we hit the right wall, correct us to the left of the wall
                                    else if (previousCollisionBox.Right >= collidableMapObject.CollisionBox.Left)
                                        position = resolutionPositionX;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SaveCollisionBox()
        {
            previousCollisionBox = CollisionBox;
        }

        protected bool TakeDamage(int damage)
        {
            IsAnimating = true;

            // if player has more health than damage amount, subtract the damage from his health
            if (health >= damage)
            {
                health -= damage;

                if (health == 0)
                    return true;            // no more health, dead
                else
                    return false;           // i'm not dead yet
            }
            else
                return true;                // player will die from this hit, so don't even calculate, just die
        }

        private void OnEnemyKilledEvent(bool isSuicideDeath)
        {
            Die();

            Vector2 enemyDeathPosition = new Vector2(CollisionBox.Center.X, CollisionBox.Center.Y);

            EnemyKilledEventArgs e = new EnemyKilledEventArgs(enemyDeathPosition, isSuicideDeath);

            if (EnemyKilledEvent != null)
                EnemyKilledEvent(this, e);
        }
    }
}
