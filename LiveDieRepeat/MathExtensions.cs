using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public static class MathExtensions
	{
		public static float ToRadians(double degrees)
		{
			return (float)(degrees * (Math.PI / 180));
		}

		public static float ToDegrees(double radians)
		{
			return (float)(radians * (180 / Math.PI));
		}
	}
}
