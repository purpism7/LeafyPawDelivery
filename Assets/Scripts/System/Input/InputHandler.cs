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

        private Game.Object _object = null; 

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

            if (!GameManager.Instance.GameState.CheckEditObject)
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

                if(_object == null)
                {
                    _object = collider.GetComponentInParent<Game.Object>();
                    _object?.Apply(new Game.Edit());
                }
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {

                    }
                    break;

                case TouchPhase.Moved:
                    {
                        DragObject(touch);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        //_object?.Apply(new Game.Arrange());
                        _object = null;
                    }                    
                    break;
            }
        }

        private void DragObject(Touch touch)
        {
            if (_object == null)
            {
                return;
            }

            var objectTm = _object.transform;

            float distance = _gameCamera.WorldToScreenPoint(objectTm.position).z;
            Vector3 movePos = new Vector3(touch.position.x, touch.position.y, distance);
            Vector3 pos = _gameCamera.ScreenToWorldPoint(movePos);

            objectTm.position = pos;

            //_object.Apply(new Game.Arrange(objectTm.position));
        }
    }
}

