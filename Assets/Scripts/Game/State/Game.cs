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
        }

        public override async UniTask InitializeAsync(MainGameManager mainGameMgr)
        {
            bool endActivateAnim = false;
            UIManager.Instance?.ActivateAnim(() =>
            {
                endActivateAnim = true;
            });

            await UniTask.WaitUntil(() => endActivateAnim);

            var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (activityPlace == null)
                return;

            activityPlace.Boom();
        }

        public override void End()
        {
            
        }
    }
}
