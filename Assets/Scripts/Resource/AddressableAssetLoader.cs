using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GameSystem
{
    public class AddressableAssetLoader : MonoBehaviour
    {
        readonly public string AssetLabelUI = "UI";
        readonly public string AssetLabelGame = "Game";
        readonly public string AssetLabelAtlas = "Atlas";

        public List<AssetLabelReference> InitLoadLabelList;

        private Dictionary<string, Dictionary<int, GameObject>> _gameObjByIdDic = new(); // Animal, Place, Deco
        private Dictionary<string, GameObject> _gameObjDic = new(); // Game
        private Dictionary<string, GameObject> _uiGameObjDic = new(); // UI

        public IEnumerator CoInit()
        {
            _gameObjByIdDic.Clear();
            _gameObjDic.Clear();
            _uiGameObjDic.Clear();

            foreach (var assetLabel in InitLoadLabelList)
            {
                if(assetLabel == null)
                {
                    continue;
                }

                var typeKey = assetLabel.labelString;
                if(typeKey == AssetLabelUI)
                {
                    yield return StartCoroutine(CoLoadUIAsset());
                }
                if (typeKey == AssetLabelGame)
                {
                    yield return StartCoroutine(CoLoadGameAsset());
                }
                else
                {
                    yield return StartCoroutine(CoLoadGameAssetById(typeKey));
                }
            }

            yield return null;
        }

        public IEnumerator CoLoadAssetAsync<T>(string labelKey, System.Action<AsyncOperationHandle<T>> action)
        {
            var locationAsync = Addressables.LoadResourceLocationsAsync(labelKey);
            yield return locationAsync;

            foreach (IResourceLocation resourceLocation in locationAsync.Result)
            {
                var assetAync = Addressables.LoadAssetAsync<T>(resourceLocation);
                assetAync.Completed += action;
            }
        }

        private IEnumerator CoLoadUIAsset()
        {
            yield return CoLoadAssetAsync<GameObject>(AssetLabelUI, (resourceLocation) =>
            {
                var resultGameObj = resourceLocation.Result;
                if (!resultGameObj)
                {
                    return;
                }

                var uiBase = resultGameObj.GetComponent<UI.Base>();
                if (uiBase != null)
                {
                    _uiGameObjDic.TryAdd(uiBase.GetType().Name, resultGameObj);
                }

                //var activityAnimal = resultGameObj.GetComponent<UI.ActivityAnimal>();
                //if (activityAnimal != null)
                //{
                //    _uiGameObjDic.TryAdd(activityAnimal.GetType().Name, resultGameObj);
                //}
            });
        }

        private IEnumerator CoLoadGameAsset()
        {
            yield return CoLoadAssetAsync<GameObject>(AssetLabelGame, (resourceLocation) =>
            {
                var resultGameObj = resourceLocation.Result;
                if (!resultGameObj)
                {
                    return;
                }

                var dropItem = resultGameObj.GetComponent<Game.DropItem>();
                if (dropItem != null)
                {
                    _gameObjDic.TryAdd(dropItem.GetType().Name, resultGameObj);
                }
            });
        }

        private IEnumerator CoLoadGameAssetById(string typeKey)
        {
            yield return CoLoadAssetAsync<GameObject>(typeKey, (resourceLocation) =>
            {
                var resultGameObj = resourceLocation.Result;
                if (!resultGameObj)
                {
                    return;
                }

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

        public GameObject InstantiateUI(string typeKey, RectTransform rootRectTm)
        {
            if (_uiGameObjDic == null)
            {
                return null;
            }

            if (_uiGameObjDic.TryGetValue(typeKey, out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootRectTm);
            }

            return null;
        }

        public GameObject InstantiateGame(string typeKey, Transform rootTm)
        {
            if (_gameObjDic == null)
            {
                return null;
            }

            if (_gameObjDic.TryGetValue(typeKey, out GameObject gameObj))
            {
                return GameObject.Instantiate(gameObj, rootTm);
            }

            return null;
        }

        public GameObject Instantiate(string typeKey, int id, Transform rootTm)
        {
            if(_gameObjByIdDic == null)
            {
                return null;
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
    }
}
