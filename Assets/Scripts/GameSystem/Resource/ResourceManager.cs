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
    [System.Serializable]
    public class MaterialData
    {
        public Game.Type.EMaterial eMaterial = Game.Type.EMaterial.None;
        public Material material = null;
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        public AddressableAssetLoader AddressableAssetLoader;
        [SerializeField]
        private MaterialData[] materialDatas = null;

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
        
            await AddressableAssetLoader.LoadAssetAsync<SpriteAtlas>(AddressableAssetLoader.AssetLabelAtlas,
                (asyncOperationHandle) =>
                {
                    var spriteAtlas = asyncOperationHandle.Result;
                    if (spriteAtlas != null)
                    {
                        AtalsLoader.Add(spriteAtlas.name, spriteAtlas);
                    }

                    _endLoad = true;
                });
        }

        public GameObject InstantiateUIGameObj<T>(Transform rootTm)
        {
            if (AddressableAssetLoader == null)
                return null;

            return AddressableAssetLoader.InstantiateUI(typeof(T).FullName, rootTm);
        }

        public T InstantiateUI<T>(Transform rootTm)
        {
            var gameObj = InstantiateUIGameObj<T>(rootTm);
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
                return default;

            var gameObj = AddressableAssetLoader.Instantiate(typeof(T).Name, id, rootTm);
            if (gameObj)
            {
                return gameObj.GetComponent<T>();
            }

            return default;
        }

        public Material GetMaterial(Game.Type.EMaterial eMaterial)
        {
            if (materialDatas == null)
                return null;

            foreach(var materialData in materialDatas)
            {
                if (materialData == null)
                    continue;

                if(materialData.eMaterial == eMaterial)
                {
                    return materialData.material;
                }
            }

            return null;
        }
    }
}

