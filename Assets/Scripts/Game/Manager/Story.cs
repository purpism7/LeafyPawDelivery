using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Game.Manager
{
    public class Story : GameSystem.Processing
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
    }
}

