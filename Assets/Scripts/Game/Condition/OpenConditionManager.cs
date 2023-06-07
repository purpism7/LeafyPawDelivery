using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using GameData;

namespace GameSystem
{
    public class OpenConditionManager : GameSystem.Processing
    {
        private List<OpenCondition> _openConditionList = new();

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            _openConditionList.Clear();
            
            yield return StartCoroutine(CoLoadOpenCondition());
        }
        
        private IEnumerator CoLoadOpenCondition()
        {
            var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
            if(addressableAssetLoader == null)
                yield break;

            bool endLoad = false;

            yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<OpenCondition>(
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

        public bool CheckOpenCondition
        {
            get
            {
                foreach (var openCondition in _openConditionList)
                {
                }
                
                Debug.Log(_openConditionList.Count);

                return true;
            }
        }
    }
}
