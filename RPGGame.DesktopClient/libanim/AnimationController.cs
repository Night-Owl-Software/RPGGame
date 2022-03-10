using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGGame.DesktopClient.libanim
{
    internal class AnimationController
    {
        private Animation _animation;
        private float _frameTimer;
        private float _maxTimer;
        private bool _paused;
        
        public AnimationController(Animation animation, float maxTimer, bool paused = false)
        {
            _animation = animation;
            _maxTimer = maxTimer;
            _paused = paused;
            _frameTimer = maxTimer;
        }

        public void SwapAnimation(Animation animation)
        {
            _animation = animation;
            _frameTimer = _maxTimer;
        }

        public void SwapAnimation(Animation animation, float maxTimer, bool paused = false)
        {
            maxTimer = maxTimer;
            paused = paused;
            SwapAnimation(animation);
        }

        public void Update(GameTime gameTime)
        {
            if (_paused || _animation == null)
            {
                return;
            }

            if(_frameTimer > 0)
            {
                _frameTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }
            else
            {
                _animation.Next();
                _frameTimer = _maxTimer;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destRect)
        {
            _animation.Draw(spriteBatch, destRect);
        }
    }
}
