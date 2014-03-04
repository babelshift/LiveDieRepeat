using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.UserInterface
{
    public class DialogBox : Control
    {
        private Texture2D texture;

        private SpriteFont dialogFont;
        private StringBuilder dialogMessage = new StringBuilder();

        // positioning
        private static Vector2 dialogMessageOffset = new Vector2(10, 10);
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 speed = new Vector2(0, 3);
        private float totalDistanceBetweenStartAndEnd;

        //todo: separate position, startPosition, and endPosition set logic
        public override Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                startPosition = value;
                endPosition = new Vector2(value.X, value.Y - texture.Height);
                totalDistanceBetweenStartAndEnd = Vector2.Distance(startPosition, endPosition);
            }
        }

        public override Rectangle Bounds { get { return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height); } }

        public override int Width { get { return texture.Width; } }
        public override int Height { get { return texture.Height; } }

        protected enum TransitionState
        {
            Entering,
            Exiting,
            Stopped
        }

        protected TransitionState transitionState;

        //private KeyboardState previousKeyboardState;

        public DialogBox(ContentManager content)
        {
            this.dialogFont = content.Load<SpriteFont>("Fonts/ResagokrBold16");
            this.transitionState = TransitionState.Entering;
            this.texture = content.Load<Texture2D>("User Interface/Controls/DialogBox");
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

            spriteBatch.Draw(texture, Position, color);
            spriteBatch.DrawString(dialogFont, dialogMessage.ToString(), Position + dialogMessageOffset, color);
        }

        public void AddMessageText(String messageText)
        {
            dialogMessage.Append(messageText);
            dialogMessage.Append(Environment.NewLine);
        }
    }
}
