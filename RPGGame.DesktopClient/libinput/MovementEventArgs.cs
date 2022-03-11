using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RPGGame.DesktopClient.libinput
{
    internal class MovementEventArgs : EventArgs
    {
        /// <summary>
        /// Ranges from 0.0 to 1.0; Refers to the amount of pressure applied to analog sticks.
        /// For Keyboard Keys, this is always 1.0
        /// </summary>
        public float Tilt { get; }

        /// <summary>
        /// The amount of time (in seconds) that has passed since the last frame.
        /// </summary>
        public float DeltaTime { get; }

        public MovementEventArgs(float tilt, float deltaTime)
        {
            Tilt = tilt;
            DeltaTime = deltaTime;
        }
    }
}
