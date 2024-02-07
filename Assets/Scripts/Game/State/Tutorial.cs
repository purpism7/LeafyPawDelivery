using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;
using static Game.Type;

namespace Game.State
{
    public class Tutorial : Base
    {
        //private TutorialManager _tutorialMgr = null;

        //public ETutorialStep ETutorialStep { get { return _tutorialMgr != null ? _tutorialMgr.ETutorialStep : ETutorialStep.None; } }

        public override void Initialize(MainGameManager mainGameMgr)
        {
            //if (mainGameMgr == null)
            //    return;

            //if (!mainGameMgr.tutorialRootTm)
            //    return;

            //_tutorialMgr = mainGameMgr.tutorialRootTm.gameObject.GetOrAddComponent<TutorialManager>();
        }

        public override void End()
        {

        }
    }
}
