using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using RPGGame.DesktopClient.libgfx;
using RPGGame.DesktopClient.libinput;

namespace RPGGame.DesktopClient
{
    public class MainGame : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        public SpriteBatch SpriteBatch;
        private ScreenManager _screenManager = new ScreenManager();
        private TestGameScreen _testScreen;

        public MainGame()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager.Initialize();
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            GFX.Initialize(Content);
            Input.Initialize();

            _testScreen = new TestGameScreen(this);
            _screenManager.LoadScreen(_testScreen, new FadeTransition(GraphicsDevice, Color.Black));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update(gameTime);
            _screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screenManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
