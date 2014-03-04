using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;

namespace LiveDieRepeat.Engine
{
    /// <summary>
    /// Stores a collection of Map objects to form a Level in a grid like fashion. There is only ever a single CurrentMap that is used to indicate
    /// which Map the player is currently on. 
    /// 
    /// For example: a simple Level consisting of two Maps would be structures as below.
    /// [0, 0] = Map 1
    /// [1, 0] = Map 2
    /// 
    /// Moving right from Map 1 would take you to Map 2 (increase in X coordinate)
    /// Moving left from Map 2 would take you to Map 1 (decrease in X coordinate)
    /// </summary>
    public class MapCollection
    {
        // Deserialized list of Maps as loaded from a Level XML file consisting of which TMX files to load and the position of each Map in the overall Level grid
        public List<Map> Maps = new List<Map>();

        // Determines which direction in the Map collection to shift to
        private enum ShiftDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Each Map can have 0 to 4 adajacent Maps (above, below, to the left, and to the right)
        /// For example, a Level with the following Map grid:
        /// [0,0]
        /// [1,0]
        /// [1,1]
        /// 
        /// [0,0] is adjacent to [1,0]
        /// [1,0] is adjacent to [0,0] and [1,1]
        /// [1,1] is adjacent to [1,0]
        /// 
        /// This property can be used to get references to the adjacent maps so collision detection and rendering can occur.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Map> CurrentAndAdjacentMaps = new List<Map>();

        // The current Map that the player is interacting with in the Level
        public Map CurrentMap { get; private set; }

        public event EventHandler<EnemySpawnedEventArgs> EnemySpawnedEvent;
        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnObjectEvent;
        public event EventHandler<MapObjectTriggerSpawnEntityEventArgs> MapObjectTriggerSpawnItemEvent;
        public event EventHandler<MapObjectTriggerSpawnEnemyEventArgs> MapObjectTriggerSpawnEnemyEvent;

        /// <summary>
        /// On initialization, every Map in the Level should be initialized (positions of textures will be established)
        /// </summary>
        public void Initialize(ContentManager content, PlayerEntity player)
        {
            foreach (Map map in Maps)
            {
                map.Initialize(content, player);
                map.EnemySpawnedEvent += new EventHandler<EnemySpawnedEventArgs>(map_EnemySpawnedEvent);
                map.MapObjectTriggerSpawnEnemyEvent += new EventHandler<MapObjectTriggerSpawnEnemyEventArgs>(map_MapObjectTriggerSpawnEnemyEvent);
                map.MapObjectTriggerSpawnItemEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(map_MapObjectTriggerSpawnItemEvent);
                map.MapObjectTriggerSpawnObjectEvent += new EventHandler<MapObjectTriggerSpawnEntityEventArgs>(map_MapObjectTriggerSpawnObjectEvent);
            }

            // Set the current Map map to the Map at [0,0]
            CurrentMap = Maps.Find(m => (int)m.GridPosition.Y == 0 && (int)m.GridPosition.X == 0);

            // Establish all Maps adjacent to the current Map
            DetermineAdjacentMaps();
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            CurrentMap.Update(gameTime, camera);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (Map map in CurrentAndAdjacentMaps)
                map.Draw(spriteBatch, camera);
        }

        public void ShiftRight()
        {
            Shift(ShiftDirection.Right);
        }

        public void ShiftLeft()
        {
            Shift(ShiftDirection.Left);
        }

        public void ShiftDown()
        {
            Shift(ShiftDirection.Down);
        }

        public void ShiftUp()
        {
            Shift(ShiftDirection.Up);
        }

        /// <summary>
        /// Shifting in the Level will establish a new current Map and new adjacent Maps. If the shifting fails because there is no Map at the shifted location, nothing happens.
        /// </summary>
        /// <param name="shiftDirection"></param>
        /// <returns></returns>
        private void Shift(ShiftDirection shiftDirection)
        {
            Map previousMap = CurrentMap;
            int currentMapIndexX = (int)CurrentMap.GridPosition.X;
            int currentMapIndexY = (int)CurrentMap.GridPosition.Y;

            // shift the index in the grid based on the direction
            if (shiftDirection == ShiftDirection.Right)
                currentMapIndexX++;
            else if (shiftDirection == ShiftDirection.Left)
                currentMapIndexX--;
            else if (shiftDirection == ShiftDirection.Up)
                currentMapIndexY--;
            else if (shiftDirection == ShiftDirection.Down)
                currentMapIndexY++;

            // set the current map to whatever map is at the new position in the grid 
            CurrentMap = Maps.Find(m => (int)m.GridPosition.Y == (int)currentMapIndexY && (int)m.GridPosition.X == (int)currentMapIndexX);

            // if a map is found, determine new adjacent maps, otherwise revert
            if (CurrentMap != null)
            {
                CurrentMap.StartMapEvents();
                DetermineAdjacentMaps();
            }
            else
                CurrentMap = previousMap;
        }

        /// <summary>
        /// Determine adjacent Maps to the current Map by checking for all Maps that are a single index away in each direction
        /// </summary>
        private void DetermineAdjacentMaps()
        {
            int currentMapIndexX = (int)CurrentMap.GridPosition.X;
            int currentMapIndexY = (int)CurrentMap.GridPosition.Y;

            CurrentAndAdjacentMaps.Clear();
            CurrentAndAdjacentMaps = Maps.FindAll(
                m => ((int)m.GridPosition.X == (currentMapIndexX - 1) || (int)m.GridPosition.X == (currentMapIndexX + 1))
                    ||
                    ((int)m.GridPosition.Y == (currentMapIndexY + 1) || (int)m.GridPosition.Y == (currentMapIndexY - 1))
                );

            CurrentAndAdjacentMaps.Add(CurrentMap);
        }

        private void map_MapObjectTriggerSpawnItemEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            if (MapObjectTriggerSpawnItemEvent != null)
                MapObjectTriggerSpawnItemEvent(sender, e);
        }

        private void map_MapObjectTriggerSpawnEnemyEvent(object sender, MapObjectTriggerSpawnEnemyEventArgs e)
        {
            if (MapObjectTriggerSpawnEnemyEvent != null)
                MapObjectTriggerSpawnEnemyEvent(sender, e);
        }

        private void map_MapObjectTriggerSpawnObjectEvent(object sender, MapObjectTriggerSpawnEntityEventArgs e)
        {
            if (MapObjectTriggerSpawnObjectEvent != null)
                MapObjectTriggerSpawnObjectEvent(sender, e);
        }

        private void map_EnemySpawnedEvent(object sender, EnemySpawnedEventArgs e)
        {
            if (EnemySpawnedEvent != null)
                EnemySpawnedEvent(sender, e);
        }
    }
}
