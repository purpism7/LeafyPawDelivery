using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class InputManager : GameSystem.Processing, IUpdater
    {
        #region Inspector
        public InputHandler InputHandler = null;
        public GameSystem.GameCameraController GameCameraCtr = null;
        public Grid grid = null;
        #endregion

        public override void Initialize()
        {
            GameCameraCtr?.Initialize(grid);
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            InputHandler?.Init(GameCameraCtr);

            yield return null;
        }

        #region IUpdate
        void IUpdater.ChainUpdate()
        {
#if UNITY_ANDROID
            if(Input.GetKey(KeyCode.Escape))
            {
                var quitGame = new PopupCreator<UI.QuitGame, UI.BaseData>()
                    .Create();
            }
#endif

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

