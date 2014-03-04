using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.UserInterface
{
    public class Portrait : Control
    {
        // content
        private SpriteFont portraitFont;
        private String portraitName;
        private Texture2D panelImage;
        private Texture2D portraitImage;

        // positioning
        private static Vector2 portraitImageOffset = new Vector2(3, 27);
        private Vector2 portraitFontOffset;
        private const int portraitFontOffsetY = 3;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 speed = new Vector2(0, 3);
        private float totalDistanceBetweenStartAndEnd;

        public override Rectangle Bounds { get { return new Rectangle((int)Position.X, (int)Position.Y, panelImage.Width, panelImage.Height); } }

        //todo: separate position, startPosition, and endPosition set logic
        public override Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                startPosition = value;
                endPosition = new Vector2(value.X, value.Y - panelImage.Height);
                totalDistanceBetweenStartAndEnd = Vector2.Distance(startPosition, endPosition);
            }
        }

        public override int Width { get { return panelImage.Width; } }
        public override int Height { get { return panelImage.Height; } }

        protected enum TransitionState
        {
            Entering,
            Exiting,
            Stopped
        }

        private TransitionState transitionState;

        //private KeyboardState previousKeyboardState;

        public Portrait(ContentManager content, Texture2D portraitImage, String portraitName, Vector2 position)
        {
            this.panelImage = content.Load<Texture2D>("User Interface/Controls/Portrait");
            this.portraitFont = content.Load<SpriteFont>("Fonts/ResagokrBold16");
            this.portraitImage = portraitImage;
            this.Position = position;
            this.portraitName = portraitName;
            // center font
            this.portraitFontOffset = new Vector2((panelImage.Width / 2) - (portraitFont.MeasureString(portraitName).X / 2), portraitFontOffsetY);
            this.transitionState = TransitionState.Entering;
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Slide(delta);
        }

        //private void HandleInput()
        //{
        //    KeyboardState keys = Keyboard.GetState();

        //    if (previousKeyboardState == null)
        //        previousKeyboardState = keys;

        //    if (!previousKeyboardState.IsKeyDown(Keys.Space) && keys.IsKeyDown(Keys.Space))
        //    {
        //        if (transitionState == TransitionState.Exiting)
        //            transitionState = TransitionState.Entering;
        //        else if (transitionState == TransitionState.Entering)
        //            transitionState = TransitionState.Exiting;
        //    }

        //    previousKeyboardState = keys;
        //}

        public void Enter()
        {
            transitionState = TransitionState.Entering;
        }

        public void Exit()
        {
            transitionState = TransitionState.Exiting;
        }

        private void Slide(float delta)
        {
            if (transitionState == TransitionState.Entering)
                position.Y += (endPosition.Y - position.Y) * speed.Y * delta; // MathHelper.Lerp(currentPosition.Y, endPosition.Y, speed.Y * delta);
            else if (transitionState == TransitionState.Exiting)
                position.Y += (startPosition.Y - position.Y) * speed.Y * delta;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha)
        {
            Color color = transitionColor * transitionAlpha;

            spriteBatch.Draw(panelImage, Position, color);
            spriteBatch.Draw(portraitImage, Position + portraitImageOffset, color);
            spriteBatch.DrawString(portraitFont, portraitName, Position + portraitFontOffset, color);
        }
    }
}
