using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Engine
{
    public class Sprite
    {
        #region XML Data

        public int Id;
        public int MillisecondsPerFrame;
        public bool IsAnimated;
        public bool IsLooping;
        public List<Point> TextureIndices = new List<Point>();
        public int FrameWidth;
        public int FrameHeight;

        #endregion

        #region Members

        private bool isActivated = false;
        private bool currentIsLooping;
        private double timeSinceLastFrame = 0;
        private int currentFrameIndex = 0;

        #endregion

        #region Properties

        [ContentSerializerIgnore]
        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2, FrameHeight / 2); }
        }

        [ContentSerializerIgnore]
        public float RadiansOfRotation { get; set; }

        [ContentSerializerIgnore]
        public float ScaleFactor { get; set; }

        [ContentSerializerIgnore]
        public static Vector2 ShadowOffset = new Vector2(7, 7);

        [ContentSerializerIgnore]
        public static Color ShadowColor = new Color(0, 0, 0, 125);

        [ContentSerializerIgnore]
        public bool IsActivated { get { return this.isActivated; } }

        [ContentSerializerIgnore]
        public Point CurrentFrame { get { return TextureIndices[currentFrameIndex]; } }

        /// <summary>Change the animation status of the sprite. If stopping animation, stop looping.
        /// </summary>
        [ContentSerializerIgnore]
        public bool IsAnimating
        {
            get { return IsAnimated; }
            set
            {
                IsAnimated = value;

                if (value == false)
                    currentIsLooping = false;
                else
                    currentIsLooping = IsLooping;
            }
        }

        #endregion

        /// <summary>
        /// todo: some how make this not manually called, maybe only use this class as a run time type that is used to construct a real type? sounds messy
        /// </summary>
        public void Activate()
        {
            this.isActivated = true;
            IsAnimating = IsAnimated;
            currentIsLooping = IsLooping;
            ScaleFactor = 1f;
        }

        public void Update(GameTime gameTime)
        {
            if (IsAnimating)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeSinceLastFrame > MillisecondsPerFrame)
                {
                    timeSinceLastFrame = 0;

                    // if there are more frames in our collection, go to the next one
                    if (currentFrameIndex < TextureIndices.Count - 1)
                        currentFrameIndex++;
                    else
                    {
                        currentFrameIndex = 0;

                        if (!currentIsLooping)
                            IsAnimating = false;
                    }

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool drawShadow)
        {
            Draw(spriteBatch, texture, position, drawShadow, 1);
        }

        /// <summary>
        /// todo: figure out how to draw something without an origin as the center
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            Rectangle sourceRectangle = new Rectangle(CurrentFrame.X * FrameWidth, CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight);

            // draw sprite
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White, RadiansOfRotation, Vector2.Zero, ScaleFactor, SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool drawShadow, float alpha)
        {
            Rectangle sourceRectangle = new Rectangle(CurrentFrame.X * FrameWidth, CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight);

            Vector2 origin = new Vector2(FrameWidth / 2, FrameHeight / 2);

            // draw shadow
            if (drawShadow)
                spriteBatch.Draw(texture, position + Sprite.ShadowOffset, sourceRectangle, Sprite.ShadowColor * alpha, RadiansOfRotation, origin, ScaleFactor, SpriteEffects.None, 0);

            // draw sprite
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White, RadiansOfRotation, origin, ScaleFactor, SpriteEffects.None, 0);
        }

        public Sprite Copy()
        {
            return (Sprite)this.MemberwiseClone();
        }
    }
}
