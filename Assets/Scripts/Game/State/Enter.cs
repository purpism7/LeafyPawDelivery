using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Enter : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            // mainGameMgr?.IGameCameraCtr?.MoveCenterGameCamera();
            
            // 진입 연출 전 Deactivate Top, Bottom 
            UIManager.Instance?.DeactivateAnim();

            IPlace place = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (place != null)
                place?.SetWorldUIActive(false);
        }
        
        public override void End()
        {
            
        }
    }
}
