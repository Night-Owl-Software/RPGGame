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
        public Rectangle ProposedBoundingbox { get; }

        public PlayerMoveEventArgs(Rectangle proposedBoundingbox)
        {
            ProposedBoundingbox = proposedBoundingbox;
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
        private Rectangle _boundingbox;
        private Rectangle _landingbox;
        private float _moveSpeed;
        private float _jumpSpeed;
        private float _vSpeed;
        private float _gravity;
        private float _maxFallSpeed;
        private bool _isGrounded;
        private bool _isMoving;
        private bool _isJumping;
        private Rectangle _drawRect;

        public bool IsGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        private Texture2D _debugTexture;

        public event EventHandler<PlayerMoveEventArgs> Moved;
        
        public PlayerCharacter(Vector2 position, Vector2 size, float moveSpeed, float jumpspeed, float gravity = 0.2f)
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
            _maxFallSpeed = _moveSpeed * 3.5f;

            _isGrounded = true;
            _isMoving = false;
            _isJumping = false;

            UpdateBoundingbox();
            UpdateDrawRect();

            // Register Events
            Input.LeftPress += OnLeftPressed;
            Input.RightPress += OnRightPressed;
            Input.DownPress += OnDownPressed;
            Input.UpClick += OnUpPressed;
        }

        private void UpdateDrawRect()
        {
            _drawRect = new Rectangle(
                (int)_position.X, 
                (int)_position.Y, 
                (int)_size.X, 
                (int)_size.Y);
        }

        private void UpdateBoundingbox()
        {
            _boundingbox = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_size.X,
                (int)_size.Y);

            _landingbox = new Rectangle(
                _boundingbox.Left,
                _boundingbox.Bottom,
                _boundingbox.Width,
                _landingboxHeight);
        }

        public void Update(GameTime gameTime)
        {
            _animationControl.Update(gameTime);

            if (!_isGrounded)
            {
                Rectangle _tempLandingbox = new Rectangle(
                    _landingbox.Left,
                    _landingbox.Top,
                    _landingbox.Width,
                    _landingbox.Height);

                if(Math.Abs(_vSpeed) < _maxFallSpeed)
                {
                    _vSpeed += _gravity;
                }

                Rectangle _proposedBoundingbox = new Rectangle(
                    (int)_position.X,
                    (int)_position.Y + (int)_vSpeed,
                    _boundingbox.Width,
                    _boundingbox.Height);

                Moved?.Invoke(this, new PlayerMoveEventArgs(_proposedBoundingbox));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animationControl.Draw(spriteBatch, _drawRect);
            //spriteBatch.Draw(_debugTexture, _boundingbox, Color.White * 0.5f);
        }

        public void UpdateMovement(Rectangle proposedBoundingbox)
        {
            _previousPosition = new Vector2((int)_position.X, (int)_position.Y);
            _position = new Vector2(proposedBoundingbox.Left, proposedBoundingbox.Top);
            UpdateBoundingbox();
            UpdateDrawRect();
        }

        public void LandOnGround()
        {
            _isGrounded = true;
        }

        private void OnLeftPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X - (int)(_moveSpeed * e.DeltaTime);
            _position = new Vector2(_x, _position.Y);

            Rectangle proposed = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _boundingbox.Width,
                _boundingbox.Height);

            Moved?.Invoke(this, new PlayerMoveEventArgs(proposed));
        }
        private void OnRightPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X + (int)(_moveSpeed * e.DeltaTime);
            _position = new Vector2(_x, _position.Y);

            Rectangle proposed = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _boundingbox.Width,
                _boundingbox.Height);

            Moved?.Invoke(this, new PlayerMoveEventArgs(proposed));
        }
        private void OnDownPressed(object sender, MovementEventArgs e)
        {
            int _y = (int)_position.Y + (int)(_moveSpeed * e.DeltaTime);
            _position = new Vector2(_position.X, _y);

            Rectangle proposed = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _boundingbox.Width,
                _boundingbox.Height);

            Moved?.Invoke(this, new PlayerMoveEventArgs(proposed));
        }
        private void OnUpPressed(object sender, MovementEventArgs e)
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _vSpeed = -(_jumpSpeed * e.DeltaTime);

                int _y = (int)_position.Y - (int)(_jumpSpeed * e.DeltaTime);
                _position = new Vector2(_position.X, _y);

                Rectangle proposed = new Rectangle(
                    (int)_position.X,
                    (int)_position.Y,
                    _boundingbox.Width,
                    _boundingbox.Height);

                Moved?.Invoke(this, new PlayerMoveEventArgs(proposed));
            }
        }
    }
}
