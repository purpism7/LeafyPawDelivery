using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Game : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            UIManager.Instance?.ActivateAnim();

            var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (activityPlace == null)
                return;

            activityPlace.Boom();
        }
    }
}
