using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Game.Creature;
using Game;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace GameSystem
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public AddressableAssetLoader AddressableAssetLoader;

        public AtlasLoader AtalsLoader { get; private set; } = null;

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            if(AddressableAssetLoader != null)
            {
                AtalsLoader = new();
                AtalsLoader.Init();

                yield return StartCoroutine(AddressableAssetLoader.CoInit());
                yield return StartCoroutine(AddressableAssetLoader.CoLoadAssetAsync<SpriteAtlas>(AddressableAssetLoader.AssetLabelAtlas,
                    (asyncOperationHandle) =>
                    {
                        var spriteAtlas = asyncOperationHandle.Result;
                        if(spriteAtlas != null)
                        {
                            AtalsLoader.Add(spriteAtlas.name, spriteAtlas);
                        }
                    }));
            }
        }

        public GameObject InstantiateUIGameObj<T>(RectTransform rootRectTm)
        {
            if (AddressableAssetLoader == null)
                return null;

            return AddressableAssetLoader.InstantiateUI(typeof(T).FullName, rootRectTm);
        }

        public T InstantiateUI<T>(RectTransform rootRectTm)
        {
            var gameObj = InstantiateUIGameObj<T>(rootRectTm);
            if (gameObj)
            {
                return gameObj.GetComponent<T>();
            }

            return default(T);
        }

        public T InstantiateGame<T>(Transform rootTm)
        {
            if (AddressableAssetLoader == null)
                return default(T);

            var fullName = typeof(T).FullName;
            var gameObj = AddressableAssetLoader.InstantiateGame(fullName, rootTm);
            if (gameObj)
            {
                return gameObj.GetComponent<T>();
            }

            return default(T);
        }

        public T Instantiate<T>(int id, Transform rootTm) where T : Game.Base
        {
            if (AddressableAssetLoader == null)
                return default(T);

            var gameObj = AddressableAssetLoader.Instantiate(typeof(T).Name, id, rootTm);
            if (gameObj)
            {
                return gameObj.GetComponent<T>();
            }

            return default(T);
        }
    }
}

