using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameSystem
{
    public class Start : GameSystem.Processing
    {
        public override void Initialize()
        {
            
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(ResourceManager.Instance.CoInit());

            yield return StartCoroutine(CoLoadConst());
        }

        private IEnumerator CoLoadConst()
        {
            var addressableAssetLoader = ResourceManager.Instance?.AddressableAssetLoader;
            if (addressableAssetLoader == null)
                yield break;

            bool endLoad = false;

            yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<GameData.Const>(
                "Const",
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if (result == null)
                        return;

                    Game.Data.Const = result;

                    endLoad = true;
                }));

            yield return new WaitUntil(() => endLoad);
        }
    }
}
