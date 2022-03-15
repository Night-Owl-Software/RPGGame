using System;
using System.Collections.Generic;
using System.Text;
using RPGGame.DesktopClient.libanim;
using RPGGame.DesktopClient.libgfx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGGame.DesktopClient.libinput;

namespace RPGGame.DesktopClient
{
    public class PlayerMoveEventArgs : EventArgs
    {
        public Rectangle Collisionbox { get; }
        public Rectangle Landingbox { get; }

        public PlayerMoveEventArgs(Rectangle proposedCollisionbox, Rectangle proposedLandingbox)
        {
            Collisionbox = proposedCollisionbox;
            Landingbox = proposedLandingbox;
        }
    }

    internal class PlayerCharacter
    {
        private const int _landingboxHeight = 4;

        private AnimationController _animationControl;
        private Dictionary<string, Animation> _animationMap;
        private Vector2 _position;
        private Vector2 _previousPosition;
        private Vector2 _size;
        private Rectangle _collisionbox;
        private Rectangle _landingbox;
        private float _moveSpeed;
        private float _jumpSpeed;
        private float _vSpeed;
        private float _gravity;
        private float _maxFallSpeed;
        private bool _isGrounded;
        private bool _isMoving;
        private bool _isJumping;
        private Rectangle _drawbox;

        public bool IsGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        private Texture2D _debugTexture;

        public event EventHandler<PlayerMoveEventArgs> Moved;
        public event EventHandler<PlayerMoveEventArgs> CheckForFall;
        
        public PlayerCharacter(Vector2 position, Vector2 size, float moveSpeed, float jumpspeed, float gravity = 0.15f)
        {
            _animationMap = new Dictionary<string, Animation>();

            Texture2D _spritesheet = GFX.GetSpriteSheet("TestPlayer");
            _debugTexture = GFX.GetSpriteSheet("DebugTexture");
            Animation _animationIdle = new Animation(
                _spritesheet, 
                new Vector2(0, 0), 
                new Vector2(32, 32), 
                4, 
                Animation.AnimationLoopType.Reverse);

            _animationMap.Add("PlayerIdle", _animationIdle);

            _animationControl = new AnimationController(_animationMap["PlayerIdle"], 0.1f);
            _position = position;
            _previousPosition = position;
            _size = size;
            _moveSpeed = moveSpeed;
            _jumpSpeed = jumpspeed;
            _gravity = gravity;
            _maxFallSpeed = _moveSpeed * 2.5f;

            _isGrounded = true;
            _isMoving = false;
            _isJumping = false;

            UpdateCollisionbox();
            UpdateDrawbox();

            // Register Events
            Input.LeftPress += OnLeftPressed;
            Input.RightPress += OnRightPressed;
            //Input.DownPress += OnDownPressed;
            Input.UpClick += OnUpPressed;
        }

        private void UpdateDrawbox()
        {
            _drawbox = new Rectangle(
                (int)_position.X, 
                (int)_position.Y, 
                (int)_size.X, 
                (int)_size.Y);
        }

        private void UpdateCollisionbox()
        {
            _collisionbox = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_size.X,
                (int)_size.Y);

            _landingbox = new Rectangle(
                _collisionbox.Left - 4,
                _collisionbox.Bottom,
                _collisionbox.Width + 8,
                _landingboxHeight);
        }

        public void Update(GameTime gameTime)
        {
            _animationControl.Update(gameTime);

            if (!_isGrounded)
            {
                if(Math.Abs(_vSpeed) < _maxFallSpeed)
                {
                    _vSpeed += _gravity;
                }

                Vector2 _newPosition = new Vector2(
                    _position.X,
                    _position.Y + _vSpeed);
                StageMovement(_newPosition);

                Moved?.Invoke(this, new PlayerMoveEventArgs(_collisionbox, _landingbox));
            }
            else
            {
                // If we are on the ground, check to make sure we are STILL on the ground
                CheckForFall?.Invoke(this, new PlayerMoveEventArgs(_collisionbox, _landingbox));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animationControl.Draw(spriteBatch, _drawbox);
            //spriteBatch.Draw(_debugTexture, _landingbox, Color.White * 0.5f);
        }

        public void Collide(CollisionObject collision, out Rectangle newCollisionbox)
        {
            // Get Center Point for CollisionObject and This to figure
            // placement relationship between colliding objects
            Point _collisionCenter = new Point(
                (collision.Collisionbox.Left + collision.Collisionbox.Right) / 2,
                (collision.Collisionbox.Top + collision.Collisionbox.Bottom) / 2);

            Point _myCenter = new Point(
                (_collisionbox.Left + _collisionbox.Right) / 2,
                (_collisionbox.Top + _collisionbox.Bottom) / 2);

            // Set our base adjustment variables
            int _adjustedX = _collisionbox.Left;
            int _adjustedY = _collisionbox.Top;

            // Get the Rectangle that encompasses the overlap between
            // the two objects
            Rectangle _intersect = CollisionHelper.GetCollisionIntersect(_collisionbox, collision.Collisionbox);

            // Assume nothing about the collision's relationship
            bool _isCollisionLeft = false;
            bool _isCollisionRight = false;
            bool _isCollisionAbove = false;
            bool _isCollisionBelow = false;


            // Check if the Collision's Center point is to our left or right
            if(_collisionCenter.X > _myCenter.X) { _isCollisionRight = true; }
            else if(_collisionbox.X < _myCenter.X) { _isCollisionLeft = true; }

            // Check if the Collision's Center point is above or below us
            if(_collisionCenter.Y > _myCenter.Y) { _isCollisionBelow = true; }
            else if(_collisionCenter.Y < _myCenter.Y) { _isCollisionAbove = true; }

            // Attempt to adjust Left/Right first
            if (_isCollisionLeft)
            {
                // Adjust to the RIGHT
                _adjustedX = _collisionbox.Left + _intersect.Width;
            }

            if (_isCollisionRight)
            {
                // Adjust to the LEFT
                _adjustedX = _collisionbox.Left - _intersect.Width;
            }

            // Check if the resulting move still results in a collision
            Rectangle _proposed = new Rectangle(_adjustedX, _adjustedY, _collisionbox.Width, _collisionbox.Height);
            int _totalMove = Math.Abs(_collisionbox.X - _adjustedX);

            // If Left/Right does NOT fix the problem, then adjust Up/Down instead
            // Also check if the left/right move to fix is too drastic (e.g. moving us off a ledge, rather than above it)
            if(_proposed.Intersects(collision.Collisionbox) || _totalMove > _intersect.Height)
            {
                if (_isCollisionAbove)
                {
                    _adjustedY = _collisionbox.Top + _intersect.Height;
                    _vSpeed = 0f;
                }

                if (_isCollisionBelow)
                {
                    _adjustedY  = _collisionbox.Top - _intersect.Height;

                    // If we are jumping / falling, land at this time
                    if (!_isGrounded)
                    {
                        _isGrounded = true;
                        _vSpeed = 0f;
                    }
                }

                // Update the Position to the new Y, but keep the original X
                _position = new Vector2(_position.X, _adjustedY);
            }
            else
            {
                // If we didn't need to adjust up/down, just
                // update our X position
                _position = new Vector2(_adjustedX, _position.Y);
            }

            // Perform updates to our Collisionbox and DrawRect before passing
            // the new collisionbox back to the caller
            UpdateCollisionbox();
            UpdateDrawbox();
            newCollisionbox = _collisionbox;
        }

        private void StageMovement(Vector2 newPosition)
        {
            _previousPosition = _position;
            _position = newPosition;
            UpdateCollisionbox();
            UpdateDrawbox();
        }

        public void Fall()
        {
            if (_isGrounded)
            {
                _isGrounded = false;
            }
        }

        private void OnLeftPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X - (int)(_moveSpeed * e.DeltaTime);
            Vector2 _newPosition = new Vector2(_x, _position.Y);
            StageMovement(_newPosition);

            Moved?.Invoke(this, new PlayerMoveEventArgs(_collisionbox, _landingbox));
        }
        private void OnRightPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X + (int)(_moveSpeed * e.DeltaTime);
            Vector2 _newPosition = new Vector2(_x, _position.Y);
            StageMovement(_newPosition);

            Moved?.Invoke(this, new PlayerMoveEventArgs(_collisionbox, _landingbox));
        }
        //private void OnDownPressed(object sender, MovementEventArgs e)
        //{
        //    int _y = (int)_position.Y + (int)(_moveSpeed * e.DeltaTime);
        //    Vector2 _newPosition = new Vector2(_position.X, _y);
        //    StageMovement(_newPosition);

        //    Moved?.Invoke(this, new PlayerMoveEventArgs(_collisionbox));
        //}
        private void OnUpPressed(object sender, MovementEventArgs e)
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _vSpeed = -(_jumpSpeed * e.DeltaTime);

                int _y = (int)_position.Y - (int)(_jumpSpeed * e.DeltaTime);
                Vector2 _newPosition = new Vector2(_position.X, _y);
                StageMovement(_newPosition);

                Moved?.Invoke(this, new PlayerMoveEventArgs(_collisionbox, _landingbox));
            }
        }
    }
}
