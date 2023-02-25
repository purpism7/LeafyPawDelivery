using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace GameSystem
{
    public class InputHandler : MonoBehaviour
    {
        private Camera _mainCamera = null;
        private System.DateTime _touchInterval;

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

            if((System.DateTime.UtcNow - _touchInterval).TotalSeconds < 0.1f)
            {
                return;
            }

            if (Input.touchCount <= 0)
            {
                return;
            }

            _touchInterval = System.DateTime.UtcNow;

            var touchPoint = Input.GetTouch(0).position;
            var ray = _mainCamera.ScreenPointToRay(touchPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                var collider = raycastHit.collider;
                if (collider == null)
                {
                    return;
                }

                var gameBase = collider.GetComponentInParent<Game.Base>();
                if(gameBase != null)
                {
                    gameBase.OnTouch();

                    return;
                }

                //var dropItem = raycastHit.collider.GetComponentInParent<Game.DropItem>();
                //if (dropItem != null)
                //{
                //    //activityArea.SelectActivityArea();

                //    Debug.Log(dropItem.name);

                //    return;
                //}

                //var activityArea = raycastHit.collider.GetComponentInParent<Game.ActivityArea>();
                //if(activityArea != null)
                //{
                //    activityArea.SelectActivityArea();

                //    return;
                //}
            }
        }
    }
}

