using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Engine
{
    public interface IFocusable
    {
        Vector2 FocusPosition { get; }
    }

    public interface ICamera2D
    {
        Vector2 Position { get; set; }
        float MoveSpeed { get; set; }
        float Rotation { get; set; }
        Vector2 Origin { get; set; }
        float Scale { get; set; }
        Vector2 ScreenCenter { get; }
        Matrix Transform { get; }
        IFocusable Focus { get; set; }

        bool IsInView(Vector2 position, int width, int height);
    }

    public class Camera : ICamera2D
    {
        private Vector2 position;
        private float viewHeight;
        private float viewWidth;

        public float MoveSpeed { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public Matrix Transform { get; set; }
        public IFocusable Focus { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public void Initialize()
        {
            viewWidth = Resolution.VirtualViewport.Width;
            viewHeight = Resolution.VirtualViewport.Height;
            ScreenCenter = new Vector2(viewWidth / 2, viewHeight / 2);
            Scale = 1f;
            MoveSpeed = 7f;
        }

        public void Update(GameTime gameTime)
        {
            Origin = ScreenCenter / Scale;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            position.X += (int)((Focus.FocusPosition.X - Position.X) * MoveSpeed * delta);
            position.Y += (int)((Focus.FocusPosition.Y - Position.Y) * MoveSpeed * delta);

            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateTranslation((int)Origin.X, (int)Origin.Y, 0) *
                        Matrix.CreateScale(Scale);
        }

        public bool IsToRightOfView(float x)
        {
            if (x > (position.X + Origin.X))
                return true;
            else
                return false;
        }

        public bool IsToLeftOfView(float x)
        {
            if (x < (Position.X - Origin.X))
                return true;
            else
                return false;
        }

        public bool IsAboveView(float y)
        {
            if (y < (Position.Y - Origin.Y))
                return true;
            else
                return false;
        }

        public bool IsBelowView(float y)
        {
            if (y > (Position.Y + Origin.Y))
                return true;
            else
                return false;
        }

        public bool IsInView(Vector2 position, int width, int height)
        {
            if ((position.X + width) < (this.Position.X - Origin.X) || (position.X) > (this.Position.X + Origin.X))
                return false;

            if ((position.Y + height) < (this.Position.Y - Origin.Y) || (position.Y) > (this.Position.Y + Origin.Y))
                return false;

            return true;
        }
    }
}
