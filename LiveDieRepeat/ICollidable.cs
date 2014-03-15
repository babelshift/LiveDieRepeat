using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public interface ICollidable
	{
		void ResolveCollision(ICollidable c);
		Rectangle CollisionBox { get; }
		Vector Position { get; }
		IReadOnlyList<ICollidable> CollidableComponents { get; }
	}
}
