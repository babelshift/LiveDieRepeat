using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using SharpDL.Input;
using System;

namespace LiveDieRepeat.UserInterface
{
	public abstract class Control : IDisposable
	{
		public bool IsFocused { get; private set; }

		public Guid ID { get; private set; }

		public bool Visible { get; set; }

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
			}
		}

		/// <summary>
		/// The position of the control in a 2D space. Objects inheriting from this class must explicitly override this property if
		/// special positioning of child or containing controls is required.
		/// </summary>
		public virtual Vector Position { get; set; }

		/// <summary>
		/// The width of the control as defined in whole pixels. Objects inheriting from this class must explicitly set this value
		/// based on its own conditions.
		/// </summary>
		public int Width { get; protected set; }

		/// <summary>
		/// The height of the control as defined in whole pixels. Objects inheriting from this class must explicitly set this value
		/// based on its own conditions.
		/// </summary>
		public int Height { get; protected set; }

		public bool IsHovered { get; private set; }

		public bool IsClicked { get; private set; }

		public event EventHandler Hovered;

		public event EventHandler Clicked;

		public event EventHandler Focused;

		public event EventHandler Focusing;

		public event EventHandler Blurring;

		public event EventHandler Blurred;

		public event EventHandler Released;

		protected Control()
		{
			ID = Guid.NewGuid();
			Visible = true;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (Visible)
			{
				if (IsHovered)
					OnHovered(EventArgs.Empty);
			}
		}

		public abstract void Draw(GameTime gameTime, Renderer renderer);

		public virtual void HandleMouseMovingEvent(object sender, MouseMotionEventArgs e)
		{
			if (Visible)
			{
				IsHovered = GetHovered(e.RelativeToWindowX, e.RelativeToWindowY);
			}
		}

		private bool GetHovered(int x, int y)
		{
			return Bounds.Contains(new Vector(x, y));
		}

		public virtual void HandleMouseButtonPressedEvent(object sender, MouseButtonEventArgs e)
		{
			if (Visible)
			{
				IsHovered = GetHovered(e.RelativeToWindowX, e.RelativeToWindowY);

				IsClicked = GetClicked(e);

				if (IsClicked)
					OnClicked(EventArgs.Empty);
			}
		}

		public virtual void HandleMouseButtonReleasedEvent(object sender, MouseButtonEventArgs e)
		{
			if (Visible)
			{
				GetReleased(e);
			}
		}

		public virtual void HandleTextInput(string text)
		{
			if (Visible)
			{
			}
		}

		public virtual void HandleKeyPressed(KeyInformation key)
		{
			if (Visible)
			{
			}
		}

		public virtual void Focus()
		{
			if (IsFocused) return;

			if (Focusing != null)
				Focusing(this, EventArgs.Empty);

			IsFocused = true;

			if (Focused != null)
				Focused(this, EventArgs.Empty);
		}

		public virtual void Blur()
		{
			if (Blurring != null)
				Blurring(this, EventArgs.Empty);

			IsFocused = false;

			if (Blurred != null)
				Blurred(this, EventArgs.Empty);
		}

		private bool GetClicked(SharpDL.Events.MouseButtonEventArgs e)
		{
			if (!IsHovered) return false;

			return e.MouseButton == MouseButtonCode.Left;
		}

		private void GetReleased(MouseButtonEventArgs e)
		{
			if (IsClicked && e.State == MouseButtonState.Released && e.MouseButton == MouseButtonCode.Left)
			{
				OnReleased(EventArgs.Empty);
			}
		}

		private void OnReleased(EventArgs e)
		{
			IsClicked = false;
			if (Released != null)
				Released(this, e);
		}

		private void OnClicked(EventArgs e)
		{
			if (Clicked != null)
				Clicked(this, e);
		}

		private void OnHovered(EventArgs e)
		{
			if (Hovered != null)
				Hovered(this, e);
		}

		public abstract void Dispose();
	}
}