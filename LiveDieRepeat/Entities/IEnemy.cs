using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LiveDieRepeat.Engine;

namespace LiveDieRepeat.Entities
{
    public interface IEnemy : ICollidable
    {
        void Update(GameTime gameTime);
        void Update(GameTime gameTime, Vector2 playerPosition);
        void Draw(SpriteBatch spriteBatch, Texture2D rectTexture);
        void SaveCollisionBox();
        bool IsDead { get; }
        void SetPath(IEnumerable<Vector2> path);
        new Vector2 Position { get; set; }
        bool IsChasingPlayer { get; set; }
        event EventHandler<EnemyKilledEventArgs> EnemyKilledEvent;
        event EventHandler<EntityDeathEventArgs> EntityDeathEvent;
    }
}
