using System;
using UnityEngine;

namespace UI.WorldUI
{
    public class AnimationWater : StateMachineBehaviour
    {
        public Action ExitAction = null;
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExitAction?.Invoke();
        }
    }
}