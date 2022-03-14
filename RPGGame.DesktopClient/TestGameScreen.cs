using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
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
        private OrthographicCamera _sceneCamera;
        private PlayerCharacter _player;

        private List<CollisionObject> _collisionObjects = new List<CollisionObject>();

        public TestGameScreen(MainGame game) : base(game) 
        {
            _tiledMapRenderer = new TiledMapRenderer(Game.GraphicsDevice);
            Game.GraphicsManager.PreferredBackBufferWidth = 800;
            Game.GraphicsManager.PreferredBackBufferHeight = 640;
            Game.GraphicsManager.ApplyChanges();

            var _viewportAdapter = new BoxingViewportAdapter(
                Game.Window,
                Game.GraphicsDevice,
                Game.GraphicsManager.PreferredBackBufferWidth,
                Game.GraphicsManager.PreferredBackBufferHeight);

            _sceneCamera = new OrthographicCamera(_viewportAdapter);

            InitializeScreen();
        }

        public void InitializeScreen()
        {
            LoadContent();

            TiledMapObjectLayer _tiledObjectsLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("Objects");
            TiledMapObjectLayer _tiledCollisionLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("Collisions");
            
            foreach(var obj in _tiledObjectsLayer.Objects)
            {
                if(obj.Name == "SpawnPlayer")
                {
                    _player = new PlayerCharacter(
                        new Vector2((int)obj.Position.X, (int)obj.Position.Y),
                        new Vector2(32, 32),
                        128f);

                    _player.Moved += OnPlayerMoved;
                }
            }

            foreach(var coll in _tiledCollisionLayer.Objects)
            {
                CollisionObject _collision = new CollisionObject(
                    new Vector2((int)coll.Position.X, (int)coll.Position.Y),
                    new Vector2((int)coll.Size.Width, (int)coll.Size.Height));

                if(coll.Type == "Slope")
                {
                    string _leftSlopeHeightString, _rightSlopeHeightString;

                    TiledMapProperties slopeProperties = coll.Properties;
                    slopeProperties.TryGetValue("SlopeLeftSpaceFromTop", out _leftSlopeHeightString);
                    slopeProperties.TryGetValue("SlopeRightSpaceFromTop", out _rightSlopeHeightString);

                    int _slopeHeightLeft = int.Parse(_leftSlopeHeightString);
                    int _slopeHeightRight = int.Parse(_rightSlopeHeightString);

                    _collision.SetSlopeData(_slopeHeightLeft, _slopeHeightRight);
                }

                _collisionObjects.Add(_collision);
            }

            LoadContent();

            _sceneCamera.LookAt(
                new Vector2(
                    Game.GraphicsManager.PreferredBackBufferWidth / 2,
                    Game.GraphicsManager.PreferredBackBufferHeight / 2));
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
            _tiledMapRenderer.Draw(_sceneCamera.GetViewMatrix());
            _player?.Draw(Game.SpriteBatch);
            Game.SpriteBatch.End();
        }

        private void OnPlayerMoved(object sender, PlayerMoveEventArgs e)
        {
            List<CollisionObject> _collisions = new List<CollisionObject>();
            PlayerCharacter _sender = sender as PlayerCharacter;
            Rectangle _senderBoundingbox = e.ProposedBoundingbox;

            foreach(CollisionObject obj in _collisionObjects)
            {
                if (obj.CheckForCollision(_senderBoundingbox))
                {
                    _collisions.Add(obj);
                }
            }

            if( _collisions.Count > 0)
            {
                for(int i = _collisions.Count - 1; i >= 0; i--)
                {
                    CollisionObject _collision = _collisions[i];
                    _senderBoundingbox = _collision.AdjustCollision(_senderBoundingbox);
                    _collisions.RemoveAt(i);

                    if(i > 0)
                    {
                        for(int j = i - 1; j > 0; j--)
                        {
                            if (!_collisions[j].CheckForCollision(_senderBoundingbox))
                            {
                                _collisions.RemoveAt(j);
                            }
                        }
                    }
                }

                _sender.UpdateMovement(_senderBoundingbox);
            }
            else
            {
                _sender.UpdateMovement(_senderBoundingbox);
            }
        }
    }
}
