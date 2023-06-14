using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using GameSystem;
using Info;
using UI;


namespace Game.Manager
{
    public class OpenCondition : GameSystem.Processing
    {
        private List<GameData.OpenCondition> _openConditionList = new();
        private int _placeId = 0;
        
        public UnityEvent<int> Listener { get; private set; }= new();

        public override void Initialize()
        {
            Listener?.RemoveAllListeners();

            var mainGameMgr = MainGameManager.Instance;
            mainGameMgr?.AnimalMgr?.Listener?.AddListener(OnChangedAnimalInfo);
            mainGameMgr?.ObjectMgr?.Listener?.AddListener(OnChangedObjectInfo);
            mainGameMgr?.placeMgr?.Listener?.AddListener(OnChangedPlace);
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            _openConditionList.Clear();

            var story = iProvider.Get<Story>();
            story?.Listener?.AddListener(OnChangedStory);

            yield return StartCoroutine(CoLoadOpenCondition());

            Check();
        }
        
        private IEnumerator CoLoadOpenCondition()
        {
            var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
            if(addressableAssetLoader == null)
                yield break;

            bool endLoad = false;

            yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<GameData.OpenCondition>(
                addressableAssetLoader.AssetLabelOpenCondition,
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if(result == null)
                        return;

                    _openConditionList.Add(result);

                    endLoad = true;
                }));
        
            yield return new WaitUntil(() => endLoad);
        }

        private bool Check()
        {
            foreach (var openCondition in _openConditionList)
            {
                if(openCondition == null)
                    continue;
                //
                // openCondition.AlreadExist = false;
                if(openCondition.Exist)
                    continue;

                var data = openCondition.Data_;
                if(data == null)
                    continue;
               
                if (openCondition.Starter)
                {
                    CreateUnlockPopup(openCondition);
                }
                
                // switch (data.EOpenType)
                // {
                //     case Type.EOpen.Object:
                //     {
                //         var objectMgr = MainGameManager.Instance?.ObjectMgr;
                //         if (objectMgr == null)
                //             break;
                //
                //         var objectData = ObjectContainer.Instance.GetData(data.Id);
                //         if (_placeId == objectData.PlaceId)
                //         {
                //             if (objectMgr.CheckExist(data.Id))
                //             {
                //                 
                //             }
                //         }
                //         
                //         break;
                //     }
                //     
                //     case Type.EOpen.Animal:
                //     {
                //         break;
                //     }
                //    
                //
                //     case Type.EOpen.Story:
                //     {
                //         
                //         break;
                //     }
                //         
                //     
                //     default:
                //         break;
                // }
            }

            return true;
        }

        private void CreateUnlockPopup(GameData.OpenCondition openCondition)
        {
            if (openCondition == null)
                return;

            var data = openCondition.Data_;
            if (data == null)
                return;

            if (Enum.TryParse(data.EOpenType.ToString(), out Type.EMain eMain))
            {
                new PopupCreator<Unlock, Unlock.Data>()
                    .SetData(new Unlock.Data()
                    {
                        EMain = eMain,
                        Id = data.Id,
                        ClickAction = () =>
                        {
                        
                        },
                    })
                    .SetCoInit(true)
                    .SetReInitialize(true)
                    .Create();
                
            }
            // openCondition.AlreadExist = true;
        }

        public bool CheckOpenCondition(Type.EOpen eOpenType, int id)
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;
            
            var userInfo = UserManager.Instance?.User;
            if (userInfo == null)
                return false;
            
            foreach (var openCondition in _openConditionList)
            {
                if (openCondition == null)
                    continue;
                
                if(openCondition.Exist)
                    continue;

                var data = openCondition.Data_;
                if(data == null)
                    continue;
                
                if(data.EOpenType != eOpenType)
                    continue;
                
                if(data.Id != id)
                    continue;
                
                if(openCondition.ReqLeaf > userInfo.Leaf)
                    continue;

                var reqDatas = openCondition.ReqDatas;
                if (reqDatas != null)
                {
                    foreach (var reqData in reqDatas)
                    {
                        if(reqData == null)
                            continue;

                        switch (reqData.EOpenType)
                        {
                            case Type.EOpen.Story:
                            {
                                var story = mainGameMgr.Story;
                                if (story == null)
                                    return false;
                                
                                if (!mainGameMgr.Story.CheckCompleted(id))
                                {
                                    return false;
                                }
                                
                                break;
                            }
                            
                            case Type.EOpen.Animal:
                            case Type.EOpen.Object:
                            {
                                if (Enum.TryParse(reqData.EOpenType.ToString(), out Type.EMain eMain))
                                {
                                    if (!mainGameMgr.CheckExist(eMain, reqData.Id))
                                    {
                                        return false;
                                    }
                                }
                                
                                break;
                            }

                            default:
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return true;
        }
        
        #region Listener
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            Debug.Log("OnChanged AnimalInfo = " + animalInfo.Id);
        }
        
        private void OnChangedObjectInfo(Info.Object objectInfo)
        {
            Debug.Log("OnChanged ObjectInfo = " + objectInfo.Id);
        }

        private void OnChangedStory(Story.Data storyData)
        {
            Debug.Log(storyData.EState + " / Id = " + storyData.Id);
        }

        private void OnChangedPlace(int placeId)
        {
            _placeId = placeId;
        }
        #endregion
    }
}
