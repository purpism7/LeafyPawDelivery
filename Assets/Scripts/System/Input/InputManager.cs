using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class InputManager : GameSystem.Processing, IUpdate
    {
        #region Inspector
        public GameSystem.GameCameraController GameCameraCtr = null;
        public InputHandler InputHandler = null;

        [SerializeField]
        private Grid grid = null;
        #endregion

        public override void Initialize()
        {
            GameCameraCtr?.Initialize(grid);
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            InputHandler?.Init(GameCameraCtr, grid);

            yield return null;
        }

        #region IUpdate
        void IUpdate.ChainUpdate()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt <= 0)
                return;

            var touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            InputHandler?.ChainUpdate();
        }
        #endregion
    }
}

