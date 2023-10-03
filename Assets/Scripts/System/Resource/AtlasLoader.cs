using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace GameSystem
{
    public class AtlasLoader
    {
        readonly public string KeyAnimalIcon = "AnimalIcon";
        readonly public string KeyObjectIcon = "ObjectIcon";

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

        public Sprite GetAnimalIconSprite(string name)
        {
            var placeMgr = MainGameManager.Instance.placeMgr;
            if (placeMgr == null)
                return null;

            int placeId = placeMgr.ActivityPlaceId;

            return GetSprite(KeyAnimalIcon + "_" + placeId, name);
        }

        public Sprite GetObjectIconSprite(string name)
        {
            var placeMgr = MainGameManager.Instance.placeMgr;
            if (placeMgr == null)
                return null;

            int placeId = placeMgr.ActivityPlaceId;

            return GetSprite(KeyObjectIcon + "_" + placeId, name);
        }

        public Sprite GetCurrencySprite(string name)
        {
            return GetSprite("Currency", name);
        }

        public Sprite GetCurrencyCashSprite()
        {
            return GetCurrencySprite("berry");
        }
    }
}

