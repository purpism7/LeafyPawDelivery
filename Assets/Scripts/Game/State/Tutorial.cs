using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Tutorial : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            UIManager.Instance?.SetInteractable(false);
        }

        public override void End()
        {

        }
    }
}
