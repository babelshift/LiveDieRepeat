using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LiveDieRepeat.Screens;

namespace LiveDieRepeat.UserInterface
{
    public abstract class Control
    {
        #region Members

        protected Vector2 position;
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        /// <summary>Indicates the containing bounds of the control. Override this and provide logic to calculate the bounds based on width and height.
        /// </summary>
        public abstract Rectangle Bounds { get; }
        public abstract int Height { get; }
        public abstract int Width { get; }

        #endregion

        #region Constructors

        /// <summary>Default constructor
        /// </summary>
        public Control() { }

        /// <summary>Constructor to use if you know the position of the control at creation time
        /// </summary>
        /// <param name="position"></param>
        public Control(Vector2 position)
        {
            Position = position;
        }

        #endregion

        #region Game Loop

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color transitionColor, float transitionAlpha);

        #endregion
    }
}
