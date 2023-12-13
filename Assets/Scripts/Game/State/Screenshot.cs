using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Screenshot : Base
    {
        public override void Initialize(MainGameManager mainGameMgr)
        {
            var screenshot = new GameSystem.PopupCreator<UI.Screenshot, UI.BaseData>()
                .SetShowBackground(false)
                .Create();
        }
    }
}
