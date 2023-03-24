using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        private Camera _gameCamera = null;
        private System.DateTime _touchInterval;
        private Vector2 _prevPos = Vector2.zero;

        private Game.Base _gameBase = null;

        public void Init(Camera gameCamera)
        {
            _gameCamera = gameCamera;
        }

        public void ChainUpdate()
        {
            UpdateTouch();
        }

        private void UpdateTouch()
        {
            if(_gameCamera == null)
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
            var ray = _gameCamera.ScreenPointToRay(touchPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                var collider = raycastHit.collider;
                if (collider == null)
                {
                    return;
                }

                if(_gameBase == null)
                {
                    _gameBase = collider.GetComponentInParent<Game.Base>();
                }
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (_gameBase != null)
                        {
                            GameSystem.GameManager.Instance.SetGameState<Game.State.Edit>();

                            _prevPos = touch.position;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        _Drag(touch);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        _gameBase = null;

                        GameSystem.GameManager.Instance.SetGameState<Game.State.Game>();
                    }                    
                    break;
            }
        }

        private void _Drag(Touch touch)
        {
            if (GameManager.Instance.GameState.CheckControlCamera)
            {
                return;
            }

            Vector3 movePos = touch.position - _prevPos;
            movePos.z = 350f;

            if (_gameBase != null)
            {
                var gameBaseTm = _gameBase.transform;

                
                gameBaseTm.DOMove(movePos, 0);

                //gameBaseTm.localPosition = movePos;

                _gameBase.OnTouch();
            }
        }
    }
}

