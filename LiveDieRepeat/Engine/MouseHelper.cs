using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LiveDieRepeat.Engine
{
    public static class MouseHelper
    {
        /// <summary>Translates the actual mouse position obtained from Mouse.GetState() into the virtual mouse position after a scaling matrix is applied to the viewport.
        /// This translation will not take the camera transformation into account. This is useful if you need to click on static screen assets such as user interface elements.
        /// </summary>
        /// <returns>Point in screen space transformed only by the resolution matrix</returns>
        public static Point GetCurrentMousePosition()
        {
            return GetCurrentMousePosition(null);
        }

        /// <summary>Translates the actual mouse position obtained from Mouse.GetState() into the virtual mouse position after a scaling matrix is applied to the viewport.
        /// This translation will take the camera transformation into account. This is useful if you need to get positions for entities that move with the camera (actors, tiles, scenery).
        /// </summary>
        /// <param name="camera"></param>
        /// <returns>Point in screen space transformed by resolution and camera matrices</returns>
        public static Point GetCurrentMousePosition(Camera camera)
        {
            MouseState mouse = Mouse.GetState();

            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);

            Matrix inversionMatrix;
            if (camera != null)
                inversionMatrix = camera.Transform * Resolution.getTransformationMatrix();
            else
                inversionMatrix = Resolution.getTransformationMatrix();

            mousePosition = Vector2.Transform(mousePosition - new Vector2(Resolution.VirtualViewportX, Resolution.VirtualViewportY), Matrix.Invert(inversionMatrix));

            Point virtualMousePosition = new Point((int)mousePosition.X, (int)mousePosition.Y);
            return virtualMousePosition;
        }
    }
}
