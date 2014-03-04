using System;
using System.Collections.Generic;
using LiveDieRepeat.Engine;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LiveDieRepeat.Engine.BulletSystem;

namespace LiveDieRepeat.Entities
{
    public class PlayerEntity : Entity, ICollidable
    {
        public static String ENTITY_DATA = "Entities/Player";

        #region Private Members

        private Rectangle previousCollisionBox;
        private ShieldPickup shieldPower;
        private List<Weapon> primaryWeapons = new List<Weapon>();
        private int currentPrimaryWeaponIndex = 0;
        private bool isMovingDown;
        private bool isMovingUp;
        private bool isMovingLeft;
        private bool isMovingRight;
        private const int DEFAULT_LIVES = 3;
        private int currentLives = DEFAULT_LIVES;
        private bool isFrozen = false;
        private List<ICollidable> collidableComponents = new List<ICollidable>();

        #endregion

        #region Properties

        public virtual IList<ICollidable> CollidableComponents { get { return collidableComponents; } }

        public override Rectangle CollisionBox
        {
            get
            {
                float scale = 0.5f;

                int xCoord = (int)(position.X - (spriteActive.Origin.X * scale));
                int yCoord = (int)(position.Y - (spriteActive.Origin.Y * scale));
                int width = (int)(spriteActive.FrameWidth * scale);
                int height = (int)(spriteActive.FrameHeight * scale);

                return new Rectangle(xCoord, yCoord, width, height);
            }
        }

        public Weapon PrimaryWeapon
        {
            get { return primaryWeapons[(int)currentPrimaryWeaponIndex]; }
        }

        public Weapon SecondaryWeapon { get; set; }

        protected override Vector2 Direction
        {
            get
            {
                // don't allow movement if the player is frozen
                if (!isFrozen)
                {
                    isMovingLeft = false;
                    isMovingRight = false;
                    isMovingDown = false;
                    isMovingUp = false;

                    Vector2 inputDirection = Vector2.Zero;
                    KeyboardState keyboardState = Keyboard.GetState();

                    if (keyboardState.IsKeyDown(Keys.A)) // left
                    {
                        isMovingLeft = true;
                        inputDirection.X -= 1;
                    }

                    if (keyboardState.IsKeyDown(Keys.D)) // right
                    {
                        isMovingRight = true;
                        inputDirection.X += 1;
                    }

                    if (keyboardState.IsKeyDown(Keys.W)) // up
                    {
                        isMovingUp = true;
                        inputDirection.Y -= 1;
                    }

                    if (keyboardState.IsKeyDown(Keys.S)) // down
                    {
                        isMovingDown = true;
                        inputDirection.Y += 1;
                    }

                    return inputDirection;
                }
                else
                    return Vector2.Zero;
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler PlayerShieldedEvent;
        public event EventHandler PlayerPickedUpMoneyEvent;
        public event EventHandler<PlayerHitEventArgs> PlayerHitEvent;
        public event EventHandler<PlayerDeathEventArgs> PlayerDeathEvent;
        public event EventHandler<PlayerPowerUpgradeEventArgs> PlayerPowerUpgradeEvent;
        public event EventHandler<PlayerReceivedItemEventArgs> PlayerReceivedItemEvent;

        #endregion

        #region Constructors

        public PlayerEntity(Vector2 position)
            : base(position)
        {
            //collidableComponents.Add(this);
        }

        #endregion

        #region Game Loop

        public void Activate(ContentManager content)
        {
            base.Activate(content, ENTITY_DATA);
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            HandleInput(keyboardState, gameTime.ElapsedGameTime.TotalSeconds);

            SetSpriteFacingDirection();

            if (shieldPower != null)
                shieldPower.Update(gameTime);

            base.Update(gameTime);
        }

        #endregion

        #region Helpers

        private void SetSpriteFacingDirection()
        {
            if (isMovingLeft == false && isMovingRight == false && isMovingDown == false && isMovingUp == false)
                spriteActive.IsAnimating = false;
            else
                spriteActive.IsAnimating = true;

            if (isMovingLeft)
                SetFacingDirection(FacingDirection.Left);

            if (isMovingRight)
                SetFacingDirection(FacingDirection.Right);

            if (isMovingUp)
                SetFacingDirection(FacingDirection.Up);

            if (isMovingDown)
                SetFacingDirection(FacingDirection.Down);
        }

        public List<ProjectileEntity> FireWeapon(TimeSpan timeOfShot, Point mousePosition, bool isAlternateFire)
        {
            List<ProjectileEntity> shots = new List<ProjectileEntity>();

            // TODO: think of a better way to evaluate powerup effects outside of the player class
            //if (activeOffensivePower is MultishotPower)
            //{
            //    float distanceFromPlayer = 10.0f;
            //    float offsetRotation = (float)Math.PI / 5f;

            //    float firstShotPositionX = position.X + (float)(Math.Cos(sprite.RadiansOfRotation + offsetRotation) * distanceFromPlayer);
            //    float firstShotPositionY = position.Y + (float)(Math.Sin(sprite.RadiansOfRotation + offsetRotation) * distanceFromPlayer);
            //    Vector2 firstShotPosition = new Vector2(firstShotPositionX, firstShotPositionY);

            //    float secondShotPositionX = position.X + (float)(Math.Cos(sprite.RadiansOfRotation - offsetRotation) * distanceFromPlayer);
            //    float secondShotPositionY = position.Y + (float)(Math.Sin(sprite.RadiansOfRotation - offsetRotation) * distanceFromPlayer);
            //    Vector2 secondShotPosition = new Vector2(secondShotPositionX, secondShotPositionY);

            //    ProjectileEntity firstShot = currentWeapon.Fire(firstShotPosition, sprite.RadiansOfRotation, timeOfShot);
            //    ProjectileEntity secondShot = currentWeapon.Fire(secondShotPosition, sprite.RadiansOfRotation, timeOfShot);

            //    if (firstShot != null)
            //        shots.Add(firstShot);
            //    if (secondShot != null)
            //        shots.Add(secondShot);
            //}
            //else
            //{

            List<ProjectileEntity> newProjectiles = new List<ProjectileEntity>();
            float radians = (float)Math.Atan2((double)(mousePosition.Y - position.Y), (double)(mousePosition.X - position.X));

            //if (isAlternateFire)
            //    newProjectiles = SecondaryWeapon.Fire(position, radians, timeOfShot);
            //else
            newProjectiles = PrimaryWeapon.Fire(position, radians, timeOfShot);

            if (newProjectiles != null)
                shots.AddRange(newProjectiles);

            return shots;
        }

        //private void RotateToMousePosition(Point mousePosition)
        //{
        //    float radians = (float)Math.Atan2((double)(mousePosition.Y - position.Y), (double)(mousePosition.X - position.X));

        //    //spriteShielded.RadiansOfRotation = radians;
        //    //spriteActive.RadiansOfRotation = radians;
        //}

        /// <summary>
        /// i don't like this because we have to pass a pointer to the level on each move update
        /// is there a way to make this better? maybe move the logic up a few levels with a collision detection class
        /// but we will also need to account for player position after an update, so how can we predict that outside of this class?
        /// </summary>
        /// <param name="level"></param>
        private void Move(double dt)
        {
            Vector2 previousPosition = position;
            Vector2 direction = Direction;
            position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));
        }

        private void HandleInput(KeyboardState keyboardState, double dt)
        {
            //RotateToMousePosition(mousePosition);

            Move(dt);
        }

        /// <summary>If the weapon slot that we are switching to is populated with a gun, set it as our currently selected gun
        /// </summary>
        /// <param name="weaponIndex"></param>
        public void SwitchWeapon(int weaponIndex)
        {
            if (IsWeaponSlotPopulated(weaponIndex))
                currentPrimaryWeaponIndex = weaponIndex;
        }

        /// <summary>A weapon slot is populated if a gun is placed at the index. Acquiring the same type of gun twice does nothing.
        /// </summary>
        /// <param name="weaponSlotIndex"></param>
        /// <returns></returns>
        private bool IsWeaponSlotPopulated(int weaponSlotIndex)
        {
            bool isWeaponSlotPopulated = false;

            if (primaryWeapons.Count > weaponSlotIndex)
                if (primaryWeapons[weaponSlotIndex] != null)
                    isWeaponSlotPopulated = true;

            return isWeaponSlotPopulated;
        }

        // move power up evaluations out of the player class
        /// <summary>
        /// Subtract the damage amount from the player's health. 
        /// </summary>
        /// <param name="damage">Damage should be based on the bullet that hits the player.</param>
        /// <returns>Returns true if the player dies as a result of taking damage</returns>
        private bool TakeDamage(int damage)
        {
            // TODO: think of a better way to evaluate powerup effects outside of the player class
            // if the user has an active shield power up, don't take damage
            if (shieldPower is ShieldPickup)
            {
                ShieldPickup shield = (ShieldPickup)shieldPower;

                int chargesRemaining = shield.RemoveCharge(1); // TODO: remove charge based on damage type, not a constant

                if (chargesRemaining == 0)
                {
                    RevokePower(shieldPower);
                    return false;
                }
                else
                    return false;           // still have charges left, player doesn't die and takes no damage
            }

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

        public void Respawn(Vector2 respawnPosition)
        {
            position = respawnPosition;
            ResetHealth();
            IsDead = false;
        }

        //public bool HasPower(Type power)
        //{
        //    bool hasPower = false;

        //    if (shieldPower != null)
        //        if (shieldPower.GetType().Equals(power))
        //            hasPower = true;

        //    return hasPower;
        //}

        // TODO: create a base class like ProjectileEntity for PowerEntity
        public void GivePower(Entity power)
        {
            if (power is ShieldPickup)
            {
                shieldPower = (ShieldPickup)power;
                ActivateShield();
            }
            else if (power is WeaponUpgradePickup)
            {
                WeaponUpgradePickup newPower = (WeaponUpgradePickup)power;
                newPower.IsPickedUp = true;
                PrimaryWeapon.AddExperienceToWeapon();
            }
        }

        public void RevokePower(Entity power)
        {
            if (power is ShieldPickup)
            {
                shieldPower = null;
                DeactivateShield();
            }
        }

        public void ResolveCollision(ICollidable collidableEntity)
        {
            // i only care about my collisions if i'm alive
            if (!IsDead)
            {
                if (collidableEntity is IEnemy)
                {
                    // do player-enemy collision
                    OnPlayerHitEvent();
                }
                else if (collidableEntity is ItemEntity)
                {
                    ItemEntity item = collidableEntity as ItemEntity;

                    GivePower(item);

                    // do player-item collision
                    // player gains item
                    if (item is ShieldPickup)
                        OnPlayerShieldedEvent(EventArgs.Empty);
                    else if (item is MoneyRed)
                        OnPlayerPickedUpMoneyEvent(EventArgs.Empty);

                }
                else if (collidableEntity is BulletMover)
                {
                    OnPlayerHitEvent();
                }
                else if (collidableEntity is ProjectileEntity)
                {
                    // do player-projectile collision
                    // player takes damage IF the projectile is hostile
                    ProjectileEntity projectile = collidableEntity as ProjectileEntity;

                    if (projectile.HasOwner())
                    {
                        if (projectile.Owner.Equals(typeof(EnemyEntity)))
                        {
                            // ack, we've been hit, notify our subscribers
                            OnPlayerHitEvent();
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
            else
                OnPlayerDeathEvent(new PlayerDeathEventArgs(currentLives));
        }

        public void SaveCollisionBox()
        {
            previousCollisionBox = CollisionBox;
        }

        /// <summary>Give the player the passed item. For now, this method assumes that the item has an associated weapon with it. This is ugly and should not be here forever.
        /// todo: do not associate weapons with all items.
        /// </summary>
        /// <param name="item"></param>
        public void GiveItem(ItemEntity item)
        {
            bool alreadyHasWeapon = false;

            foreach (Weapon existingWeapon in primaryWeapons)
                if (existingWeapon.GetType().Equals(item.WeaponAssociated.GetType()))
                    alreadyHasWeapon = true;

            if (!alreadyHasWeapon)
            {
                primaryWeapons.Add(item.WeaponAssociated);
                OnPlayerReceivedItemEvent(this, new PlayerReceivedItemEventArgs(item));
            }
        }

        //todo: don't override this, make player die just like every other entity
        public override void Die()
        {
            IsDead = true;

            OnPlayerDeathEvent(new PlayerDeathEventArgs(currentLives));
        }

        #endregion

        #region Events

        private void OnPlayerPowerUpgradeEvent(PlayerPowerUpgradeEventArgs e)
        {
            if (PlayerPowerUpgradeEvent != null)
                PlayerPowerUpgradeEvent(this, e);
        }

        private void OnPlayerHitEvent()
        {
            if (TakeDamage(1))
                Die();

            float healthPercentRemaining = ((float)health / (float)totalHealth) * 100;

            float shieldPercentRemaining = 0;

            if (shieldPower != null)
                shieldPercentRemaining = ((float)shieldPower.ChargesRemaining / (float)shieldPower.TotalCharges) * 100;

            PlayerHitEventArgs e = new PlayerHitEventArgs(healthPercentRemaining, shieldPercentRemaining);

            if (PlayerHitEvent != null)
                PlayerHitEvent(this, e);
        }

        private void OnPlayerDeathEvent(PlayerDeathEventArgs e)
        {
            currentLives--;

            if (PlayerDeathEvent != null)
                PlayerDeathEvent(this, e);
        }

        private void OnPlayerShieldedEvent(EventArgs e)
        {
            if (PlayerShieldedEvent != null)
                PlayerShieldedEvent(this, e);
        }

        private void OnPlayerPickedUpMoneyEvent(EventArgs e)
        {
            if (PlayerPickedUpMoneyEvent != null)
                PlayerPickedUpMoneyEvent(this, e);
        }

        protected void OnPlayerReceivedItemEvent(object sender, PlayerReceivedItemEventArgs e)
        {
            if (PlayerReceivedItemEvent != null)
                PlayerReceivedItemEvent(sender, e);
        }

        #endregion
    }

    #region Event Arguments

    public class PlayerPowerUpgradeEventArgs : EventArgs
    {
        private int powerLevel;
        public int PowerLevel { get { return powerLevel; } }

        public PlayerPowerUpgradeEventArgs(int powerLevel)
        {
            this.powerLevel = powerLevel;
        }
    }

    public class PlayerHitEventArgs : EventArgs
    {
        public float HealthPercentRemaining { get; private set; }
        public float ShieldPercentRemaining { get; private set; }

        public PlayerHitEventArgs(float healthPercentRemaining, float shieldPercentRemaining)
        {
            ShieldPercentRemaining = shieldPercentRemaining;
            HealthPercentRemaining = healthPercentRemaining;
        }
    }

    public class PlayerDeathEventArgs : EventArgs
    {
        private int playerLives;
        public int PlayerLives { get { return playerLives; } }

        public PlayerDeathEventArgs(int playerLives)
        {
            this.playerLives = playerLives;
        }
    }

    public class PlayerReceivedItemEventArgs : EventArgs
    {
        public ItemEntity ItemReceived { get; private set; }

        public PlayerReceivedItemEventArgs(ItemEntity itemReceived)
        {
            ItemReceived = itemReceived;
        }
    }

    #endregion
}
