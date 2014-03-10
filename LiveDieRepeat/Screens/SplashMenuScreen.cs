using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Events;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.Screens
{
	public class SplashMenuScreen : Screen
	{
		bool redIncreasing = true;

		float tileOffset = 0;
		float redMod = 255;
		float greenMod = 255;
		float blueMod = 255;

		private Label labelVersion;
		private Icon iconLogo;
		private Button buttonStart;
		private Button buttonOptions;
		private Button buttonQuit;
		private Texture textureBackgroundTile;

		public SplashMenuScreen(ContentManager contentManager)
			: base(contentManager)
		{
		}

		public event EventHandler StartClicked;
		public event EventHandler OptionsClicked;
		public event EventHandler QuitClicked;

		public override void Activate(Renderer renderer)
		{
			base.Activate(renderer);

			buttonStart = ControlFactory.CreateButton(ContentManager, "ButtonSplash", "ButtonSplashHover");
			buttonStart.ButtonType = ButtonType.TextOnly;
			buttonStart.Label = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 44, new Color(178, 219, 255), "Start");
			buttonStart.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - buttonStart.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2 + 25);

			buttonOptions = ControlFactory.CreateButton(ContentManager, "ButtonSplash", "ButtonSplashHover");
			buttonOptions.ButtonType = ButtonType.TextOnly;
			buttonOptions.Label = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 44, new Color(178, 219, 255), "Options");
			buttonOptions.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - buttonOptions.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2 + 105);

			buttonQuit = ControlFactory.CreateButton(ContentManager, "ButtonSplash", "ButtonSplashHover");
			buttonQuit.ButtonType = ButtonType.TextOnly;
			buttonQuit.Label = ControlFactory.CreateLabel(ContentManager, Styles.Fonts.Anton, 44, new Color(178, 219, 255), "Quit");
			buttonQuit.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - buttonQuit.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2 + 185);

			iconLogo = ControlFactory.CreateIcon(ContentManager, "IconLogo");
			iconLogo.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - iconLogo.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL / 2 - iconLogo.Height);

			textureBackgroundTile = ContentManager.GetTexture("SplashBackgroundTile");

			AddControl(iconLogo);
			AddControl(buttonStart);
			AddControl(buttonOptions);
			AddControl(buttonQuit);

			buttonStart.Clicked += buttonStart_Clicked;
			buttonOptions.Clicked += buttonOptions_Clicked;
			buttonQuit.Clicked += buttonQuit_Clicked;
		}

		public override void HandleKeyReleasedEvent(object sender, KeyboardEventArgs e)
		{
			base.HandleKeyReleasedEvent(sender, e);

			if (e.KeyInformation.VirtualKey == SharpDL.Input.VirtualKeyCode.Escape)
				OnQuit(this, EventArgs.Empty);
		}

		private void OnQuit(object sender, EventArgs e)
		{
			if (QuitClicked != null)
				QuitClicked(sender, e);
		}

		private void buttonQuit_Clicked(object sender, EventArgs e)
		{
			OnQuit(sender, e);
		}

		private void buttonOptions_Clicked(object sender, EventArgs e)
		{
			if (OptionsClicked != null)
				OptionsClicked(sender, e);
		}

		private void buttonStart_Clicked(object sender, EventArgs e)
		{
			if (StartClicked != null)
				StartClicked(sender, e);
		}

		public override void Update(GameTime gameTime, bool otherWindowHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherWindowHasFocus, coveredByOtherScreen);

			AdjustTileOffset();

			//AdjustRedColorMod();

			iconLogo.TextureFrame.SetColorMod((byte)redMod, (byte)greenMod, (byte)blueMod);
		}

		public override void Draw(GameTime gameTime, Renderer renderer)
		{
			for (int x = -1; x <= MainGame.SCREEN_WIDTH_LOGICAL / textureBackgroundTile.Width; x++)
				for (int y = -1; y <= MainGame.SCREEN_HEIGHT_LOGICAL / textureBackgroundTile.Height; y++)
					textureBackgroundTile.Draw((x + tileOffset) * textureBackgroundTile.Width, (y + tileOffset) * textureBackgroundTile.Height);

			base.Draw(gameTime, renderer);
		}

		private void AdjustRedColorMod()
		{
			if (redIncreasing)
			{
				if (redMod < 255)
					redMod += 3f;
				else
					redIncreasing = false;
			}
			else
			{
				if (redMod > 0)
					redMod -= 3f;
				else
					redIncreasing = true;
			}
		}

		private void AdjustTileOffset()
		{
			tileOffset += .005f;
			if (tileOffset >= 1)
				tileOffset = 0;
		}

		public override void Dispose()
		{
			base.Dispose();

			textureBackgroundTile.Dispose();
		}
	}
}
