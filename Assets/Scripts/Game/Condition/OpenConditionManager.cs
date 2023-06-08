using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Manager;
using UnityEngine;

using GameData;
using Info;
using UI;

namespace GameSystem
{
    public class OpenConditionManager : GameSystem.Processing
    {
        private List<OpenCondition> _openConditionList = new();
        private int _activityPlaceId = 0;
        private Unlock _unlockPopup = null; 
        
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

        public bool CheckOpenCondition()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;

            var activityPlaceId = mainGameMgr.placeMgr?.ActivityPlace?.Id;
            
            foreach (var openCondition in _openConditionList)
            {
                if(openCondition == null)
                    continue;
                
                if(openCondition.AlreadExist)
                    continue;

                var data = openCondition.Data_;
                if(data == null)
                    continue;
               
                if (openCondition.Starter)
                {
                    Debug.Log("starter = " + openCondition.name );

                    _unlockPopup = new PopupCreator<Unlock, Unlock.Data>()
                        .SetData(new Unlock.Data()
                        {
                            EOpenType = data.EOpenType,
                            Id = data.Id,
                            // ClickAction = () =>
                            // {
                            //     Cutscene.Create(null);
                            // },
                        })
                        .SetCoInit(true)
                        .Create();
                }
                
                switch (data.EOpenType)
                {
                    case Type.EOpen.Object: 
                        {
                            var objectData = ObjectContainer.Instance.GetData(data.Id);
                            if (activityPlaceId == objectData.PlaceId)
                            {
                                if (mainGameMgr.ObjectMgr.CheckExist(data.Id))
                                {
                                
                                }
                            }
                        }
                        break;

                    case Type.EOpen.Animal:
                        {
                            
                        }
                        break;
                    
                    default:
                        break;
                }
            }
                
            Debug.Log(_openConditionList.Count);

            return true;
        }
    }
}
