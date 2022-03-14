using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace RPGGame.DesktopClient.libgfx
{
    public class LoadedTexture
    {
        public int ConnectedObjects;
        public Texture2D Texture;
    }

    internal static class GFX
    {
        private static ContentManager gfxContent;
        private static Dictionary<string, string> _textureMap;
        private static Dictionary<string, LoadedTexture> _loadedTextures;
        private static string _gfxJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Night Owl Software\\RPGGame\\data\\TextureMap.json");

        public static void Initialize(ContentManager content)
        {
            gfxContent = content;
            _loadedTextures = new Dictionary<string, LoadedTexture>();
            InitializeSpritesheetMap();
        }

        private static void InitializeSpritesheetMap()
        {
            _textureMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_gfxJsonPath));
        }

        public static Texture2D GetSpriteSheet(string spritesheetId)
        {
            if(_loadedTextures.Count == 0 || !_loadedTextures.ContainsKey(spritesheetId))
            {
                Texture2D _texture = gfxContent.Load<Texture2D>(_textureMap[spritesheetId]);
                LoadedTexture _loadedTexture = new LoadedTexture { Texture = _texture, ConnectedObjects = 1 };
                _loadedTextures.Add(spritesheetId, _loadedTexture);
                return _texture;
            }
            else
            {
                _loadedTextures[spritesheetId].ConnectedObjects++;
            }

            Texture2D spriteSheet = _loadedTextures[spritesheetId].Texture;
            return spriteSheet;
        }

        public static void UnloadTexture(string spritesheet)
        {
            if(_loadedTextures != null && _loadedTextures.ContainsKey(spritesheet))
            {
                if(_loadedTextures[spritesheet].ConnectedObjects == 1)
                {
                    _loadedTextures[spritesheet].Texture = null;
                    _loadedTextures[spritesheet].Texture.Dispose();
                    _loadedTextures.Remove(spritesheet);
                }
                else
                {
                    _loadedTextures[spritesheet].ConnectedObjects--;
                }
            }
        }
    }
}
