using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(GameCamera == null)
            {
                return;
            }

            Drag();
            ZoomInOut();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_center, _mapSize * 2);
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
                        _prevPos = touch.position - touch.deltaPosition;
                        //_prevPos = GameCamera.transform.position;
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        var nowPos = touch.position - touch.deltaPosition;
                        //Debug.Log(touch.deltaPosition);
                        var movePos = _prevPos - nowPos;
                        //Debug.Log(GameCamera.transform.position + " / " + movePos);
                        GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, movePos, Time.deltaTime * 10f);
                        //GameCamera.transform.Translate(movePos);
                        //GameCamera.transform.position = movePos;
                        //_prevPos = touch.position - touch.deltaPosition;

                        float lx = _mapSize.x - _width;
                        float clampX = Mathf.Clamp(GameCamera.transform.position.x, -lx + _center.x, lx + _center.x);

                        float ly = _mapSize.y - _height;
                        float clampY = Mathf.Clamp(GameCamera.transform.position.y, -ly + _center.y, ly + _center.y);

                        GameCamera.transform.position = new Vector3(clampX, clampY, -10f);
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
        }
    }
}

