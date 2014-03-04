using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Engine
{
    public interface ITriggerSubscriber
    {
        int Id { get; }
        void PerformAction(TriggerAction triggerAction);
    }

    public class Trigger
    {
        public string Type;
        public List<TriggerAction> Actions = new List<TriggerAction>();
    }

    public class TriggerAction
    {
        public string Type;
        public int EntityId;
        public List<Vector2> Path = new List<Vector2>();
        public List<int> SubscriberIds = new List<int>();
    }

    /// <summary>
    /// Data used to define what type of enemy to spawn, how many to spawn, and the attributes to give it. These enemies are added to its respective event wave enemy list.
    /// </summary>
    public class MapEventWaveEnemy
    {
        public int Type;
        public int Count;
        public bool HasWeapon;
        public List<Vector2> Path = new List<Vector2>();
    }

    /// <summary>
    /// Represents a single wave of enemies in an event. Each wave contains one to many groups of enemies to spawn (MapEventWaveEnemy). Enemies are spawned serially with a defined
    /// time between each spawn.
    /// </summary>
    public class MapEventWave
    {
        #region XML Elements

        public int MillisecondsBetweenEnemies;
        public List<MapEventWaveEnemy> Enemies = new List<MapEventWaveEnemy>();

        #endregion

        [ContentSerializerIgnore]
        public bool IsActive
        {
            get
            {
                if (currentState == State.Active)
                    return true;
                else
                    return false;
            }
        }

        private State currentState = State.InActive;
        private Queue<IEnemy> spawnQueue = new Queue<IEnemy>();
        private double millisecondsSinceLastSpawn = 0;

        private enum State
        {
            InActive,
            Active,
            Completed
        }

        #region Event Handlers

        public event EventHandler<EnemySpawnedEventArgs> EnemySpawnedEvent;
        public event EventHandler<EventArgs> EndedEvent;

        #endregion

        public void Start()
        {
            foreach (MapEventWaveEnemy mapEventWaveEnemy in Enemies)
            {
                for (int i = 0; i < mapEventWaveEnemy.Count; i++)
                {
                    // the event enemy type relates to an enemy id that is used to spawn the correct enemy class
                    IEnemy enemyEntity = Factory<IEnemy>.Create(mapEventWaveEnemy.Type);
                    enemyEntity.SetPath(mapEventWaveEnemy.Path);
                    spawnQueue.Enqueue(enemyEntity);
                }
            }

            currentState = State.Active;
        }

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                millisecondsSinceLastSpawn += gameTime.ElapsedGameTime.TotalMilliseconds;

                // game has waited long enough to spawn the next enemy
                if (millisecondsSinceLastSpawn > MillisecondsBetweenEnemies)
                {
                    millisecondsSinceLastSpawn = 0;

                    if (spawnQueue.Count != 0)
                    {
                        IEnemy enemy = spawnQueue.Dequeue();

                        // notify the owning map event that this wave has spawned an enemy
                        OnEnemySpawned(this, new EnemySpawnedEventArgs(enemy));
                    }
                    else
                    {
                        // notify the owning map event that this wave has ended
                        OnEndedEvent(this, EventArgs.Empty);
                    }
                }
            }
        }

        #region Events

        private void OnEnemySpawned(object sender, EnemySpawnedEventArgs e)
        {
            if (EnemySpawnedEvent != null)
                EnemySpawnedEvent(sender, e);
        }

        private void OnEndedEvent(object sender, EventArgs e)
        {
            currentState = State.Completed;

            if (EndedEvent != null)
                EndedEvent(sender, e);
        }

        #endregion
    }

    /// <summary>
    /// Represents a single map event that contains many event waves. Waves are executed serially with a defined time between each execution.
    /// </summary>
    public class MapEvent
    {
        #region XML Elements

        public int SecondsBetweenWaves;
        public List<MapEventWave> Waves = new List<MapEventWave>();
        public List<Trigger> Triggers = new List<Trigger>();

        #endregion

        [ContentSerializerIgnore]
        private List<ITriggerSubscriber> subscriberObjects = new List<ITriggerSubscriber>();

        [ContentSerializerIgnore]
        public MapEventWave CurrentWave { get; private set; }

        [ContentSerializerIgnore]
        public bool IsActive
        {
            get
            {
                if (currentState == State.Active)
                    return true;
                else
                    return false;
            }
        }

        private State currentState = State.InActive;
        private double secondsSincePreviousWaveComplete = 0;
        private Queue<MapEventWave> waveQueue = new Queue<MapEventWave>();

        private enum State
        {
            InActive,
            Active,
            Completed
        }

        #region Event Handlers

        public event EventHandler<EnemySpawnedEventArgs> EnemySpawnedEvent;
        public event EventHandler<MapEventEndedEventArgs> MapEventCompletedEvent;

        #endregion

        //private bool isComplete = false;
        //private bool isRunning = false;

        public void AddTriggerSubscriberObject(ITriggerSubscriber subscriberObject)
        {
            subscriberObjects.Add(subscriberObject);
        }

        public void Start(List<ITriggerSubscriber> subscriberObjects)
        {
            if (currentState == State.InActive)
            {
                currentState = State.Active;
                this.subscriberObjects = subscriberObjects;

                foreach (var eventWave in Waves)
                    waveQueue.Enqueue(eventWave);
            }
        }

        public void Update(GameTime gameTime)
        {
            // only update if the event is started
            if (IsActive)
            {
                // if there is no current wave, get one from the queue and start it
                if (CurrentWave == null)
                {
                    CurrentWave = waveQueue.Dequeue();
                    CurrentWave.EnemySpawnedEvent += new EventHandler<EnemySpawnedEventArgs>(CurrentWave_EnemySpawnedEvent);
                    CurrentWave.Start();
                }
                else
                {
                    // we have a current wave, if it's not active, get the next one
                    if (!CurrentWave.IsActive)
                    {
                        secondsSincePreviousWaveComplete += gameTime.ElapsedGameTime.TotalSeconds;

                        // game has waited long enough since the last wave completed to start the next wave
                        if (secondsSincePreviousWaveComplete > SecondsBetweenWaves)
                        {
                            secondsSincePreviousWaveComplete = 0;

                            // if there are more waves, activate them!
                            if (waveQueue.Count != 0)
                            {
                                CurrentWave = waveQueue.Dequeue();
                                CurrentWave.EnemySpawnedEvent += new EventHandler<EnemySpawnedEventArgs>(CurrentWave_EnemySpawnedEvent);
                                CurrentWave.Start();
                            }
                            else
                            {
                                OnMapEventCompletedEvent();
                            }
                        }
                    }
                    else
                    {
                        // we have an active wave, update it
                        CurrentWave.Update(gameTime);
                    }
                }
            }
        }

        #region Events

        private void CurrentWave_EnemySpawnedEvent(object sender, EnemySpawnedEventArgs e)
        {
            if (EnemySpawnedEvent != null)
                EnemySpawnedEvent(sender, e);
        }

        private void OnMapEventCompletedEvent()
        {
            Trigger onCompleteTrigger = Triggers.Find(t => t.Type == "OnComplete");

            // if a trigger is found, figure out the action and perform it
            if (onCompleteTrigger != null)
            {
                foreach (TriggerAction action in onCompleteTrigger.Actions)
                {
                    foreach (int subscriberId in action.SubscriberIds)
                    {
                        // modify subscriber properties to be opened (no more collision, change texture)
                        ITriggerSubscriber subscriberObject = subscriberObjects.Find(s => s.Id == subscriberId);
                        subscriberObject.PerformAction(action);
                    }
                }
            }

            currentState = State.Completed;

            // todo: don't hardcode event id
            if (MapEventCompletedEvent != null)
                MapEventCompletedEvent(this, new MapEventEndedEventArgs(0));
        }

        #endregion
    }

    #region Event Arguments

    public class EnemySpawnedEventArgs : EventArgs
    {
        public IEnemy SpawnedEnemy { get; private set; }

        public EnemySpawnedEventArgs(IEnemy enemy)
        {
            SpawnedEnemy = enemy;
        }
    }

    public class MapEventEndedEventArgs : EventArgs
    {
        public int EventId { get; private set; }

        public MapEventEndedEventArgs(int eventId)
        {
            EventId = eventId;
        }
    }

    #endregion
}
