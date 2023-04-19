using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GameSystem
{
    public class CameraController : MonoBehaviour
    {
        public Camera GameCamera = null;

        private Vector2 _center = Vector2.zero;
        private Vector2 _mapSize = new Vector2(2000f, 2000f);

        private Vector2 _prevPos = Vector2.zero;
        private float _height = 0;
        private float _width = 0;

        private void Start()
        {
            if(GameCamera != null)
            {
                _height = GameCamera.orthographicSize;
                _width = _height * Screen.width / Screen.height;
            }
        }

        private void FixedUpdate()
        {
            if (GameCamera == null)
            {
                return;
            }

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

            Drag();
            ZoomInOut();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_center, _mapSize * 2f);
        }

        private void Drag()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 1)
            {
                return;
            }

            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    {
                        _prevPos = touch.position;
                        //_prevPos = GameCamera.transform.position;
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        var nowPos = touch.position - touch.deltaPosition;
                        //var movePos = _prevPos - touch.position;
                        var cameraTm = GameCamera.transform;

                        var movePos = touch.position - _prevPos;

                        //Debug.Log(GameCamera.transform.position + " / " + movePos);
                        cameraTm.position = Vector3.Lerp(cameraTm.position, movePos, Time.deltaTime * 10f);
                        //cameraTm.DOMove(movePos, 1f);

                        float x = _mapSize.x - _width;
                        float clampX = Mathf.Clamp(cameraTm.position.x, -x + _center.x, x + _center.x);

                        float y = _mapSize.y - _height;
                        float clampY = Mathf.Clamp(cameraTm.position.y, -y + _center.y, y + _center.y);

                        cameraTm.position = new Vector3(clampX, clampY, -10f);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        //_prevPos = GameCamera.transform.position;
                    }
                    break;
            }
        }

        private void ZoomInOut()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 2)
            {
                return;
            }

            var firTouch = Input.GetTouch(0);
            var secTouch = Input.GetTouch(1);

            var firMag = (firTouch.position - firTouch.deltaPosition).sqrMagnitude;
            var secMag = (secTouch.position - secTouch.deltaPosition).sqrMagnitude;

            float res = firMag - secMag;

            GameCamera.orthographicSize += res * 1f;
        }
    }
}

