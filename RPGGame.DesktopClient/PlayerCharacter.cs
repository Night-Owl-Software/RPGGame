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
    internal class PlayerCharacter
    {
        private AnimationController _animationControl;
        private Dictionary<string, Animation> _animationMap;
        private Vector2 _position;
        private Vector2 _size;
        private float _moveSpeed;
        private float _jumpSpeed;
        private float _gravity;
        private float _maxFallSpeed;
        private bool _isGrounded;
        private bool _isMoving;
        private bool _isJumping;
        private Rectangle _drawRect;
        
        public PlayerCharacter(Vector2 position, Vector2 size, float moveSpeed)
        {
            _animationMap = new Dictionary<string, Animation>();

            Texture2D _spritesheet = GFX.GetSpriteSheet("TestPlayer");
            Animation _animationIdle = new Animation(
                _spritesheet, 
                new Vector2(0, 0), 
                new Vector2(32, 32), 
                4, 
                Animation.AnimationLoopType.Reverse);

            _animationMap.Add("PlayerIdle", _animationIdle);

            _animationControl = new AnimationController(_animationMap["PlayerIdle"], 0.1f);
            _position = position;
            _size = size;
            _moveSpeed = moveSpeed;
            _jumpSpeed = moveSpeed * 2.0f;

            _isGrounded = true;
            _isMoving = false;
            _isJumping = false;

            UpdateDrawRect();

            // Register Events
            Input.LeftPress += OnLeftPressed;
            Input.RightPress += OnRightPressed;
            Input.DownPress += OnDownPressed;
            Input.UpPress += OnUpPressed;
        }

        private void UpdateDrawRect()
        {
            _drawRect = new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y);
        }

        public void Update(GameTime gameTime)
        {
            _animationControl.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animationControl.Draw(spriteBatch, _drawRect);
        }

        private void OnLeftPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X - (int)(_moveSpeed * e.DeltaTime);

            if( _x < 0)
            {
                _x = 0;
            }

            _position = new Vector2(_x, _position.Y);
            UpdateDrawRect();
        }

        private void OnRightPressed(object sender, MovementEventArgs e)
        {
            int _x = (int)_position.X + (int)(_moveSpeed * e.DeltaTime);

            if (_x > (800 - _size.X))
            {
                _x = (800 - (int)_size.X);
            }

            _position = new Vector2(_x, _position.Y);
            UpdateDrawRect();
        }

        private void OnDownPressed(object sender, MovementEventArgs e)
        {
            int _y = (int)_position.Y + (int)(_moveSpeed * e.DeltaTime);

            if (_y > (600 - _size.Y))
            {
                _y = (600 - (int)_size.Y);
            }

            _position = new Vector2(_position.X, _y);
            UpdateDrawRect();
        }

        private void OnUpPressed(object sender, MovementEventArgs e)
        {
            int _y = (int)_position.Y - (int)(_moveSpeed * e.DeltaTime);

            if (_y < 0)
            {
                _y = 0;
            }

            _position = new Vector2(_position.X, _y);
            UpdateDrawRect();
        }
    }
}
