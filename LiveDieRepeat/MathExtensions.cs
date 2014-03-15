using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public static class MathExtensions
	{
		public static float ToRadians(double angle)
		{
			return (float)(angle * (Math.PI / 180));
		}
	}
}
