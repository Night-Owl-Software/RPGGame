using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RPGGame.DesktopClient
{
    internal class CollisionObject
    {
        private Vector2 _position;
        private Vector2 _size;
        private Rectangle _collisionbox;
        private Point _center;
        private bool _isSlope;
        private int _slopeLeftHeight;
        private int _slopeRightHeight;

        public Rectangle Collisionbox
        {
            get { return _collisionbox; }
        }

        public CollisionObject(Vector2 position, Vector2 size)
        {
            _position = position;
            _size = size;
            _collisionbox = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_size.X,
                (int)size.Y);
            _center = new Point(
                (_collisionbox.Left + _collisionbox.Right) / 2,
                (_collisionbox.Top + _collisionbox.Bottom) / 2);
        }

        public bool CheckForCollision(Rectangle boundingbox)
        {
            if (_collisionbox.Intersects(boundingbox)) { return true; }
            return false;
        }

        //public Rectangle AdjustCollision(Rectangle boundingbox, out bool isCollisionBelow)
        //{
        //    Point _senderCenter = new Point(
        //        (boundingbox.Left + _collisionbox.Right) / 2,
        //        (boundingbox.Top+ _collisionbox.Bottom) / 2);

        //    int _newX = boundingbox.Left;       // X of the Colliding Object after being moved out of intersection
        //    int _newY = boundingbox.Top;        // Y of the Colliding Object after being moved out of intersection
        //    isCollisionBelow = false;           // Assume the collision is NOT below to start

        //    Rectangle _intersect = CollisionHelper.GetCollisionIntersect(_collisionbox, boundingbox); // Intersection Overlap Rectangle

        //    bool _isSenderLeft = false;     // Is the colliding Object's center to the left of ours?
        //    bool _isSenderRight = false;    // Is the colliding Object's center to the right ours?
        //    bool _isSenderBelow = false;    // Is the colliding Object's center below ours?
        //    bool _isSenderAbove = false;    // Is the colliding Object's center above ours?

        //    // Check if Sender is to the left or right of our center
        //    if(_center.X > _senderCenter.X) { _isSenderLeft = true; }
        //    else if(_center.X < _senderCenter.X) { _isSenderRight = true; }
            
        //    // Check if Sender is above or below our center
        //    if(_center.Y > _senderCenter.Y) { _isSenderBelow = true;}
        //    else if(_center.Y < _senderCenter.Y) { _isSenderAbove = true;}

        //    // Use the previous BOOLs to move the Sender out of the collision
        //    if (_isSenderLeft)
        //    {
        //        // Move Sender RIGHT
        //        _newX = boundingbox.Left - _intersect.Width;
        //    }

        //    if (_isSenderRight)
        //    {
        //        // Move Sender LEFT
        //        _newX = boundingbox.Left + _intersect.Width;
        //    }

        //    // Check if adjusting Left/Right still results in an intersect with us
        //    Rectangle _proposedSolution = new Rectangle(_newX, _newY, boundingbox.Width, boundingbox.Height);
        //    int _totalMove = Math.Abs(boundingbox.X - _newX);

        //    // If moving left/right does NOT fix the problem, then we need to move up/down to fix it instead
        //    if (_collisionbox.Intersects(_proposedSolution) || (_totalMove > _intersect.Height))
        //    {
        //        if (_isSenderBelow)
        //        {
        //            // Move Sender DOWN
        //            _newY = boundingbox.Top - _intersect.Height;
        //            isCollisionBelow = true;
        //        }

        //        if (_isSenderAbove)
        //        {
        //            // Move Sender UP
        //            _newY = boundingbox.Top + _intersect.Height;
                    
        //        }

        //        return new Rectangle(boundingbox.X, _newY, boundingbox.Width, boundingbox.Height);
        //    }
        //    else
        //    {
        //        // If moving left/right DID fix the problem, then simply return that solution
        //        return _proposedSolution;
        //    }
        //}

        public void SetSlopeData(int heightFromTopLeft, int heightFromTopRight)
        {
            _isSlope = true;
            _slopeLeftHeight = heightFromTopLeft;
            _slopeRightHeight = heightFromTopRight;
        }
    }
}
