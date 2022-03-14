using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RPGGame.DesktopClient
{
    internal static class CollisionHelper
    {
        public static Rectangle GetCollisionIntersect(Rectangle rect1, Rectangle rect2)
        {
            if (rect1 == null || rect2 == null)
            {
                return Rectangle.Empty;
            }

            Rectangle _collisionOverlap;
            _collisionOverlap = Rectangle.Intersect(rect1, rect2);
            return _collisionOverlap;
        }
    }
}
