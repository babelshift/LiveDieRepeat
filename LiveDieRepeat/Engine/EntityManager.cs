using System;
using System.Collections.Generic;
using LiveDieRepeat.Engine.BulletSystem;
using LiveDieRepeat.Entities;
using LiveDieRepeat.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.Engine
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class EntityManager
    {
        #region Game Data

        private Camera camera;
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;

        #endregion

        private Random random;

        private const int chasePlayerDistance = 75;

        private MapCollection currentLevel;

        Texture2D rectTexture;

        #region Entities

        private PlayerEntity player;
        private List<ProjectileEntity> playerProjectiles;
        private List<ProjectileEntity> hostileProjectiles;
        private List<IEnemy> enemies;
        private List<ItemEntity> items;
        private List<Entity> deathEntities;
        private List<Entity> objects;

        #endregion

        #region Weapons

        #endregion

        #region Event Handlers

        public event EventHandler PlayerShieldedEvent;
        public event EventHandler<WeaponGainedExperienceEventArgs> WeaponGainedExperienceEvent;
        public event EventHandler PlayerPickedUpMoneyEvent;
        public event EventHandler<PlayerHitEventArgs> PlayerHitEvent;
        public event EventHandler<PlayerDeathEventArgs> PlayerDeathEvent;
        public event EventHandler<EnemyKilledEventArgs> EnemyKilledEvent;
        public event EventHandler<PlayerReceivedItemEventArgs> PlayerReceivedItemEvent;

        #endregion

        #region Constructors

        public EntityManager(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        #endregion

        #region Game Loop

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize(GameServiceContainer gameServices, Camera camera, MapCollection currentLevel, PlayerEntity player)
        {
            this.currentLevel = currentLevel;
            this.camera = camera;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.playerProjectiles = new List<ProjectileEntity>();
            this.hostileProjectiles = new List<ProjectileEntity>();
            this.enemies = new List<IEnemy>();
            this.random = new Random(DateTime.Now.Millisecond);
            this.items = new List<ItemEntity>();
            this.deathEntities = new List<Entity>();
            this.objects = new List<Entity>();

            // set the player's position and speed;

            // initialize the player and add to the list of players
            this.player = player;
            this.player.PlayerPowerUpgradeEvent += new EventHandler<PlayerPowerUpgradeEventArgs>(player_PlayerPowerUpgradeEvent);
            this.player.PlayerHitEvent += new EventHandler<PlayerHitEventArgs>(player_PlayerHitEvent);
            this.player.PlayerDeathEvent += new EventHandler<PlayerDeathEventArgs>(player_PlayerDeathEvent);
            this.player.PlayerPickedUpMoneyEvent += new EventHandler(player_PlayerPickedUpMoneyEvent);
            this.player.PlayerShieldedEvent += new EventHandler(player_PlayerShieldedEvent);
            this.player.PlayerReceivedItemEvent += new EventHandler<PlayerReceivedItemEventArgs>(player_PlayerReceivedItemEvent);

            LoadContent(gameServices);
        }

        protected void LoadContent(GameServiceContainer gameServices)
        {
            if (content == null)
                content = new ContentManager(gameServices, "Content");

            player.Activate(content);

            rectTexture = new Texture2D(graphicsDevice, 1, 1);
            rectTexture.SetData(new Color[] { Color.White });

            //todo: is this the best place to register types? should objects be loaded here and then copied when generated?
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.Simple, () => new EnemySimple(content));
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.Simple2, () => new EnemySimpleFast(content));
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.WallHugger, () => new WallHugger(content));
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.Eyeball, () => new EnemyEyeball(content));
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.Brain, () => new EnemyBrain(content));
            Factory<IEnemy>.RegisterType((int)EntityId.Enemy.BossBrain, () => new BossBrain(content, graphicsDevice));
            Factory<Entity>.RegisterType((int)EntityId.DeathAnimation.ExplosionMedium, () => new ExplosionMedium(content));
            Factory<Entity>.RegisterType((int)EntityId.Object.ArrowDown, () => new ArrowDown(content));
            Factory<Entity>.RegisterType((int)EntityId.Object.ArrowUp, () => new ArrowUp(content));
            Factory<Entity>.RegisterType((int)EntityId.Object.ArrowLeft, () => new ArrowLeft(content));
            Factory<Entity>.RegisterType((int)EntityId.Object.ArrowRight, () => new ArrowRight(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.MachineGunSingle, () => new MachineGunSingleBullet(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.MachineGunDouble, () => new MachineGunDoubleBullet(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.MachineGunTriple, () => new MachineGunTripleBullet(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.EnemySimple, () => new EnemyProjectileSimple(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.Rocket, () => new Rocket(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.Fireball, () => new Fireball(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.Mine, () => new Mine(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.Grenade, () => new Grenade(content));
            Factory<ProjectileEntity>.RegisterType((int)EntityId.Projectile.Spread, () => new Spread(content));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.MachineGun, () => new MachineGun(content, typeof(PlayerEntity)));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.RocketLauncher, () => new RocketLauncher(content, typeof(PlayerEntity)));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.FlameThrower, () => new FlameThrower(content, typeof(PlayerEntity)));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.MineLayer, () => new MineLayer(content, typeof(PlayerEntity)));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.GrenadeLauncher, () => new GrenadeLauncher(content, typeof(PlayerEntity)));
            Factory<Weapon>.RegisterType((int)EntityId.Weapon.SpreadGun, () => new SpreadGun(content, typeof(PlayerEntity)));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.SpreadGunPickup, () => new SpreadGunPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.ShieldPickup, () => new ShieldPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.MoneyRedPickup, () => new MoneyRed(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.WeaponPowerUpPickup, () => new WeaponUpgradePickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.FlameThrowerPickup, () => new FlameThrowerPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.MinePickup, () => new MineLayerPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.RocketLauncherPickup, () => new RocketLauncherPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.GrenadeLauncherPickup, () => new GrenadeLauncherPickup(content));
            Factory<ItemEntity>.RegisterType((int)EntityId.Item.MachineGunPickup, () => new MachineGunPickup(content));

            //todo: map events should be started based on map triggers (such as the player entering the map) (in the map collection)
            currentLevel.EnemySpawnedEvent += new EventHandler<EnemySpawnedEventArgs>(currentLevel_EnemySpawnedEvent);
            currentLevel.MapObjectTriggerSpawnObjectEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(currentLevel_MapObjectTriggerSpawnObjectEvent);
            currentLevel.MapObjectTriggerSpawnItemEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(currentLevel_MapObjectTriggerSpawnItemEvent);
            currentLevel.MapObjectTriggerSpawnEnemyEvent += new EventHandler<MapObjectTriggerSpawnEnemyEventArgs>(currentLevel_MapObjectTriggerSpawnEnemyEvent);
            currentLevel.CurrentMap.StartMapEvents();

            // give the player his guns!
            player.GiveItem(Factory<ItemEntity>.Create((int)EntityId.Item.MachineGunPickup));
            //player.GiveWeapon(Factory<Weapon>.Create((int)EntityId.Weapon.RocketLauncher));
            //player.GiveWeapon(Factory<Weapon>.Create((int)EntityId.Weapon.FlameThrower));
            //player.GiveWeapon(Factory<Weapon>.Create((int)EntityId.Weapon.GrenadeLauncher));
            //player.SecondaryWeapon = Factory<Weapon>.Create((int)EntityId.Weapon.RocketLauncher);
            //player.SecondaryWeapon = Factory<Weapon>.Create((int)EntityId.Weapon.SpreadGun);
        }

        //todo: move this garbage

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        //todo: any assets owned by a map should be managed in the map class?
        public void Update(GameTime gameTime)
        {
            UpdateCameraView();

            // update states
            UpdatePlayers(gameTime);
            UpdateProjectiles(gameTime);
            UpdateEnemies(gameTime);
            UpdatePowerUps(gameTime);
            UpdateDeathEntities(gameTime);
            UpdateObjects(gameTime);

            // remove expired entities
            RemoveExpiredEntities();

            // check collisions
            UpdateCollisions();
        }

        //todo: any assets owned by a map should be managed in the map class?
        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.Transform * Resolution.getTransformationMatrix());

            foreach (var mapObject in objects)
                mapObject.Draw(spriteBatch, rectTexture, false);

            foreach (var item in items)
                item.Draw(spriteBatch, rectTexture);

            foreach (var playerProjectile in playerProjectiles)
                playerProjectile.Draw(spriteBatch, rectTexture);

            foreach (var hostileProjectile in hostileProjectiles)
                hostileProjectile.Draw(spriteBatch, rectTexture);

            foreach (var enemy in enemies)
                enemy.Draw(spriteBatch, rectTexture);

            foreach (var deathEntity in deathEntities)
                deathEntity.Draw(spriteBatch, rectTexture);

            foreach (var bulletMover in BulletMoverManager.BulletMovers)
                bulletMover.Draw(spriteBatch, rectTexture);

            player.Draw(spriteBatch, rectTexture);

            RectangleExtensions.DrawRectangle(player.CollisionBox, rectTexture, Color.Red, spriteBatch, false, 1);

            spriteBatch.End();
        }

        public void Unload()
        {
            // clear out entity lists?
            Factory<IEnemy>.Clear();
            Factory<ProjectileEntity>.Clear();
            Factory<Entity>.Clear();
            Factory<Weapon>.Clear();
            Factory<ItemEntity>.Clear();
            BulletMoverManager.FreeBulletMovers();
            BulletMoverManager.ClearBulletMovers();
            content.Unload();
        }

        #endregion

        #region Update Entities

        private void UpdateCameraView()
        {
            // should this class really be shifting the camera?
            // maybe an event should be passed up when the conditional is true
            if (camera.IsToRightOfView(player.CollisionBox.Left))
                currentLevel.ShiftRight();
            else if (camera.IsToLeftOfView(player.CollisionBox.Right))
                currentLevel.ShiftLeft();
            else if (camera.IsAboveView(player.CollisionBox.Bottom))
                currentLevel.ShiftUp();
            else if (camera.IsBelowView(player.CollisionBox.Top))
                currentLevel.ShiftDown();

            camera.Focus = currentLevel.CurrentMap;
        }

        private void UpdateCollisions()
        {
            CollisionManager.HandleCollisions(player, enemies);
            CollisionManager.HandleCollisions(player, BulletMoverManager.BulletMovers);
            CollisionManager.HandleCollisions(player, items);

            // check collisions for entities
            CollisionManager.HandleCollisions(enemies, playerProjectiles);
            
            //todo: need a separate entity type (deathentity?) to implement icollidable
            //CollisionManager.HandleCollisions(enemies, deathEntities);

            // to do: try to save this between updates and only add/remove the differential
            List<ICollidable> collidables = new List<ICollidable>();
            collidables.Add(player);
            collidables.AddRange(playerProjectiles);
            collidables.AddRange(hostileProjectiles);
            collidables.AddRange(items);
            collidables.AddRange(enemies);
            collidables.AddRange(BulletMoverManager.BulletMovers);

            // check collisions for tiles
            foreach (MapObjectLayer objectLayer in currentLevel.CurrentMap.MapObjectLayers)
                CollisionManager.HandleCollisions(objectLayer.GetCollidableObjects(), collidables);

            // to do: this hack sucks. figure out a better spot.
            player.SaveCollisionBox();

            foreach (var enemy in enemies)
                enemy.SaveCollisionBox();
        }

        private void UpdatePowerUps(GameTime gameTime)
        {
            foreach (var item in items)
                item.Update(gameTime, player.Position);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var enemy in enemies)
                enemy.Update(gameTime, player.Position);
        }

        /// <summary>
        /// Loop through all players and update them
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdatePlayers(GameTime gameTime)
        {
            Point mousePosition = MouseHelper.GetCurrentMousePosition(camera);
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            player.Update(gameTime, keyboardState);

            foreach (ItemEntity item in items)
            {
                // if player and power up are within a certain number of pixels of each other, begin chasing player
                if (Vector2.Distance(item.Position, player.Position) < chasePlayerDistance)
                    item.ChasePlayer();
            }

            // is the user firing?
            // should occur from within the entities that can fire
            // entitymanager subscribes to events on the entities for FireWeaponEvent
            // if the event fires, get the projectile from the event args and add to list
            List<ProjectileEntity> newProjectiles = new List<ProjectileEntity>();

            if (mouseState.LeftButton == ButtonState.Pressed)
                newProjectiles.AddRange(player.FireWeapon(gameTime.TotalGameTime, mousePosition, false));
            //else if (mouseState.RightButton == ButtonState.Pressed)
            //    newProjectiles.AddRange(player.FireWeapon(gameTime.TotalGameTime, mousePosition, true));

            // subscribe to death event
            foreach (ProjectileEntity newProjectile in newProjectiles)
                newProjectile.EntityDeathEvent += new EventHandler<EntityDeathEventArgs>(newProjectile_EntityDeathEvent);

            if (newProjectiles.Count > 0 && newProjectiles != null)
                playerProjectiles.AddRange(newProjectiles);

        }

        private void newProjectile_EntityDeathEvent(object sender, EntityDeathEventArgs e)
        {
            if (e.DeathEntityId > 0)
            {
                Entity deathEntity = Factory<Entity>.Create(e.DeathEntityId);
                deathEntity.Position = e.Position;
                deathEntities.Add(deathEntity);
            }
        }

        /// <summary>
        /// Loop through all projectiles and update them.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateProjectiles(GameTime gameTime)
        {
            // Can't use a foreach here because projectiles are being removed from the collection as they leave the screen.
            foreach (Entity projectile in playerProjectiles)
            {
                projectile.Update(gameTime, player.Position);
                if (!camera.IsInView(new Vector2(projectile.CollisionBox.X, projectile.CollisionBox.Y), projectile.CollisionBox.Width, projectile.CollisionBox.Height))
                    projectile.Die();
            }

            foreach (Entity hostileProjectile in hostileProjectiles)
            {
                hostileProjectile.Update(gameTime);
                if (!camera.IsInView(hostileProjectile.Position, hostileProjectile.CollisionBox.Width, hostileProjectile.CollisionBox.Height))
                    hostileProjectile.Die();
            }
        }

        public void UpdateDeathEntities(GameTime gameTime)
        {
            foreach (var deathEntity in deathEntities)
                deathEntity.Update(gameTime);
        }

        public void UpdateObjects(GameTime gameTime)
        {
            foreach (var mapObject in objects)
                mapObject.Update(gameTime);
        }

        #endregion

        #region Special Behaviors

        private void RemoveExpiredEntities()
        {
            RemoveExpiredEntities<ProjectileEntity>(playerProjectiles);
            RemoveExpiredEntities<ProjectileEntity>(hostileProjectiles);
            RemoveExpiredEntities<ItemEntity>(items);
            enemies.RemoveAll(e => e.IsDead);
            RemoveExpiredEntities<Entity>(deathEntities);
        }

        private void RemoveExpiredEntities<T>(List<T> entities) 
            where T : Entity
        {
            entities.RemoveAll(e => e.IsDead);
        }

        private void ResetEntities()
        {
            BulletMoverManager.FreeBulletMovers();
            BulletMoverManager.ClearBulletMovers();
            enemies.Clear();
            hostileProjectiles.Clear();
            playerProjectiles.Clear();
            items.Clear();
            //todo: don't use hard coded index
            player.Respawn(currentLevel.CurrentMap.GetPlayerSpawnPoint());
            player.GiveItem(Factory<ItemEntity>.Create((int)EntityId.Item.MachineGunPickup));
            //player.SecondaryWeapon = Factory<Weapon>.Create((int)EntityId.Weapon.RocketLauncher);
        }

        /// <summary>
        /// Spawns a random power up at the passed position (usually where an enemy dies or a spawn spot on a map)
        /// </summary>
        /// <param name="spawnPosition"></param>
        private void SpawnItem(Vector2 spawnPosition)
        {
            Random randomPower = new Random();
            int randomPowerNumber = randomPower.Next(1, 10);

            ItemEntity newItem = null;

            if (randomPowerNumber == 1)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.ShieldPickup);
            else if (randomPowerNumber == 2)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.WeaponPowerUpPickup);
            else if (randomPowerNumber == 3)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.SpreadGunPickup);
            else if (randomPowerNumber == 4)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.FlameThrowerPickup);
            else if (randomPowerNumber == 5 || randomPowerNumber == 6)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.MoneyRedPickup);
            else if (randomPowerNumber == 7)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.MinePickup);
            else if (randomPowerNumber == 8)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.RocketLauncherPickup);
            else if (randomPowerNumber == 9)
                newItem = Factory<ItemEntity>.Create((int)EntityId.Item.GrenadeLauncherPickup);

            if (newItem != null)
            {
                newItem.Position = spawnPosition;
                items.Add(newItem);
            }
        }

        #endregion

        #region Helper Methods

        public void SwitchPlayerWeapon(int weaponIndex)
        {
            player.SwitchWeapon(weaponIndex);
        }

        #endregion

        #region Events

        private void OnEnemyKilledEvent(EnemyKilledEventArgs e)
        {
            if (EnemyKilledEvent != null)
                EnemyKilledEvent(this, e);
        }

        private void OnPlayerHitEvent(PlayerHitEventArgs e)
        {
            if (PlayerHitEvent != null)
                PlayerHitEvent(this, e);
        }

        private void OnPlayerDeathEvent(PlayerDeathEventArgs e)
        {
            if (PlayerDeathEvent != null)
                PlayerDeathEvent(this, e);

            ResetEntities();
        }

        private void OnPlayerShieldedEvent(EventArgs e)
        {
            if (PlayerShieldedEvent != null)
                PlayerShieldedEvent(this, e);
        }

        private void OnWeaponGainedExperienceEvent(WeaponGainedExperienceEventArgs e)
        {
            if (WeaponGainedExperienceEvent != null)
                WeaponGainedExperienceEvent(this, e);
        }

        private void OnPlayerPickedUpMoneyEvent(EventArgs e)
        {
            if (PlayerPickedUpMoneyEvent != null)
                PlayerPickedUpMoneyEvent(this, e);
        }

        private void OnPlayerPickedUpWeaponEvent(object sender, PlayerReceivedItemEventArgs e)
        {
            if (PlayerReceivedItemEvent != null)
                PlayerReceivedItemEvent(sender, e);
        }

        private void player_PlayerPowerUpgradeEvent(object sender, PlayerPowerUpgradeEventArgs e)
        {
            //todo: update UI somehow? display a message?
        }

        private void player_PlayerHitEvent(object sender, PlayerHitEventArgs e)
        {
            OnPlayerHitEvent(e);
        }

        private void player_PlayerDeathEvent(object sender, PlayerDeathEventArgs e)
        {
            OnPlayerDeathEvent(e);
        }

        private void player_PlayerShieldedEvent(object sender, EventArgs e)
        {
            OnPlayerShieldedEvent(e);
        }

        private void player_PlayerPickedUpMoneyEvent(object sender, EventArgs e)
        {
            OnPlayerPickedUpMoneyEvent(e);
        }

        private void enemy_EnemyKilledEvent(object sender, EnemyKilledEventArgs e)
        {
            OnEnemyKilledEvent(e);

            if (!e.IsSuicideDeath)
                SpawnItem(e.Position);
        }

        private void currentLevel_EnemySpawnedEvent(object sender, EnemySpawnedEventArgs e)
        {
            IEnemy enemy = e.SpawnedEnemy;
            enemy.EnemyKilledEvent += new EventHandler<EnemyKilledEventArgs>(enemy_EnemyKilledEvent);
            enemy.EntityDeathEvent += new EventHandler<EntityDeathEventArgs>(newProjectile_EntityDeathEvent);
            if (enemy is EnemySimple)
                (enemy as EnemySimple).CurrentWeapon = Factory<Weapon>.Create((int)EntityId.Weapon.MachineGun);
            enemies.Add(enemy);
        }

        private void currentLevel_MapObjectTriggerSpawnObjectEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            Entity entity = Factory<Entity>.Create(e.EntityId);
            entity.Position = e.Position;
            objects.Add(entity);
        }

        private void currentLevel_MapObjectTriggerSpawnItemEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            ItemEntity item = Factory<ItemEntity>.Create(e.EntityId);
            item.Position = e.Position;
            items.Add(item);
        }

        private void currentLevel_MapObjectTriggerSpawnEnemyEvent(object sender, MapObjectTriggerSpawnEnemyEventArgs e)
        {
            IEnemy enemy = Factory<IEnemy>.Create(e.EnemyId);
            enemy.Position = e.Position;
            enemy.SetPath(e.Path);
            enemies.Add(enemy);
        }

        private void player_PlayerReceivedItemEvent(object sender, PlayerReceivedItemEventArgs e)
        {
            OnPlayerPickedUpWeaponEvent(sender, e);
        }

        #endregion
    }

    #region Event Arguments
    //todo: should these args be here?
    public class EnemyKilledEventArgs : EventArgs
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }

        private bool isSuicideDeath;
        public bool IsSuicideDeath { get { return this.isSuicideDeath; } }

        public EnemyKilledEventArgs(Vector2 position, bool isSuicideDeath)
        {
            this.position = position;
            this.isSuicideDeath = isSuicideDeath;
        }
    }

    #endregion
}
