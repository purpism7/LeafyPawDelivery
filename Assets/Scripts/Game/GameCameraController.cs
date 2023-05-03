    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GameSystem
{
    public class GameCameraController : MonoBehaviour
    {
        public Camera GameCamera = null;

        private Vector2 _center = Vector2.zero;
        private Vector2 _mapSize = new Vector2(2000f, 2000f);

        private Vector2 _prevPos = Vector2.zero;
        private float _height = 0;
        private float _width = 0;

        public bool StopUpdate { get; private set; } = false;

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

            if(StopUpdate)
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
                        //_prevPos = touch.position;
    
                        _prevPos = touch.position - touch.deltaPosition;
                        //_prevPos = GameCamera.ScreenToWorldPoint(touch.position);
                        //startTouchPosition = worldPoint;
                    }
                    break;

                case TouchPhase.Moved:
                    {
                        var nowPos = touch.position - touch.deltaPosition;
                        var movePos = (_prevPos - nowPos) * Time.deltaTime * 30f;

                        //var diffPos = _prevPos - GameCamera.ScreenToViewportPoint(touch.position);
                        //var movePos = (_prevPos - nowPos);
                        //
                        //cameraTm.position = Vector3.Lerp(cameraTm.position, movePos, Time.deltaTime * 10f);

                        //                         nowPos = touch.position - touch.deltaPosition;
                        //                     movePosDiff = (Vector2)(prePos - nowPos) * Time.deltaTime;
                        //                     prePos = touch.position - touch.deltaPosition;
                        //Debug.Log(touch.position);

                        var pos = GameCamera.transform.position + new Vector3(movePos.x, movePos.y, 0);

                        float x = _mapSize.x - _width;
                        float clampX = Mathf.Clamp(pos.x, -x + _center.x, x + _center.x);

                        float y = _mapSize.y - _height;
                        float clampY = Mathf.Clamp(pos.y, -y + _center.y, y + _center.y);

                        GameCamera.transform.position = new Vector3(clampX, clampY, 0);
                        //GameCamera.transform.Translate(position);

                        _prevPos = touch.position - touch.deltaPosition;


                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        //_prevPos = GameCamera.transform.position;

                        //_prevPos = touch.position;


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

        public void SetStopUpdate(bool stopUpdate)
        {
            StopUpdate = stopUpdate;
        }
    }
}

