using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Manager
{
    public class Story : GameSystem.Processing
    {
        public enum EState
        {
            None,
            
            Start,
            End,
        }

        public class Data
        {
            public int Id = 0;
            public EState EState = EState.None;
        }
        
        private Dictionary<int, GameData.Story> _storyDic = new();
        private int _placeId = 0;
        
        public UnityEvent<Data> Listener = new();

        public override void Initialize()
        {
            _storyDic.Clear();
            
            var mainGameMgr = MainGameManager.Instance;
            mainGameMgr?.AnimalMgr?.Listener.AddListener(OnChangedAnimalInfo);
            mainGameMgr?.placeMgr?.Listener?.AddListener(OnChangedPlace);
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(CoLoadStory());
        }
        
        private IEnumerator CoLoadStory()
        {
            var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
            if(addressableAssetLoader == null)
                yield break;

            bool endLoad = false;

            yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<GameData.Story>(
                addressableAssetLoader.AssetLabelStory,
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if(result == null)
                        return;
                    
                    _storyDic.TryAdd(result.PlaceId, result);
                    
                    endLoad = true;
                }));
        
            yield return new WaitUntil(() => endLoad);
        }

        private bool Check()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;
            
            if (_storyDic.TryGetValue(_placeId, out GameData.Story story))
            {
                if (story == null)
                    return false;

                if (story.Datas == null)
                    return false;

                for (int i = 0; i < story.Datas.Length; ++i)
                {
                    var data = story.Datas[i];
                    if(data == null)
                        continue;
                    
                    if(data.Completed)
                        continue;

                    if (data.ReqDatas == null)
                        continue;
                    
                    bool check = true;

                    foreach (var reqData in data.ReqDatas)
                    {
                        if(reqData == null)
                            continue;

                        if (Enum.TryParse(reqData.EOpenType.ToString(), out Type.EMain eMain))
                        {
                            if (!mainGameMgr.CheckExist(eMain, reqData.Id))
                            {
                                check = false;

                                break;
                            }
                        }
                    }
   
                    if (check)
                    {
                        StartStory(i + 1, data);

                        break;
                    }
                }
            }
            
            return false;
        }

        private void StartStory(int storyId, GameData.Story.Data storyData)
        {
            if (storyData == null)
                return;
            
            Cutscene.Create(new Cutscene.Data()
            {
                TargetGameObj = storyData.PlayStory,
                EndAction = () =>
                {
                    EndStory(storyId, storyData);
                },
            });
                        
            Listener?.Invoke(new Data()
            {
                Id = storyId,
                EState = EState.Start,
            });
        }

        private void EndStory(int storyId, GameData.Story.Data storyData)
        {
            if (storyData == null)
                return;
            
            Listener?.Invoke(new Data()
            {
                Id = storyId,
                EState = EState.End,
            });

            storyData.Completed = true;
        }

        public bool CheckCompleted(int storyId)
        {
            if (_storyDic.TryGetValue(_placeId, out GameData.Story story))
            {
                if (story == null)
                    return false;

                if (story.Datas == null)
                    return false;

                int index = storyId - 1;
                if (story.Datas.Length <= index ||
                    0 < index)
                    return false;

                var storyData = story.Datas[index];
                if (storyData == null)
                    return false;
                
                return storyData.Completed;
            }

            return false;
        }

        #region Listener
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            Debug.Log("Start Story Animal = " + animalInfo.Id);
            Check();
        }
        
        private void OnChangedObjectInfo(Info.Object objectInfo)
        {
            Check();
        }
        
        private void OnChangedPlace(int placeId)
        {
            _placeId = placeId;
        }
        #endregion
    }
}

