using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class InputManager : GameSystem.Processing
    {
        #region Inspector
        public GameSystem.GameCameraController GameCameraCtr = null;
        public InputHandler InputHandler = null;

        [SerializeField]
        private Grid grid = null;
        #endregion

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            InputHandler?.Init(GameCameraCtr, grid);

            yield return null;
        }

        private void Update()
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

            InputHandler?.ChainUpdate();
        }
    }
}

