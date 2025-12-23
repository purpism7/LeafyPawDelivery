using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.State
{
    public class Game : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            //UIManager.Instance?.ActivateAnim();

            //var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            //if (activityPlace == null)
            //    return;

            //activityPlace.Boom();

            var objectList = MainGameManager.Get<PlaceManager>()?.ActivityIPlace?.ObjectList;
            foreach (IObject obj in objectList)
            {
                obj?.SetWaterUIActivate(true);
            }
        }

        public override async UniTask InitializeAsync(MainGameManager mainGameMgr)
        {
            //bool endActivateAnim = false;
            //UIManager.Instance?.ActivateAnim(() =>
            //{
            //    endActivateAnim = true;
            //});

            //await UniTask.WaitUntil(() => endActivateAnim);
            
           

            await UniTask.CompletedTask;
        }

        public override void End()
        {
            //var objectList = MainGameManager.Get<PlaceManager>()?.ActivityIPlace?.ObjectList;
            //foreach (IObject obj in objectList)
            //{
            //    obj?.SetWaterGardenUIActivate(false);
            //}


            UIManager.Instance?.ActivateAnim(() =>
            {
                var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
                if (activityPlace == null)
                    return;

                activityPlace.Boom();
            });
        }
    }
}
