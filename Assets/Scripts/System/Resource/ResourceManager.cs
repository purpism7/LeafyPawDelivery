using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;

using Cysharp.Threading.Tasks;

using Game.Creature;
using Game;

namespace GameSystem
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public AddressableAssetLoader AddressableAssetLoader;

        public AtlasLoader AtalsLoader { get; private set; } = null;
        private bool _endLoad = false;

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            _endLoad = false;
            InitializeAsync().Forget();

            while(!_endLoad)
            {
                yield return null;
            }
        }

        private async UniTask InitializeAsync()
        {
            _endLoad = false;

            if (AddressableAssetLoader == null)
                return;

            AtalsLoader = new();
            AtalsLoader.Init();

            await AddressableAssetLoader.InitializeAsync();
            //var task = Task.Run();
            //if (task != null)
            //{
            //    //while (!task.IsCompleted)
            //    //{
            //    //    yield return null;
            //    //}
            //}

            //yield return new WaitUntil(() => task.IsCompleted);

            //yield return StartCoroutine();
            //bool endLoad = false;

            await AddressableAssetLoader.LoadAssetAsync<SpriteAtlas>(AddressableAssetLoader.AssetLabelAtlas,
                (asyncOperationHandle) =>
                {
                    var spriteAtlas = asyncOperationHandle.Result;
                    if (spriteAtlas != null)
                    {
                        AtalsLoader.Add(spriteAtlas.name, spriteAtlas);
                    }

                    //endLoad = true;
                    _endLoad = true;
                });

            //StartCoroutine(AddressableAssetLoader.CoLoadAssetAsync<SpriteAtlas>(AddressableAssetLoader.AssetLabelAtlas,
            //    (asyncOperationHandle) =>
            //    {
            //        var spriteAtlas = asyncOperationHandle.Result;
            //        if (spriteAtlas != null)
            //        {
            //            AtalsLoader.Add(spriteAtlas.name, spriteAtlas);
            //        }

            //        endLoad = true;
            //    }));

            //await UniTask.WaitUntil(() => endLoad);
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

        public Game.Creature.Animal InstantiateAnimal(int animalId, int skinId, Transform rootTm)
        {
            if (AddressableAssetLoader == null)
                return null;

            //var fullName = typeof(Game.Creature.Animal).FullName;
            var gameObj = AddressableAssetLoader.InstantiateAnimal(animalId, skinId, rootTm);
            if (gameObj)
            {
                return gameObj.GetComponent<Game.Creature.Animal>();
            }

            return null;
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

