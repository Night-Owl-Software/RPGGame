using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tiled.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGGame.DesktopClient
{
    internal class TestGameScreen : GameScreen
    {
        private new MainGame Game => (MainGame)base.Game;

        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledMapRenderer;
        private PlayerCharacter _player;

        public TestGameScreen(MainGame game) : base(game) 
        {
            _tiledMapRenderer = new TiledMapRenderer(Game.GraphicsDevice);
            Game.GraphicsManager.PreferredBackBufferWidth = 800;
            Game.GraphicsManager.PreferredBackBufferHeight = 640;
            Game.GraphicsManager.ApplyChanges();

            InitializeScreen();
        }

        public void InitializeScreen()
        {
            LoadContent();

            TiledMapObjectLayer _tiledObjectsLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("Objects");
            
            foreach(var obj in _tiledObjectsLayer.Objects)
            {
                if(obj.Name == "SpawnPlayer")
                {
                    _player = new PlayerCharacter(
                        new Vector2((int)obj.Position.X, (int)obj.Position.Y),
                        new Vector2(32, 32),
                        128f);
                }
            }

            LoadContent();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Load Scene Content
            _tiledMap = Game.Content.Load<TiledMap>("Scenes/Tiled/TestMap");
            _tiledMapRenderer.LoadMap(_tiledMap);
        }

        public override void Update(GameTime gameTime)
        {
            // Perform Screen Updates
            _player?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Perform Screen Draw
            Game.GraphicsDevice.Clear(Color.White);
            Game.SpriteBatch.Begin();
            _tiledMapRenderer.Draw();
            _player?.Draw(Game.SpriteBatch);
            Game.SpriteBatch.End();
        }
    }
}
