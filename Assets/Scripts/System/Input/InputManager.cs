using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class InputManager : GameSystem.Processing
    {
        public Camera MainCamera = null;
        public InputHandler InputHandler = null;

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            InputHandler.Init(MainCamera);

            yield return null;
        }

        private void LateUpdate()
        {
            InputHandler?.ChainLateUpdate();
        }
    }
}

