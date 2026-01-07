using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            int placeId = GameUtils.ActivityPlaceId;

            return GetSprite(KeyAnimalIcon + "_" + placeId, name);
        }

        public Sprite GetObjectIconSpriteByPlace(string name)
        {
            int placeId = GameUtils.ActivityPlaceId;

            return GetSprite(KeyObjectIcon + "_" + placeId, name);
        }

        public Sprite GetObjectIconSprite(string name)
        {
            return GetSprite(KeyObjectIcon, name);
        }

        public Sprite GetCurrencySprite(string name)
        {
            return GetSprite("Currency", name);
        }

        public Sprite GetAnimalCurrencySpriteByPlaceId(int placeId)
        {
            Game.IPlaceData iPlaceData = MainGameManager.Get<Game.PlaceManager>();
            var placeData = iPlaceData?.GetPlaceData(placeId);
            if (placeData == null)
                return null;

            return GetCurrencySprite(placeData.AnimalSpriteName);
        }

        public Sprite CurrencyCashSprite
        {
            get
            {
                return GetCurrencySprite("jewel");
            }
        }

        public Sprite GetAnimalSkinSprite(string name)
        {
            int placeId = GameUtils.ActivityPlaceId;

            return GetSprite("AnimalSkinIcon_" + placeId, name);
        }

        public Sprite GetShopItemSprite(Game.Type.ECategory eCategory, string name)
        {
            if(eCategory == Game.Type.ECategory.Cash)
            {
                return GetSprite("Shop", name);
            }

            var placeData = MainGameManager.Get<Game.PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return null;

            string currencyName = eCategory == Game.Type.ECategory.AnimalCurrency ? placeData.Animal.ToString() : placeData.Object.ToString();

            return GetSprite("Shop", string.Format(name, currencyName));
        }

        public Sprite GetItemSprite(string name)
        {
            return GetSprite("Item", name);
        }

        public Sprite GetCropSprite(string name)
        {
            return GetSprite("Crop", name);
        }
    }
}

