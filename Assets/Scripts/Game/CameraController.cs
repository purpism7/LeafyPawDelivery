using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class CameraController : MonoBehaviour
    {
        public Camera GameCamera = null;

        private Vector2 _prevPos = Vector2.zero;

        private void LateUpdate()
        {
            if(GameCamera == null)
            {
                return;
            }

            LateUpdateDragCamera();
        }

        private void LateUpdateDragCamera()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 1)
            {
                return;
            }

            var touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                _prevPos = touch.position - touch.deltaPosition;
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                var nowPos = touch.position - touch.deltaPosition;
                var movePos = _prevPos - nowPos;

                GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, movePos, 1f * Time.deltaTime);

                //_prevPos = touch.position - touch.deltaPosition;
            }
        }
    }
}

