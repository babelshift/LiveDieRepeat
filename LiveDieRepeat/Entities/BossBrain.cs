using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Entities
{
    public class BossBrain : IEnemy
    {
        private IEnemy entityLeftEye;
        private IEnemy entityRightEye;
        private IEnemy entityBrain;
        private List<ICollidable> collidableComponents = new List<ICollidable>();
        private List<IEnemy> entityComponents = new List<IEnemy>();
        protected Queue<Vector2> path = new Queue<Vector2>();
        private Vector2 currentDestination;
        private Vector2 speed = new Vector2(100, 100);
        private float radiansOfRotation = 0f;

        public Vector2 Position { get; set; }
        public IList<ICollidable> CollidableComponents { get { return collidableComponents; } }
        public Rectangle CollisionBox { get { throw new NotImplementedException(); } }
        public bool IsChasingPlayer { get; set; }

        public event EventHandler<EnemyKilledEventArgs> EnemyKilledEvent;
        public event EventHandler<EntityDeathEventArgs> EntityDeathEvent;

        public bool IsDead
        {
            get
            {
                if (entityLeftEye.IsDead && entityRightEye.IsDead && entityBrain.IsDead)
                    return true;
                else
                    return false;
            }
        }

        protected Vector2 Direction
        {
            get
            {
                float xCoord = (float)Math.Cos((double)radiansOfRotation);
                float yCoord = (float)Math.Sin((double)radiansOfRotation);

                return new Vector2(xCoord, yCoord);
            }
        }

        public BossBrain(ContentManager content, GraphicsDevice graphicsDevice)
        {
            entityLeftEye = Factory<IEnemy>.Create((int)EntityId.Enemy.Eyeball);
            entityRightEye = Factory<IEnemy>.Create((int)EntityId.Enemy.Eyeball);
            entityBrain = Factory<IEnemy>.Create((int)EntityId.Enemy.Brain);

            entityLeftEye.IsChasingPlayer = false;
            entityRightEye.IsChasingPlayer = false;
            entityBrain.IsChasingPlayer = false;

            entityComponents.Add(entityBrain);
            entityComponents.Add(entityLeftEye);
            entityComponents.Add(entityRightEye);

            collidableComponents.Add(entityLeftEye);
            collidableComponents.Add(entityRightEye);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            Move(gameTime, playerPosition);

            if (entityLeftEye != null)
                entityLeftEye.Position = new Vector2(Position.X - 15, Position.Y + 18);
            if (entityRightEye != null)
                entityRightEye.Position = new Vector2(Position.X + 15, Position.Y + 18);
            if (entityBrain != null)
                entityBrain.Position = Position;

            //Position = GetNextCircularPosition(gameTime, playerPosition);

            foreach (var entityComponent in entityComponents.ToList())
            {
                entityComponent.Update(gameTime);
                RemoveIfDeadEntity(entityComponent);
            }

            // only brain left, make it collidable
            if (entityComponents.Count == 1 && entityComponents.Contains(entityBrain))
                collidableComponents.Add(entityBrain);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectTexture)
        {
            foreach (var entityComponent in entityComponents)
                entityComponent.Draw(spriteBatch, rectTexture);
        }

        public void SaveCollisionBox() { }

        public void ResolveCollision(ICollidable collidableEntity) { }

        private void RemoveIfDeadEntity(IEnemy e)
        {
            if (e.IsDead)
            {
                entityComponents.Remove(e);
                collidableComponents.Remove(e);
            }
        }

        public void SetPath(IEnumerable<Vector2> path)
        {
            foreach (Vector2 destination in path)
                this.path.Enqueue(destination);
        }

        private void Move(GameTime gameTime, Vector2 playerPosition)
        {
            // todo: any way to move this to enemy entity? it seems generic enough
            if (currentDestination == Vector2.Zero || entityBrain.CollisionBox.Contains((int)currentDestination.X, (int)currentDestination.Y))
            {
                if (path.Count > 0)
                    currentDestination = path.Dequeue();
                else
                    currentDestination = playerPosition;
            }

            radiansOfRotation = (float)Math.Atan2((double)(currentDestination.Y - Position.Y), (double)(currentDestination.X - Position.X));

            Vector2 previousPosition = Position;
            Vector2 direction = Direction;
            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2((float)(direction.X * speed.X * dt), (float)(direction.Y * speed.Y * dt));

            //SetMovingDirections(previousPosition);
        }

        private void OnEnemyKilledEvent(object sender, EnemyKilledEventArgs e)
        {
            if (EnemyKilledEvent != null)
                EnemyKilledEvent(sender, e);
        }

        private void OnEntityDeathEvent(object sender, EntityDeathEventArgs e)
        {
            if (EntityDeathEvent != null)
                EntityDeathEvent(sender, e);
        }
    }
}
