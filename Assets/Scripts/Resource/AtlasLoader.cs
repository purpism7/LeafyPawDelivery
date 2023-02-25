using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace GameSystem
{
    public class AtlasLoader
    {
        readonly public string KeyAnimalIcon = "AnimalIcon";

        private Dictionary<string, SpriteAtlas> _spriteAtlasDic = new();

        public void Init()
        {
            _spriteAtlasDic.Clear();
        }

        public void Add(string key, SpriteAtlas spriteAtlas)
        {
            _spriteAtlasDic.TryAdd(key, spriteAtlas);
        }

        public Sprite GetSprite(string key, string name)
        {
            if(_spriteAtlasDic.TryGetValue(key, out SpriteAtlas spriteAtlas))
            {
                return spriteAtlas.GetSprite(name);
            }

            return null;
        }
    }
}

