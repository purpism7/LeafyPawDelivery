using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Enter : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            // 진입 연출 전 Deactivate Top, Bottom 
            UIManager.Instance?.DeactivateAnim();
        }

        public override void End()
        {
            
        }
    }
}
