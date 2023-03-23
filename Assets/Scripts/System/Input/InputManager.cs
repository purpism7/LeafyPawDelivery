using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            int touchCnt = Input.touchCount;
            if (touchCnt <= 0)
            {
                return;
            }

            var touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            InputHandler?.ChainLateUpdate();
        }
    }
}

