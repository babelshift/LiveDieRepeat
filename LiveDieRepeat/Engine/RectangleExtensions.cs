using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LiveDieRepeat.Engine
{
    /// <summary>
    /// A set of helpful methods for working with rectangles.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which sides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the position of the center of the bottom edge of the rectangle.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }

        public static void DrawRectangle(Rectangle rec, Texture2D tex, Color col, SpriteBatch spriteBatch, bool solid, int thickness)
        {
            if (!solid)
            {

                Vector2 Position = new Vector2(rec.X, rec.Y);
                int border = thickness;

                int borderWidth = (int)(rec.Width) + (border * 2);
                int borderHeight = (int)(rec.Height) + (border);

                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y), new Vector2((int)rec.X + rec.Width, (int)rec.Y), tex, col, spriteBatch, thickness); //top bar 
                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y + rec.Height), new Vector2((int)rec.X + rec.Width, (int)rec.Y + rec.Height), tex, col, spriteBatch, thickness); //bottom bar 
                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y), new Vector2((int)rec.X, (int)rec.Y + rec.Height), tex, col, spriteBatch, thickness); //left bar 
                drawStraightLine(new Vector2((int)rec.X + rec.Width, (int)rec.Y), new Vector2((int)rec.X + rec.Width, (int)rec.Y + rec.Height), tex, col, spriteBatch, thickness); //right bar 
            }
            else
            {
                spriteBatch.Draw(tex, new Vector2((float)rec.X, (float)rec.Y), rec, col, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }

        }

        //draws a line (rectangle of thickness) from A to B.  A and B have make either horiz or vert line. 
        public static void drawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color col, SpriteBatch spriteBatch, int thickness)
        {
            Rectangle rec;
            if (A.X < B.X) // horiz line 
            {
                rec = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
            }
            else //vert line 
            {
                rec = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
            }

            spriteBatch.Draw(tex, rec, col);
        } 
    }
}
