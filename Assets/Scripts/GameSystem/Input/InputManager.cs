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
#if !UNITY_EDITOR && UNITY_ANDROID
            if(Input.GetKey(KeyCode.Escape))
            {
                var quitGame = new PopupCreator<UI.QuitGame, UI.BaseData>()
                    .Create();
            }
#endif

            if (!HasPrimaryPointer())
                return;

            if (IsPointerOverUI())
                return;

            InputHandler?.ChainUpdate();
        }
        #endregion

        private bool HasPrimaryPointer()
        {
            if (Input.touchCount > 0)
                return true;

#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButton(0) ||
                   Input.GetMouseButtonDown(0) ||
                   Input.GetMouseButtonUp(0);
#else
            return false;
#endif
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject();
#else
            return false;
#endif
        }
    }
}
