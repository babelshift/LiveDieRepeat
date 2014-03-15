using SharpDL;
using SharpDL.Graphics;
using System;

namespace LiveDieRepeat
{
	public abstract class Agent : IDisposable
	{
		private Vector position;

		public bool IsDead { get; private set; }

		public virtual Vector Position
		{
			get { return position; }
			protected set { position = value; }
		}

		public void TeleportTo(Vector position)
		{
			Position = position;
		}

		public void Die()
		{
			IsDead = true;
		}

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(GameTime gameTime, Renderer renderer);

		public abstract void Dispose();
	}
}