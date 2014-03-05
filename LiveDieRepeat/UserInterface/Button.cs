using LiveDieRepeat.Content;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.UserInterface
{
	public enum ButtonType
	{
		TextOnly,
		IconOnly,
		IconAndText,
		FrameOnly
	}

	public class Button : Control
	{
		#region Members

		private Texture textureFrame;
		private ButtonType buttonType;

		#endregion Members

		#region Properties

		public Icon Icon { get; set; }

		public Icon IconHovered { get; set; }

		public Icon IconSelected { get; set; }

		public Texture TextureFrame
		{
			get { return textureFrame; }
			set
			{
				textureFrame = value;
				Width = textureFrame.Width;
				Height = textureFrame.Height;
			}
		}

		public Texture TextureFrameHovered { get; set; }

		public Texture TextureFrameSelected { get; set; }

		public Label Label { get; set; }

		public string Text { get { return Label.Text; } set { Label.Text = value; } }

		public bool IsSelected { get; set; }

		public ButtonType ButtonType
		{
			get { return buttonType; }
			set
			{
				buttonType = value;
				if (buttonType == ButtonType.FrameOnly)
				{

				}
				else if (buttonType == ButtonType.TextOnly)
				{
					if (Label != null)
						Label.Position = new Vector(base.Position.X + TextureFrame.Width / 2 - Label.Width / 2, base.Position.Y + TextureFrame.Height / 2 - Label.Height / 2);
				}
				else if (buttonType == ButtonType.IconOnly)
				{
					if (Icon != null)
						Icon.Position = new Vector(base.Position.X + (TextureFrame.Width / 2 - Icon.Width / 2), base.Position.Y + (TextureFrame.Height / 2 - Icon.Height / 2));
				}
				else if (buttonType == ButtonType.IconAndText)
				{
					if (Icon != null)
						Icon.Position = new Vector(base.Position.X + 5, base.Position.Y + (TextureFrame.Height / 2 - Icon.Height / 2));

					if (Label != null)
						Label.Position = new Vector(Icon.Position.X + Icon.Width + 3, base.Position.Y + (TextureFrame.Height / 2 - Label.Height / 2));
				}
			}
		}

		public override Vector Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;

				if (buttonType == ButtonType.TextOnly)
				{
					if (Label != null)
						Label.Position = new Vector(base.Position.X + (TextureFrame.Width / 2 - Label.Width / 2), base.Position.Y + (TextureFrame.Height / 2 - Label.Height / 2));
				}
				else if (buttonType == ButtonType.IconOnly)
				{
					if (Icon != null)
						Icon.Position = new Vector(base.Position.X + (TextureFrame.Width / 2 - Icon.Width / 2), base.Position.Y + (TextureFrame.Height / 2 - Icon.Height / 2));
				}
				else if (buttonType == ButtonType.IconAndText)
				{
					if (Icon != null)
						Icon.Position = new Vector(base.Position.X + 5, base.Position.Y + (TextureFrame.Height / 2 - Icon.Height / 2));

					if (Label != null)
						Label.Position = new Vector(Icon.Position.X + Icon.Width + 3, base.Position.Y + (TextureFrame.Height / 2 - Label.Height / 2));
				}
			}
		}

		public void EnableLabelShadow(ContentManager content, int shadowOffsetX, int shadowOffsetY)
		{
			if (Label != null)
				Label.EnableShadow(content, shadowOffsetX, shadowOffsetY);
		}

		#endregion Properties

		#region Constructors

		public Button()
		{
			Visible = true;
			IsSelected = false;
		}

		#endregion Constructors

		#region Game Loop

		public override void Update(GameTime gameTime)
		{
			if (!Visible)
				return;

			base.Update(gameTime);

			if (Icon != null)
				Icon.Update(gameTime);

			if (Label != null)
				Label.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			if (!Visible)
				return;

			if (IsHovered)
			{
				if (TextureFrameHovered != null)
					TextureFrameHovered.Draw(Position.X, Position.Y);
			}

			if (IsClicked || IsSelected)
			{
				if (TextureFrameSelected != null)
					TextureFrameSelected.Draw(Position.X, Position.Y);
			}

			if (!IsHovered && !IsClicked && !IsSelected)
				TextureFrame.Draw(Position.X, Position.Y);

			if (Icon != null)
				Icon.Draw(gameTime, renderer);

			if (Label != null)
				Label.Draw(gameTime, renderer);
		}

		#endregion Game Loop

		#region Behaviors

		public void ToggleOn()
		{
			IsSelected = true;
		}

		public void ToggleOff()
		{
			IsSelected = false;
		}

		#endregion Behaviors

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (Icon != null)
				Icon.Dispose();
			if (IconHovered != null)
				IconHovered.Dispose();
			if (IconSelected != null)
				IconSelected.Dispose();
			if (TextureFrameHovered != null)
				TextureFrameHovered.Dispose();
			if (TextureFrameSelected != null)
				TextureFrameSelected.Dispose();
			if (Label != null)
				Label.Dispose();
		}
	}
}