using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace RPGGame.DesktopClient.libinput
{
    internal static class Input
    {
        private static readonly string _jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Night Owl Software\\RPGGame\\data\\InputMap.json");
        private static Dictionary<string, Keys> _inputMap;
        private static Dictionary<string, bool> _activeKeys;
        private static Dictionary<string, Action> _keyEvents;
        private static float _deltaTime;

        public static event EventHandler LeftClick;
        public static event EventHandler<MovementEventArgs> LeftPress;
        public static event EventHandler LeftRelease;
        public static event EventHandler RightClick;
        public static event EventHandler<MovementEventArgs> RightPress;
        public static event EventHandler RightRelease;
        public static event EventHandler UpClick;
        public static event EventHandler<MovementEventArgs> UpPress;
        public static event EventHandler UpRelease;
        public static event EventHandler DownClick;
        public static event EventHandler<MovementEventArgs> DownPress;
        public static event EventHandler DownRelease;

        public static void Initialize()
        {
            _inputMap = new Dictionary<string, Keys>( JsonConvert.DeserializeObject<Dictionary<string,Keys>>(File.ReadAllText(_jsonPath)) );
            _activeKeys = new Dictionary<string, bool>();

            foreach(string key in _inputMap.Keys)
            {
                _activeKeys[key] = false;
            }

            _keyEvents = new Dictionary<string, Action>()
            {

                { "LeftKeyClick", delegate() { LeftClick?.Invoke(null, EventArgs.Empty); } },
                { "LeftKeyPress", delegate () { OnLeftPressed(1.0f); } },
                { "LeftKeyRelease", delegate () { LeftRelease?.Invoke(null, EventArgs.Empty); } },

                { "RightKeyClick", delegate () { RightClick?.Invoke(null, EventArgs.Empty); } },
                { "RightKeyPress", delegate () {  OnRightPressed(1.0f); } },
                { "RightKeyRelease", delegate () { RightRelease?.Invoke(null, EventArgs.Empty); } },

                { "DownKeyClick", delegate () { DownClick?.Invoke(null, EventArgs.Empty); } },
                { "DownKeyPress", delegate () {  OnDownPressed(1.0f); } },
                { "DownKeyRelease", delegate () { DownRelease?.Invoke(null, EventArgs.Empty); } },

                { "UpKeyClick", delegate () { UpClick?.Invoke(null, EventArgs.Empty); } },
                { "UpKeyPress", delegate () {  OnUpPressed(1.0f); } },
                { "UpKeyRelease", delegate () { UpRelease?.Invoke(null, EventArgs.Empty); } }

            };
        }

        public static void Update(GameTime gameTime)
        {
            _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ProcessKeyboard();
        }

        private static void ProcessKeyboard()
        {
            KeyboardState _keyState = Keyboard.GetState();

            foreach (string key in _inputMap.Keys)
            {
                if (_keyState.IsKeyDown(_inputMap[key]))
                {
                    if (_activeKeys[key])
                    {
                        // Key is Already Down
                        _keyEvents[$"{key}Press"]();
                    }
                    else
                    {
                        // Key Click on this Frame
                        _keyEvents[$"{key}Click"]();
                        _activeKeys[key] = true;
                    }
                }

                if (_keyState.IsKeyUp(_inputMap[key]) && _activeKeys[key])
                {
                    // Key was released this frame
                    _keyEvents[$"{key}Release"]();
                    _activeKeys[key] = false;
                }
            }
        }

        private static void OnLeftPressed(float tilt)
        {
            LeftPress?.Invoke(null, new MovementEventArgs(tilt, _deltaTime));
        }

        private static void OnRightPressed(float tilt)
        {
            RightPress?.Invoke(null, new MovementEventArgs(tilt, _deltaTime));
        }

        private static void OnUpPressed(float tilt)
        {
            UpPress?.Invoke(null, new MovementEventArgs(tilt, _deltaTime));
        }

        private static void OnDownPressed(float tilt)
        {
            DownPress?.Invoke(null, new MovementEventArgs(tilt, _deltaTime));
        }
    }
}
