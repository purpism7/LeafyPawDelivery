using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        private Camera _mainCamera = null;
        private System.DateTime _touchInterval;
        private Vector2 _prevPos = Vector2.zero;

        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        public void ChainLateUpdate()
        {
            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if(_mainCamera == null)
            {
                return;
            }

            //if((System.DateTime.UtcNow - _touchInterval).TotalSeconds < 0.1f)
            //{
            //    return;
            //}

            _touchInterval = System.DateTime.UtcNow;

            var touch = Input.GetTouch(0);
            var touchPoint = touch.position;
            var ray = _mainCamera.ScreenPointToRay(touchPoint);

            Game.Base gameBase = null;

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                var collider = raycastHit.collider;
                if (collider == null)
                {
                    return;
                }

                gameBase = collider.GetComponentInParent<Game.Base>();
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (gameBase != null)
                        {
                            GameSystem.GameManager.Instance.SetGameState<Game.State.Edit>();

                            _prevPos = touch.position  - touch.deltaPosition;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        if (GameManager.Instance.GameState.CheckControlCamera)
                        {
                            return;
                        }

                        if(gameBase != null)
                        {
                            var nowPos = touch.position - touch.deltaPosition;
                            var movePos = _prevPos - nowPos;
                            var gamebaseTm = gameBase.transform;
                            Debug.Log(movePos);
                            gamebaseTm.localPosition = -movePos;

                            gameBase.OnTouch();
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        GameSystem.GameManager.Instance.SetGameState<Game.State.Game>();
                    }                    
                    break;
            }
        }
    }
}

