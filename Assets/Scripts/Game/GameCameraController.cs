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
        private Vector3 _velocity = Vector3.zero;

        private Vector3 _prevPos = Vector3.zero;
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
            if (GameCamera is null)
                return;
            
            if(StopUpdate)
                return;

            int touchCnt = Input.touchCount;
            if (touchCnt <= 0)
                return;

            var touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            Drag();
            // ZoomInOut();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_center, _mapSize * 2f);
            Gizmos.DrawWireSphere(GameCamera.transform.position + GameCamera.transform.forward,  30f);
        }

        private void Drag()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 1)
                return;
           
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    {

                    }
                    break;

                case TouchPhase.Moved:
                    {
                        var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0) * Time.deltaTime * 100f;
                        var cameraTm = GameCamera.transform;
                        var movePos = cameraTm.position - pos;
                        
                        float x = _mapSize.x - _width;
                        float clampX = Mathf.Clamp(movePos.x, -x + _center.x, x + _center.x);

                        float y = _mapSize.y - _height;
                        float clampY = Mathf.Clamp(movePos.y, -y + _center.y, y + _center.y);

                        cameraTm.position = new Vector3(clampX, clampY, 0);
                        // cameraTm.Translate(pos);
                        // cameraTm.position = Vector3.SmoothDamp(cameraTm.position, new Vector3(clampX, clampY, 0), ref _velocity, 0.01f);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        
                    }
                    break;
            }
        }

        private void ZoomInOut()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 2)
                return;
            
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

