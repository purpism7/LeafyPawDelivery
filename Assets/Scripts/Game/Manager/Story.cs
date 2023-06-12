using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Game.Manager
{
    public class Story : GameSystem.Processing, MainGameManager.ICondition
    {
        private Dictionary<int, GameData.Story> _storyDic = new();

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            _storyDic.Clear();

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

        bool MainGameManager.ICondition.Check(int placeId)
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;
            
            if (_storyDic.TryGetValue(placeId, out GameData.Story story))
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
                        Cutscene.Create(new Cutscene.Data()
                        {
                            Id = i + 1,
                            TargetGameObj = data.PlayStory,
                            EndAction = EndStory,
                        });

                        break;
                    }
                }
            }
            
            return false;
        }

        private void EndStory(int storyId)
        {
            Debug.Log(storyId);
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;
            
            mainGameMgr?.CheckOpenCondition();
        }
    }
}

