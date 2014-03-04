using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ContentPipelineExtensionLibrary;
using BulletMLLib;
using LiveDieRepeat.Engine.BulletSystem;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.Engine
{
    public class Path
    {
        public int Id;
        public List<Vector2> Destinations = new List<Vector2>();
    }

    /// <summary>
    /// Represents a single Tile in a Layer. Implements ICollidable so the CollisionManager can block movement on Collidable tiles.
    /// </summary>
    public class Tile
    {
        //[ContentSerializerIgnore]
        //public Rectangle CollisionBox
        //{
        //    get
        //    {
        //        int rectX = (int)Position.X;
        //        int rectY = (int)Position.Y;
        //        int rectWidth = Texture.Width - (Texture.Width - SourceRectangle.Width);
        //        int rectHeight = Texture.Height - (Texture.Height - SourceRectangle.Height);

        //        return new Rectangle(rectX, rectY, rectWidth, rectHeight);
        //    }
        //}

        [ContentSerializerIgnore]
        public Vector2 Position { get; set; }

        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public SpriteEffects SpriteEffects;
    }

    /// <summary>
    /// Represents a single Layer in a Map. Made up of many Tiles.
    /// </summary>
    public class Layer
    {
        public int Width;
        public int Height;
        public List<Tile> Tiles;
    }

    /// <summary>
    /// Represents a single map object from a Tiled editor object layer
    /// </summary>
    public class MapObject : ICollidable, ITriggerSubscriber
    {
        #region XML Content

        public int Id { get; set; }
        public Rectangle Bounds;
        public String Name;
        public String Type;
        public bool IsCollidable;

        #endregion

        /// <summary>
        /// Offset is needed because each map is contained to a single screen. A level is built as a grid of maps. Each maps grid location determines its offset.
        /// A map like this where M0 is the starting map will have M1 with a Y offset of 1 and M2 will have a X offset of 1 and Y offset of 1.
        /// [M0]
        /// [M1][M2]
        /// </summary>
        [ContentSerializerIgnore]
        public Vector2 Offset { get; set; }

        private List<ICollidable> collidableComponents = new List<ICollidable>();

        [ContentSerializerIgnore]
        public virtual IList<ICollidable> CollidableComponents { get { return collidableComponents; } }

        [ContentSerializerIgnore]
        public Rectangle CollisionBox
        { 
            get 
            {
                return new Rectangle(Bounds.X + (int)Offset.X, Bounds.Y + (int)Offset.Y, Bounds.Width, Bounds.Height);
            }
        }

        [ContentSerializerIgnore]
        public Vector2 Position { get; set; }

        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnObjectEvent;
        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnItemEvent;
        public event EventHandler<MapObjectTriggerSpawnEnemyEventArgs> MapObjectTriggerSpawnEnemyEvent;

        public void ResolveCollision(ICollidable e) { }

        public void PerformAction(TriggerAction triggerAction)
        {
            if (triggerAction.Type == "Open")
                IsCollidable = false;
            else if (triggerAction.Type == "SpawnItem")
                OnMapObjectTriggerSpawnItemEvent(triggerAction.EntityId);
            else if (triggerAction.Type == "SpawnEnemy")
                OnMapObjectTriggerSpawnEnemyEvent(triggerAction.EntityId, triggerAction.Path);
            else if (triggerAction.Type == "SpawnObject")
                OnMapObjectTriggerSpawnObjectEvent(triggerAction.EntityId);

            // how to play a door open animation?
        }

        private void OnMapObjectTriggerSpawnObjectEvent(int objectId)
        {
            if (MapObjectTriggerSpawnObjectEvent != null)
                MapObjectTriggerSpawnObjectEvent(this, new MapObjectTriggerSpawnEntityEventArgs(objectId, new Vector2(CollisionBox.Center.X, CollisionBox.Center.Y)));
        }

        private void OnMapObjectTriggerSpawnItemEvent(int itemId)
        {
            if (MapObjectTriggerSpawnItemEvent != null)
                MapObjectTriggerSpawnItemEvent(this, new MapObjectTriggerSpawnEntityEventArgs(itemId, new Vector2(CollisionBox.Center.X, CollisionBox.Center.Y)));
        }

        private void OnMapObjectTriggerSpawnEnemyEvent(int enemyId, List<Vector2> path)
        {
            if (MapObjectTriggerSpawnEnemyEvent != null)
                MapObjectTriggerSpawnEnemyEvent(this, new MapObjectTriggerSpawnEnemyEventArgs(enemyId, new Vector2(CollisionBox.Center.X, CollisionBox.Center.Y), path));
        }
    }

    /// <summary>
    /// Represents a single object layer from Tiled editor. Made up of many map objects.
    /// </summary>
    public class MapObjectLayer
    {
        public String Name;
        public List<MapObject> MapObjects = new List<MapObject>();

        // todo: automatic property
        private List<MapObject> collidableObjects = new List<MapObject>();

        public List<MapObject> GetCollidableObjects()
        {
            return collidableObjects;
        }

        public void AddCollidableObject(MapObject mapObject)
        {
            collidableObjects.Add(mapObject);
        }
    }

    /// <summary>
    /// Represents a single Map from Tiled Map Editor. Made up of many Layers. Call Initialize() to establish positions for the Tiles in each Layer prior to Draw.
    /// TO DO: establish a map width and height based on layers? hmm...
    /// </summary>
    public class Map : IFocusable
    {
        #region XML Content

        public int TileWidth;
        public int TileHeight;
        public Vector2 GridPosition;
        public String BulletSpawner;
        public List<Layer> Layers = new List<Layer>();
        public List<MapObjectLayer> MapObjectLayers = new List<MapObjectLayer>();
        public List<MapEvent> Events = new List<MapEvent>();
        public List<Path> Paths = new List<Path>();

        #endregion

        [ContentSerializerIgnore]
        public Vector2 FocusPosition { get; private set; }

        [ContentSerializerIgnore]
        public List<MapObject> enemySpawnPoints;

        [ContentSerializerIgnore]
        public List<MapObject> bulletSpawnPoints;

        [ContentSerializerIgnore]
        public List<MapObject> playerSpawnPoints;

        [ContentSerializerIgnore]
        private List<ITriggerSubscriber> subscriberObjects = new List<ITriggerSubscriber>();

        private int offsetX;
        private int offsetY;
        private Random random;
        private BulletMLParser parser = new BulletMLParser();
        private int bulletTimer = 0;
        private BulletMover bulletMover;
        private SpriteSheet bulletSheet;
        private Sprite bulletSprite;

        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnObjectEvent;
        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnItemEvent;
        public event EventHandler<MapObjectTriggerSpawnEnemyEventArgs> MapObjectTriggerSpawnEnemyEvent;
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawnedEvent;
        //public event EventHandler<MapEventEndedEventArgs> MapEventEndedEvent;

        /// <summary>
        /// On initialization, assign positions and properties to tiles and map objects
        /// </summary>
        public void Initialize(ContentManager content, PlayerEntity player)
        {
            offsetX = (int)GridPosition.X * Resolution.VirtualViewport.Width;
            offsetY = (int)GridPosition.Y * Resolution.VirtualViewport.Height;

            random = new Random();
            SetupMapObjects();
            SetupTiles();
            SetupSpawnPoints();
            SetupBulletSpawner(content, player);
        }

        /// <summary>
        /// Update any events that this map owns
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, Camera camera)
        {
            foreach (var mapEvent in Events)
                mapEvent.Update(gameTime);

            UpdateBullets(gameTime, camera);
        }

        /// <summary>
        /// Loops through all tiles in all layers in the map and draws them appropriately.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var l in Layers)
            {
                for (int y = 0; y < l.Height; y++)
                {
                    for (int x = 0; x < l.Width; x++)
                    {
                        Tile t = l.Tiles[y * l.Width + x];
                        int positionX = (int)t.Position.X;
                        int positionY = (int)t.Position.Y;

                        // only draw tiles that are in the camera's view
                        // probably should apply this globally to all dynamic elements some how
                        // only draw if there is a texture (blank cells in Tiled are deserialized as empty textures)
                        if (camera.IsInView(new Vector2(positionX, positionY), TileWidth, TileHeight)
                            && t.Texture != null)
                        {
                            spriteBatch.Draw(
                                t.Texture,
                                new Rectangle(positionX, positionY, TileWidth, TileHeight),
                                t.SourceRectangle,
                                Color.White,
                                0,
                                Vector2.Zero,
                                t.SpriteEffects,
                                0);

                        }
                    }
                }
            }
        }

        private void UpdateBullets(GameTime gameTime, Camera camera)
        {
            if (!String.IsNullOrEmpty(BulletSpawner))
            {
                bulletTimer++;
                if (bulletTimer > 60)
                {
                    bulletTimer = 0;
                    bulletMover.used = false;

                    if (bulletMover.used == false)
                    {
                        bulletMover = BulletMoverManager.CreateBulletMover(bulletSheet, bulletSprite, GetRandomBulletSpawnLocation(), parser.tree);
                    }
                }

                BulletMoverManager.Update(gameTime, camera);
                BulletMoverManager.FreeBulletMovers();
            }
        }

        public void StartMapEvents()
        {
            StartMapEvent(0);
        }

        // todo: don't take an index, instead take an event id
        /// <summary>
        /// Start the map event based on the passed event id
        /// </summary>
        /// <param name="index"></param>
        private void StartMapEvent(int index)
        {
            if (Events.Count - 1 >= index)
            {
                Events[index].EnemySpawnedEvent += new EventHandler<EnemySpawnedEventArgs>(Map_EnemySpawnedEvent);
                Events[index].Start(subscriberObjects);
            }
        }

        /// <summary>
        /// When one of our events tells us an enemy has spawned, assign its position to a spawn point that this map owns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_EnemySpawnedEvent(object sender, EnemySpawnedEventArgs e)
        {
            int randomSpawnId = random.Next(0, enemySpawnPoints.Count);

            e.SpawnedEnemy.Position = GetRandomEnemySpawnLocation(randomSpawnId);

            if (EnemySpawnedEvent != null)
                EnemySpawnedEvent(sender, e);
        }

        private void SetupMapObjects()
        {
            foreach (MapObjectLayer mapObjectLayer in MapObjectLayers)
            {
                foreach (MapObject mapObject in mapObjectLayer.MapObjects)
                {
                    mapObject.Offset = new Vector2(offsetX, offsetY);

                    if (mapObject.IsCollidable)
                        mapObjectLayer.AddCollidableObject(mapObject);

                    if (mapObject.Id > 0)
                    {
                        mapObject.MapObjectTriggerSpawnObjectEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(mapObject_MapObjectTriggerSpawnObjectEvent);
                        mapObject.MapObjectTriggerSpawnItemEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(mapObject_MapObjectTriggerSpawnItemEvent);
                        mapObject.MapObjectTriggerSpawnEnemyEvent += new EventHandler<MapObjectTriggerSpawnEnemyEventArgs>(mapObject_MapObjectTriggerSpawnEnemyEvent);
                        subscriberObjects.Add(mapObject);
                    }
                }
            }
        }

        private void mapObject_MapObjectTriggerSpawnObjectEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            if (MapObjectTriggerSpawnObjectEvent != null)
                MapObjectTriggerSpawnObjectEvent(sender, e);
        }

        private void mapObject_MapObjectTriggerSpawnEnemyEvent(object sender, MapObjectTriggerSpawnEnemyEventArgs e)
        {
            if (MapObjectTriggerSpawnEnemyEvent != null)
                MapObjectTriggerSpawnEnemyEvent(sender, e);
        }

        private void mapObject_MapObjectTriggerSpawnItemEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            if (MapObjectTriggerSpawnItemEvent != null)
                MapObjectTriggerSpawnItemEvent(sender, e);
        }

        /// <summary>
        /// Assign positions to all tiles that make up this map. Positions are based on its index in the layer, its width/height, and the position that this map occupies in the 
        /// overall MapCollection structure. Maps make up a larger grid of maps to form a level.
        /// Also assign a collidable property to the tile if its property marks it as collidable.
        /// </summary>
        private void SetupTiles()
        {
            FocusPosition = new Vector2((Resolution.VirtualViewport.Width / 2) + offsetX, (Resolution.VirtualViewport.Height / 2) + offsetY);

            // assign position values to each tile
            foreach (Layer layer in Layers)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        Tile t = layer.Tiles[y * layer.Width + x];
                        Vector2 position = new Vector2((x * TileWidth) + offsetX, (y * TileHeight) + offsetY);
                        t.Position = position;
                    }
                }
            }
        }

        /// <summary>Extract any spawn points out of our map object layers
        /// </summary>
        private void SetupSpawnPoints()
        {
            enemySpawnPoints = new List<MapObject>();
            bulletSpawnPoints = new List<MapObject>();
            playerSpawnPoints = new List<MapObject>();

            // go through all map object layers and extract the spawn points
            foreach (MapObjectLayer mapObjectLayer in MapObjectLayers)
            {
                foreach (MapObject mapObject in mapObjectLayer.MapObjects)
                {
                    // todo: don't hardcode this string
                    if (mapObject.Type == "EnemySpawn")
                        enemySpawnPoints.Add(mapObject);
                    else if (mapObject.Type == "BulletSpawn")
                        bulletSpawnPoints.Add(mapObject);
                    else if (mapObject.Type == "PlayerSpawn")
                        playerSpawnPoints.Add(mapObject);
                }
            }
        }

        private void SetupBulletSpawner(ContentManager content, PlayerEntity player)
        {
            //todo: think of a better way to handle this, maybe move to bulletmanager?
            if (!String.IsNullOrEmpty(BulletSpawner))
            {
                EntityData bulletEntityData = content.Load<EntityData>("Entities/BulletOrangeCircle");
                bulletSheet = content.Load<SpriteSheet>(bulletEntityData.SpriteSheet);
                List<int> spriteIds = new List<int>();
                spriteIds.Add(bulletEntityData.SpriteIdFacingDown);
                bulletSheet.Activate(spriteIds);
                bulletSprite = bulletSheet.GetSprite(1); //todo: don't hardcode
                bulletSprite.ScaleFactor = .5f;

                parser.ParseXML(String.Format(@"Content\BulletPatterns\{0}", BulletSpawner));
                BulletMLManager.Init(new MyBulletFunctions(player));

                foreach (MapObject bulletSpawn in bulletSpawnPoints)
                {
                    //todo: don't use an index
                    Vector2 bulletPosition = GetBulletSpawnLocation(bulletSpawnPoints.IndexOf(bulletSpawn));
                    bulletMover = BulletMoverManager.CreateBulletMover(bulletSheet, bulletSprite, bulletPosition, parser.tree);
                }
            }
        }

        /// <summary>
        /// Get a random position within the bounds of the spawn point
        /// </summary>
        /// <param name="spawnPointName"></param>
        /// <returns></returns>
        private Vector2 GetRandomEnemySpawnLocation(int spawnPointId)
        {
            MapObject spawnPoint = enemySpawnPoints[spawnPointId];

            int randomPositionX = random.Next(spawnPoint.Bounds.X, spawnPoint.Bounds.X + spawnPoint.Bounds.Width);
            int randomPositionY = random.Next(spawnPoint.Bounds.Y, spawnPoint.Bounds.Y + spawnPoint.Bounds.Height);

            return new Vector2(randomPositionX + offsetX, randomPositionY + offsetY);
        }

        private Vector2 GetRandomBulletSpawnLocation()
        {
            int randomSpawnPointId = random.Next(0, bulletSpawnPoints.Count);

            return GetBulletSpawnLocation(randomSpawnPointId);
        }

        private Vector2 GetBulletSpawnLocation(int spawnPointId)
        {
            float x = bulletSpawnPoints[spawnPointId].Bounds.X + (bulletSpawnPoints[spawnPointId].Bounds.Width / 2) + offsetX;
            float y = bulletSpawnPoints[spawnPointId].Bounds.Y + (bulletSpawnPoints[spawnPointId].Bounds.Height / 2) + offsetY;

            //todo: don't hardcode this index
            return new Vector2(x, y);
        }

        public Vector2 GetPlayerSpawnPoint()
        {
            //todo: don't hardcode this index
            float x = playerSpawnPoints[0].Bounds.X + (playerSpawnPoints[0].Bounds.Width / 2) + offsetX;
            float y = playerSpawnPoints[0].Bounds.Y + (playerSpawnPoints[0].Bounds.Height / 2) + offsetY;

            //todo: don't hardcode this index
            return new Vector2(x, y);
        }
    }

    public class MapObjectTriggerSpawnEntityEventArgs : EventArgs
    {
        public int EntityId { get; private set; }
        public Vector2 Position { get; private set; }

        public MapObjectTriggerSpawnEntityEventArgs(int entityId, Vector2 position)
        {
            EntityId = entityId;
            Position = position;
        }
    }

    public class MapObjectTriggerSpawnEnemyEventArgs : EventArgs
    {
        public int EnemyId { get; private set; }
        public Vector2 Position { get; private set; }
        public List<Vector2> Path { get; private set; }

        public MapObjectTriggerSpawnEnemyEventArgs(int enemyId, Vector2 position, List<Vector2> path)
        {
            EnemyId = enemyId;
            Position = position;
            Path = path;
        }
    }
}
