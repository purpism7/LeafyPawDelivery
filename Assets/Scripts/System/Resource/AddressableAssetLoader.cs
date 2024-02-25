using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class AddressableAssetLoader : MonoBehaviour
    {
        public const string AssetLabelUI = "UI";
        public const string AssetLabelGame = "Game";
        public const string AssetLabelAnimal = "Animal";
        public const string AssetLabelAtlas = "Atlas";
        public const string AssetLabelCutscene = "Cutscene";
        public const string AssetLabelOpenCondition = "OpenCondition";

        public string AssetLabelObject { get { return "Object"; } }
        public string AssetLabelData { get { return "Data"; } }
        public string AssetLabelStory { get { return "Story"; } }

        public List<AssetLabelReference> InitLoadLabelList;

        private Dictionary<string, Dictionary<int, GameObject>> _gameObjByIdDic = new(); // Place, Object
        private Dictionary<(int, int), GameObject> _animalGameObjDic = new(); // Animal
        private Dictionary<string, GameObject> _gameObjDic = new(); // Game
        private Dictionary<string, GameObject> _uiGameObjDic = new(); // UI
        private Dictionary<string, GameObject> _cutsceneGameObjDic = new(); // Cutscene

        public async UniTask InitializeAsync()
        {
            _gameObjByIdDic.Clear();
            _animalGameObjDic.Clear();
            _gameObjDic.Clear();
            _uiGameObjDic.Clear();

            foreach (var assetLabel in InitLoadLabelList)
            {
                if (assetLabel == null)
                    continue;

                var typeKey = assetLabel.labelString;

                switch (typeKey)
                {
                    case AssetLabelUI:
                        {
                            await LoadUIAssetAsync();

                            break;
                        }

                    case AssetLabelGame:
                        {
                            //yield return StartCoroutine(CoLoadGameAsset());
                            await LoadGameAssetAsync();

                            break;
                        }

                    case AssetLabelCutscene:
                        {
                            //yield return StartCoroutine(CoLoadCutsceneAsset());
                            await LoadCutsceneAssetAsync();

                            break;
                        }

                    case AssetLabelAnimal:
                        {
                            //yield return StartCoroutine(CoLoadAnimalAsset());
                            int placeId = PlayerPrefs.GetInt(Game.Data.PlayPrefsKeyLastPlaceKey, Game.Data.Const.StartPlaceId);

                            await LoadAnimalAssetAsync(placeId);

                            break;
                        }

                    default:
                        {
                            if (typeKey.Equals(AssetLabelObject))
                            {
                                typeKey += "_" + PlayerPrefs.GetInt(Game.Data.PlayPrefsKeyLastPlaceKey, Game.Data.Const.StartPlaceId);
                            }

                            await LoaGameAssetByIdAsync(typeKey);
                            //yield return StartCoroutine(CoLoadGameAssetById(typeKey));

                            break;
                        }
                }
            }
        }

        //public IEnumerator CoInit()
        //{
            

        //    _endLoad = true;

        //    foreach (var assetLabel in InitLoadLabelList)
        //    {
        //        yield return new WaitUntil(() => _endLoad); 
                
        //        if(assetLabel == null)
        //            continue;
                
        //        var typeKey = assetLabel.labelString;
        //        Debug.Log("typeKey = " + typeKey);

        //        _endLoad = false;

        //        switch(typeKey)
        //        {
        //            case AssetLabelUI:
        //                {
        //                     StartCoroutine(CoLoadUIAsset());

        //                    break;
        //                }

        //            case AssetLabelGame:
        //                {
        //                    yield return StartCoroutine(CoLoadGameAsset());

        //                    break;
        //                }

        //            case AssetLabelCutscene:
        //                {
        //                    yield return StartCoroutine(CoLoadCutsceneAsset());

        //                    break;
        //                }

        //            case AssetLabelAnimal:
        //                {
        //                    yield return StartCoroutine(CoLoadAnimalAsset());

        //                    break;
        //                }

        //            default:
        //                {
        //                    if(typeKey.Equals(AssetLabelObject))
        //                    {
        //                        typeKey += "_" + PlayerPrefs.GetInt(Game.Data.PlayPrefsKeyLastPlaceKey, 1);
        //                        Debug.Log(typeKey);
        //                    }

        //                    yield return StartCoroutine(CoLoadGameAssetById(typeKey));

        //                    break;
        //                }
        //        }
        //        //if(typeKey == AssetLabelUI)
        //        //{
        //        //    yield return StartCoroutine(CoLoadUIAsset());
        //        //}
        //        //else if (typeKey == AssetLabelGame)
        //        //{
        //        //    yield return StartCoroutine(CoLoadGameAsset());
        //        //}
        //        //else if (typeKey == AssetLabelCutscene)
        //        //{
        //        //    yield return StartCoroutine(CoLoadCutsceneAsset());
        //        //}
        //        //else if(typeKey == AssetLabelAnimal)
        //        //{
        //        //    yield return StartCoroutine(CoLoadAnimalAsset());
        //        //}
        //        //else
        //        //{
        //        //    yield return StartCoroutine(CoLoadGameAssetById(typeKey));
        //        //}
        //    }
        //}

        public async UniTask LoadAssetAsync<T>(string labelKey, System.Action<AsyncOperationHandle<T>> action)
        {
            var locationAsync = await Addressables.LoadResourceLocationsAsync(labelKey);

            foreach (IResourceLocation resourceLocation in locationAsync)
            {
                var assetAync = Addressables.LoadAssetAsync<T>(resourceLocation);

                await UniTask.WaitUntil(() => assetAync.IsDone);

                if (assetAync.Result == null)
                    continue;

                assetAync.Completed += action;
            }
        }

        public IEnumerator CoLoadAssetAsync<T>(string labelKey, System.Action<AsyncOperationHandle<T>> action)
        {
            var locationAsync = Addressables.LoadResourceLocationsAsync(labelKey);
            yield return StartCoroutine(locationAsync);

            foreach (IResourceLocation resourceLocation in locationAsync.Result)
            {
                var assetAync = Addressables.LoadAssetAsync<T>(resourceLocation);

                while(!assetAync.IsDone)
                {
                    yield return null;
                }
                //yield return new WaitUntil(() => assetAync.IsDone);

                if (assetAync.Result == null)
                    continue;

                assetAync.Completed += action;
            }
        }

        private async UniTask LoadUIAssetAsync()
        {
            await LoadAssetAsync<GameObject>(AssetLabelUI,
                (resourceLocation) =>
                {
                    var resultGameObj = resourceLocation.Result;
                    if (!resultGameObj)
                        return;

                    var uiBase = resultGameObj.GetComponent<UI.Base>();
                    if (uiBase != null)
                    {
                        _uiGameObjDic.TryAdd(uiBase.GetType().FullName, resultGameObj);
                    }

                    //var activityAnimal = resultGameObj.GetComponent<UI.ActivityAnimal>();
                    //if (activityAnimal != null)
                    //{
                    //    _uiGameObjDic.TryAdd(activityAnimal.GetType().Name, resultGameObj);
                    //}
                });
        }
        //private IEnumerator CoLoadUIAsset()
        //{
        //    yield return StartCoroutine(CoLoadAssetAsync<GameObject>(AssetLabelUI, (resourceLocation) =>
        //    {
        //        var resultGameObj = resourceLocation.Result;
        //        if (!resultGameObj)
        //        {
        //            return;
        //        }

        //        var uiBase = resultGameObj.GetComponent<UI.Base>();
        //        if (uiBase != null)
        //        {
        //            _uiGameObjDic.TryAdd(uiBase.GetType().FullName, resultGameObj);
        //        }

        //        //var activityAnimal = resultGameObj.GetComponent<UI.ActivityAnimal>();
        //        //if (activityAnimal != null)
        //        //{
        //        //    _uiGameObjDic.TryAdd(activityAnimal.GetType().Name, resultGameObj);
        //        //}
        //        _endLoad = true;
        //    }));
        //}

        private async UniTask LoadGameAssetAsync()
        {
            await LoadAssetAsync<GameObject>(AssetLabelGame,
                (resourceLocation) =>
                {
                    var resultGameObj = resourceLocation.Result;
                    if (!resultGameObj)
                        return;

                    var gameCommon = resultGameObj.GetComponent<Game.Common>();
                    if (gameCommon != null)
                    {
                        _gameObjDic.TryAdd(gameCommon.GetType().FullName, resultGameObj);
                    }
                });
        }

        //private IEnumerator CoLoadGameAsset()
        //{
        //    yield return StartCoroutine(CoLoadAssetAsync<GameObject>(AssetLabelGame, (resourceLocation) =>
        //    {
        //        var resultGameObj = resourceLocation.Result;
        //        if (!resultGameObj)
        //            return;

        //        var gameCommon = resultGameObj.GetComponent<Game.Common>();
        //        if (gameCommon != null)
        //        {
        //             Debug.Log(gameCommon.GetType().FullName);
        //            _gameObjDic.TryAdd(gameCommon.GetType().FullName, resultGameObj);
        //        }

        //        _endLoad = true;
        //    }));
        //}

        private async UniTask LoadCutsceneAssetAsync()
        {
            await LoadAssetAsync<GameObject>(AssetLabelCutscene,
                (resourceLocation) =>
                {
                    var resultGameObj = resourceLocation.Result;
                    if (!resultGameObj)
                        return;

                    _cutsceneGameObjDic.TryAdd(resultGameObj.name, resultGameObj);
                });
        }

        //private IEnumerator CoLoadCutsceneAsset()
        //{
        //    yield return StartCoroutine(CoLoadAssetAsync<GameObject>(AssetLabelCutscene, (resourceLocation) =>
        //    {
        //        var resultGameObj = resourceLocation.Result;
        //        if (!resultGameObj)
        //            return;

        //        _cutsceneGameObjDic.TryAdd(resultGameObj.name, resultGameObj);

        //        _endLoad = true;
        //    }));
        //}

        public async UniTask LoadAnimalAssetAsync(int placeId)
        {
            string typeKey = string.Format("{0}_{1}", AssetLabelAnimal, placeId);
            
            await LoadAssetAsync<GameObject>(typeKey,
                (resourceLocation) =>
                {
                    var resultGameObj = resourceLocation.Result;
                    if (!resultGameObj)
                        return;

                    var animal = resultGameObj.GetComponent<Game.Creature.Animal>();
                    if (animal != null)
                    {
                        _animalGameObjDic.TryAdd((animal.Id, animal.SkinId), resultGameObj);
                    }
                });
        }

        //private IEnumerator CoLoadAnimalAsset()
        //{
        //    yield return StartCoroutine(CoLoadAssetAsync<GameObject>(AssetLabelAnimal, (resourceLocation) =>
        //    {
        //        var resultGameObj = resourceLocation.Result;
        //        if (!resultGameObj)
        //            return;

        //        var animal = resultGameObj.GetComponent<Game.Creature.Animal>();
        //        if (animal != null)
        //        {
        //            _animalGameObjDic.TryAdd((animal.Id, animal.SkinId), resultGameObj);
        //        }

        //        _endLoad = true;
        //    }));
        //}

        public async UniTask LoaGameAssetByIdAsync(string typeKey)
        {
            await LoadAssetAsync<GameObject>(typeKey,
                (resourceLocation) =>
                {
                    var resultGameObj = resourceLocation.Result;
                    if (!resultGameObj)
                        return;

                    var gameBase = resultGameObj.GetComponent<Game.Base>();
                    if (gameBase != null)
                    {
                        if (_gameObjByIdDic.TryGetValue(typeKey, out Dictionary<int, GameObject> dic))
                        {
                            dic.TryAdd(gameBase.Id, resultGameObj);
                        }
                        else
                        {
                            var gameObjDic = new Dictionary<int, GameObject>();
                            gameObjDic.Clear();
                            gameObjDic.Add(gameBase.Id, resultGameObj);

                            _gameObjByIdDic.Add(typeKey, gameObjDic);
                        }
                    }
                });
        }

        //private IEnumerator CoLoadGameAssetById(string typeKey)
        //{
        //    _endLoad = true;

        //    yield return StartCoroutine(CoLoadAssetAsync<GameObject>(typeKey, (resourceLocation) =>
        //    {
        //        var resultGameObj = resourceLocation.Result;
        //        if (!resultGameObj)
        //            return;
               
        //        var gameBase = resultGameObj.GetComponent<Game.Base>();
        //        if (gameBase != null)
        //        {
        //            if (_gameObjByIdDic.TryGetValue(typeKey, out Dictionary<int, GameObject> dic))
        //            {
        //                dic.TryAdd(gameBase.Id, resultGameObj);
        //            }
        //            else
        //            {
        //                var gameObjDic = new Dictionary<int, GameObject>();
        //                gameObjDic.Clear();
        //                gameObjDic.Add(gameBase.Id, resultGameObj);

        //                _gameObjByIdDic.Add(typeKey, gameObjDic);
        //            }
        //        }
                
        //        //_endLoad = true;
        //    }));
        //}

        //private IEnumerator CoLoadSound()
        //{
        //    yield return StartCoroutine(CoLoadAssetAsync<AudioSource>("Sound", (resourceLocation) =>
        //    {
        //        var audioSource = resourceLocation.Result;
        //        if (!audioSource)
        //            return;


        //        //var animal = resultGameObj.GetComponent<Game.Creature.Animal>();
        //        //if (animal != null)
        //        //{
        //        //    _animalGameObjDic.TryAdd((animal.Id, animal.SkinId), resultGameObj);
        //        //}

        //        _endLoad = true;
        //    }));
        //}

        public GameObject InstantiateUI(string typeKey, RectTransform rootRectTm)
        {
            if (_uiGameObjDic == null)
                return null;

            if (_uiGameObjDic.TryGetValue(typeKey, out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootRectTm);
            }

            return null;
        }

        public GameObject InstantiateGame(string typeKey, Transform rootTm)
        {
            if (_gameObjDic == null)
                return null;

            if (_gameObjDic.TryGetValue(typeKey, out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootTm);
            }

            return null;
        }
        
        public GameObject InstantiateCutscene(string typeKey, Transform rootTm)
        {
            if (_cutsceneGameObjDic == null)
                return null;

            if (_cutsceneGameObjDic.TryGetValue(typeKey, out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootTm);
            }

            return null;
        }

        public GameObject Instantiate(string typeKey, int id, Transform rootTm)
        {
            if(_gameObjByIdDic == null)
                return null;

            if(typeKey.Contains(AssetLabelObject))
            {
                typeKey += "_" + GameUtils.ActivityPlaceId;
            }

            if(_gameObjByIdDic.TryGetValue(typeKey, out Dictionary<int, GameObject> dic))
            {
                if(dic.TryGetValue(id, out GameObject gameObj))
                {
                    return GameObject.Instantiate(gameObj, rootTm);
                }
            }

            return null;
        }

        public GameObject InstantiateAnimal(int animalId, int skinId, Transform rootTm)
        {
            if (_animalGameObjDic == null)
                return null;

            if(_animalGameObjDic.TryGetValue((animalId, skinId), out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootTm);
            }

            return null;
        }
    }
}
