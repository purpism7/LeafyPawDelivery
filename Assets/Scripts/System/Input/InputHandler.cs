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
            if (_gameCamera == null)
            {
                return;
            }

            if (GameManager.Instance.GameState.CheckControlCamera)
            {
                return;
            }

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
                            //GameSystem.GameManager.Instance.SetGameState<Game.State.Edit>();

                            
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

                        //_prevPos = touch.position;
                        //_prevPos = touch.position;
                        //GameSystem.GameManager.Instance.SetGameState<Game.State.Game>();
                    }                    
                    break;
            }
        }

        private void _Drag(Touch touch)
        {
            if (_gameBase != null)
            {
                var gameBaseTm = _gameBase.transform;

                float distance = _gameCamera.WorldToScreenPoint(gameBaseTm.position).z;
                Vector3 movePos = new Vector3(touch.position.x, touch.position.y, distance);
                Vector3 pos = _gameCamera.ScreenToWorldPoint(movePos);

                //gameBaseTm.DOLocalMove(movePos, 0);

                gameBaseTm.position = pos;

                _gameBase.OnTouch();
            }
        }
    }
}

