using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameData;
using GameSystem;
using Info;
using UI;

namespace Game.Manager
{
    public class OpenCondition : GameSystem.Processing, MainGameManager.ICondition
    {
        private List<GameData.OpenCondition> _openConditionList = new();
        private int _activityPlaceId = 0;

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

        bool MainGameManager.ICondition.Check(int placeId)
        {
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
                    CreateUnlockPopup(data);
                }
                
                switch (data.EOpenType)
                {
                    case Type.EOpen.Object:
                    {
                        var objectMgr = MainGameManager.Instance?.ObjectMgr;
                        if (objectMgr == null)
                            break;

                        var objectData = ObjectContainer.Instance.GetData(data.Id);
                        if (placeId == objectData.PlaceId)
                        {
                            if (objectMgr.CheckExist(data.Id))
                            {
                                
                            }
                        }
                        
                        break;
                    }
                    
                    case Type.EOpen.Animal:
                    {
                        break;
                    }
                   

                    case Type.EOpen.Story:
                    {
                        
                        break;
                    }
                        
                    
                    default:
                        break;
                }
            }

            return true;
        }

        private void CreateUnlockPopup(GameData.OpenCondition.Data data)
        {
            if (data == null)
                return;

            new PopupCreator<Unlock, Unlock.Data>()
                .SetData(new Unlock.Data()
                {
                    EOpenType = data.EOpenType,
                    Id = data.Id,
                    ClickAction = () =>
                    {
                        
                    },
                })
                .SetCoInit(true)
                .Create();
        }
    }
}
