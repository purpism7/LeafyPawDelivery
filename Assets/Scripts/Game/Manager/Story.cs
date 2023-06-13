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

        protected override void Initialize()
        {
            _storyDic.Clear();
            
            var mainGameMgr = MainGameManager.Instance;
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

                        if (!mainGameMgr.CheckExist(reqData.EOpenType, reqData.Id))
                        {
                            check = false;

                            break;
                        }
                    }
   
                    if (check)
                    {
                        StartStory(i + 1, data.PlayStory);
                        
                        break;
                    }
                }
            }
            
            return false;
        }

        private void StartStory(int storyId, GameObject gameObj)
        {
            Cutscene.Create(new Cutscene.Data()
            {
                Id = storyId,
                TargetGameObj = gameObj,
                EndAction = EndStory,
            });
                        
            Listener?.Invoke(new Data()
            {
                Id = storyId,
                EState = EState.Start,
            });
        }

        private void EndStory(int storyId)
        {
            Listener?.Invoke(new Data()
            {
                Id = storyId,
                EState = EState.End,
            });
        }
        
        private void OnChangedPlace(int placeId)
        {
            _placeId = placeId;
        }
    }
}

