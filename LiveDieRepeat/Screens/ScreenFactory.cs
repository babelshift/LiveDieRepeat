using System;

namespace LiveDieRepeat.Screens
{
	public class ScreenFactory
	{
		public Screen CreateScreen(Type screenType)
		{
			// All of our screens have empty constructors so we can just use Activator
			return Activator.CreateInstance(screenType) as Screen;
		}
	}
}