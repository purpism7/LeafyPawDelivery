using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Edit : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (activityPlace == null)
                return;

            activityPlace.Bust();
        }
    }
}

