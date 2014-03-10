using SharpDL.Events;
using SharpDL.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.UserInterface
{
	public interface IInputtable
	{
		void HandleKeyPressedEvent(object sender, KeyboardEventArgs e);
		void HandleKeyReleasedEvent(object sender, KeyboardEventArgs e);
		void HandleTextInputtingEvent(object sender, TextInputEventArgs e);
		void HandleMouseButtonReleasedEvent(object sender, MouseButtonEventArgs e);
		void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e);
		void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e);
	}
}
