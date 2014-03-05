using LiveDieRepeat.Content;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.UserInterface
{
	public static class ControlFactory
	{
		public static Button CreateButton(ContentManager contentManager, string textureFrameKey, string textureFrameHoveredKey = "", string textureFrameSelectedKey = "")
		{
			if (contentManager == null) throw new ArgumentNullException("content");

			if (String.IsNullOrEmpty(textureFrameKey))
				throw new ArgumentNullException("textureFrameKey");

			Button button = new Button();
			button.TextureFrame = contentManager.GetTexture(textureFrameKey);

			if (!String.IsNullOrEmpty(textureFrameHoveredKey))
				button.TextureFrameHovered = contentManager.GetTexture(textureFrameHoveredKey);

			if (!String.IsNullOrEmpty(textureFrameSelectedKey))
				button.TextureFrameSelected = contentManager.GetTexture(textureFrameSelectedKey);

			return button;
		}

		public static Icon CreateIcon(ContentManager contentManager, string iconKey)
		{
			if (contentManager == null) throw new ArgumentNullException("content");

			if (String.IsNullOrEmpty(iconKey))
				throw new ArgumentNullException("iconKey");

			Icon icon = new Icon(contentManager.GetTexture(iconKey));
			return icon;
		}

		public static Label CreateLabel(ContentManager contentManager, string fontPath, int fontSize, Color fontColor, string text, int wrapLength = 0)
		{
			if (contentManager == null) throw new ArgumentNullException("content");

			if (String.IsNullOrEmpty(fontPath))
				throw new ArgumentNullException("fontPath");

			if (String.IsNullOrEmpty(text))
				throw new ArgumentNullException("text");

			if (fontSize <= 0)
				throw new ArgumentOutOfRangeException("fontSize");

			Label label = new Label();
			label.TrueTypeText = contentManager.GetText(fontPath, fontSize, fontColor, text);

			return label;
		}
	}
}