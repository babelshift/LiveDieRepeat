﻿using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;

namespace LiveDieRepeat.Screens
{
	/// <summary>
	/// A screen is a single layer that has update and draw logic, and which
	/// can be combined with other layers to build up a complex menu system.
	/// For instance the main menu, the options menu, the "are you sure you
	/// want to quit" message box, and the main game itself are all implemented
	/// as screens.
	/// </summary>
	public abstract class Screen : IInputtable, IDisposable
	{
		private TimeSpan transitionOnTime = TimeSpan.Zero;
		private TimeSpan transitionOffTime = TimeSpan.Zero;
		private float transitionPosition = 1;
		private ScreenState screenState = ScreenState.TransitionOn;
		private bool otherWindowHasFocus;

		private readonly List<Control> controls = new List<Control>();

		protected IList<Control> Controls { get { return controls; } }

		protected ContentManager ContentManager { get; private set; }

		#region Properties

		/// <summary>
		/// Normally when one screen is brought up over the top of another,
		/// the first screen will transition off to make room for the new
		/// one. This property indicates whether the screen is only a small
		/// popup, in which case screens underneath it do not need to bother
		/// transitioning off.
		/// </summary>
		public bool IsPopup { get; protected set; }

		/// <summary>
		/// Indicates how long the screen takes to
		/// transition on when it is activated.
		/// </summary>
		public TimeSpan TransitionOnTime
		{
			get { return transitionOnTime; }
			protected set { transitionOnTime = value; }
		}

		/// <summary>
		/// Indicates how long the screen takes to
		/// transition off when it is deactivated.
		/// </summary>
		public TimeSpan TransitionOffTime
		{
			get { return transitionOffTime; }
			protected set { transitionOffTime = value; }
		}

		/// <summary>
		/// Gets the current position of the screen transition, ranging
		/// from zero (fully active, no transition) to one (transitioned
		/// fully off to nothing).
		/// </summary>
		public float TransitionPosition
		{
			get { return transitionPosition; }
			protected set { transitionPosition = value; }
		}

		/// <summary>
		/// Gets the current alpha of the screen transition, ranging
		/// from 1 (fully active, no transition) to 0 (transitioned
		/// fully off to nothing).
		/// </summary>
		public float TransitionAlpha
		{
			get { return 1f - TransitionPosition; }
		}

		/// <summary>
		/// Gets the current screen transition state.
		/// </summary>
		public ScreenState ScreenState
		{
			get { return screenState; }
			protected set { screenState = value; }
		}

		/// <summary>
		/// There are two possible reasons why a screen might be transitioning
		/// off. It could be temporarily going away to make room for another
		/// screen that is on top of it, or it could be going away for good.
		/// This property indicates whether the screen is exiting for real:
		/// if set, the screen will automatically remove itself as soon as the
		/// transition finishes.
		/// </summary>
		public bool IsExiting { get; protected internal set; }

		/// <summary>
		/// Checks whether this screen is active and can respond to user input.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return !otherWindowHasFocus &&
				(screenState == ScreenState.TransitionOn ||
				screenState == ScreenState.Active);
			}
		}

		/// <summary>
		/// Gets the manager that this screen belongs to.
		/// </summary>
		public ScreenManager ScreenManager { get; internal set; }

		/// <summary>
		/// Gets whether or not this screen is serializable. If this is true,
		/// the screen will be recorded into the screen manager's state and
		/// its Serialize and Deserialize methods will be called as appropriate.
		/// If this is false, the screen will be ignored during serialization.
		/// By default, all screens are assumed to be serializable.
		/// </summary>
		public bool IsSerializable { get; private set; }

		#endregion Properties

		public Screen(ContentManager contentManager)
		{
			IsPopup = false;
			ContentManager = contentManager;
		}

		/// <summary>
		/// Activates the screen. Called when the screen is added to the screen manager or if the game resumes
		/// from being paused or tombstoned.
		/// </summary>
		/// <param name="renderer"></param>
		public virtual void Activate(Renderer renderer)
		{
		}

		/// <summary>
		/// Deactivates the screen. Called when the game is being deactivated due to pausing or tombstoning.
		/// </summary>
		public virtual void Deactivate()
		{
		}

		/// <summary>
		/// Unload contentManager for the screen. Called when the screen is removed from the screen manager.
		/// </summary>
		public virtual void Unload()
		{
			Dispose();
		}

		/// <summary>
		/// Allows the screen to run logic, such as updating the transition position.
		/// Unlike HandleInput, this method is called regardless of whether the screen
		/// is active, hidden, or in the middle of a transition.
		/// </summary>
		public virtual void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			this.otherWindowHasFocus = otherWindowHasFocus;

			if (IsExiting)
			{
				// If the screen is going away to die, it should transition off.
				screenState = ScreenState.TransitionOff;

				if (!UpdateTransition(gameTime, transitionOffTime, 1))
				{
					// When the transition finishes, remove the screen.
					ScreenManager.RemoveScreen(this);
				}
			}
			else if (coveredByOtherScreen)
			{
				// If the screen is covered by another, it should transition off.
				// Being covered by another screen essentially means we want to replace this screen with another
				// For example, going from main menu to in game would result in the "in game" screen covering the "main menu" screen
				if (UpdateTransition(gameTime, transitionOffTime, 1))
				{
					// Still busy transitioning.
					screenState = ScreenState.TransitionOff;
				}
				else
				{
					// Transition finished!
					screenState = ScreenState.Hidden;
				}
			}
			else
			{
				// Otherwise the screen should transition on and become active.
				if (UpdateTransition(gameTime, transitionOnTime, -1))
				{
					// Still busy transitioning.
					screenState = ScreenState.TransitionOn;
				}
				else
				{
					// Transition finished!
					screenState = ScreenState.Active;
				}
			}
		}

		/// <summary>
		/// Helper for updating the screen transition position.
		/// </summary>
		private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
		{
			// How much should we move by?
			float transitionDelta;

			if (time == TimeSpan.Zero)
				transitionDelta = 1;
			else
				transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

			// Update the transition position.
			transitionPosition += transitionDelta * direction;

			// Did we reach the end of the transition?
			if (((direction < 0) && (transitionPosition <= 0)) ||
				((direction > 0) && (transitionPosition >= 1)))
			{
				//transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
				return false;
			}

			// Otherwise we are still busy transitioning.
			return true;
		}

		#region Input Events

		public virtual void HandleTextInputtingEvent(object sender, TextInputEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleTextInputtingEvent(sender, e);
			}
		}

		public virtual void HandleMouseButtonReleasedEvent(object sender, MouseButtonEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleMouseButtonReleasedEvent(sender, e);
			}
		}

		public virtual void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleMouseButtonPressedEvent(sender, e);
			}
		}

		public virtual void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleMouseMovingEvent(sender, e);
			}
		}

		public virtual void HandleKeyPressedEvent(object sender, KeyboardEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleKeyPressedEvent(sender, e);
			}
		}

		public virtual void HandleKeyReleasedEvent(object sender, KeyboardEventArgs e)
		{
			foreach (var control in controls)
			{
				control.HandleKeyReleasedEvent(sender, e);
			}
		}

		#endregion Input Events

		/// <summary>
		/// Allows the screen to handle user input. Unlike Update, this method
		/// is only called when the screen is active, and not when some other
		/// screen has taken the focus.
		/// </summary>
		public virtual void HandleInput(GameTime gameTime, bool isMouseInsideWindowBounds)
		{
		}

		/// <summary>
		/// This is called when the screen should draw itself.
		/// </summary>
		public virtual void Draw(GameTime gameTime, Renderer renderer)
		{
			foreach (var control in controls)
			{
				control.Draw(gameTime, renderer);
			}
		}

		/// <summary>
		/// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
		/// instantly kills the screen, this method respects the transition timings
		/// and will give the screen a chance to gradually transition off.
		/// </summary>
		public void ExitScreen()
		{
			if (TransitionOffTime == TimeSpan.Zero)
			{
				// If the screen has a zero transition time, remove it immediately.
				ScreenManager.RemoveScreen(this);
			}
			else
			{
				// Otherwise flag that it should transition off and then exit.
				IsExiting = true;
			}
		}

		protected void AddControl(Control control)
		{
			controls.Add(control);
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			foreach (var control in controls)
			{
				control.Dispose();
			}
		}
	}
}