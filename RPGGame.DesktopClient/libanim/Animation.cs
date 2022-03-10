using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGGame.DesktopClient.libanim
{
    internal class Animation
    {
        public enum AnimationLoopType
        {
            CutToFirst,
            Reverse
        }

        private Texture2D _spriteTexture;
        private Vector2 _framePosition;
        private Vector2 _frameSize;
        private Rectangle _frame;
        private int _frameIndex;
        private int _frameLast;
        private AnimationLoopType _loopType;
        private bool _reverseLoopActive;

        public Animation(Texture2D spritesheet, Vector2 framePos, Vector2 frameSize, int frameCount, AnimationLoopType loopType = AnimationLoopType.CutToFirst)
        {
            _spriteTexture = spritesheet;
            _framePosition = framePos;
            _frameSize = frameSize;
            UpdateFrameRect();
            _frameIndex = 0;
            _frameLast = frameCount - 1;
            _loopType = loopType;
            _reverseLoopActive = false;
        }

        private void UpdateFrameRect()
        {
            int _frameX = (int)_framePosition.X + (int)_frameSize.X * _frameIndex;
            _frame = new Rectangle(_frameX, (int)_framePosition.Y, (int)_frameSize.X, (int)_frameSize.Y);
        }

        public void Next()
        {
            switch (_loopType)
            {
                case AnimationLoopType.CutToFirst:
                    {
                        // If On Last Frame, Cut to First Frame, Otherwise, go to next frame
                        if (_frameIndex == _frameLast)
                        {
                            _frameIndex = 0;
                        }
                        else
                        {
                            _frameIndex++;
                        }
                        break;
                    }
                case AnimationLoopType.Reverse:
                    {
                        if (_reverseLoopActive)
                        {
                            // Play Animation Reverse Until Reaching the Beginning
                            if (_frameIndex == 0)
                            {
                                _reverseLoopActive = false;
                                _frameIndex++;
                            }
                            else
                            {
                                _frameIndex--;
                            }
                        }
                        else
                        {
                            // Play Animation Forward Until Reaching the End
                            if (_frameIndex == _frameLast)
                            {
                                _reverseLoopActive = true;
                                _frameIndex--;
                            }
                            else
                            {
                                _frameIndex++;
                            }
                        }
                        break;
                    }
            }

            UpdateFrameRect();
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destRect)
        {
            spriteBatch.Draw(_spriteTexture, destRect, _frame, Color.White);
        }

        public Animation Clone()
        {
            int _frameCount = _frameLast + 1;
            return new Animation(_spriteTexture, _framePosition, _frameSize, _frameCount, _loopType);
        }
    }
}
