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
        public Camera UICamera = null;
        
        private Vector2 _center = Vector2.zero;
        private Vector2 _mapSize = new Vector2(2000f, 2000f);
        private Vector3 _velocity = Vector3.zero;

        private Vector3 _prevPos = Vector3.zero;

        public float Height { get; private set; } = 0;
        public float Width { get; private set; } = 0;
        public Vector3 Center { get { return GameCamera != null ? GameCamera.transform.position + GameCamera.transform.forward : Vector3.zero; }  }

        public bool StopUpdate { get; private set; } = false;
        public IGridProvider IGridProvider { get; private set; } = null;

        private float _halfHeight = 0;

        public void Initialize(IGridProvider iGridProvider)
        {
            IGridProvider = iGridProvider;

            if (GameCamera != null)
            {
                _halfHeight = GameCamera.orthographicSize;
                Height = _halfHeight * 2f;
                Width = Height * GameCamera.aspect;
            }
        }

        private void Start()
        {
            
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
            if (GameCamera == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_center, _mapSize * 2f);
            Gizmos.DrawWireSphere(Center, 30f);

            Gizmos.color = Color.yellow;

            float height = GameCamera.orthographicSize * 2f;
            float width = height * GameCamera.aspect;

            Gizmos.DrawWireCube(Center, new Vector3(width - 100f, height - 700f));
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
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0) * Time.deltaTime * 50f;
                        var cameraTm = GameCamera.transform;
                        var movePos = cameraTm.position - pos;
                        
                        float x = _mapSize.x - Width;
                        float clampX = Mathf.Clamp(movePos.x, -x + _center.x, x + _center.x);

                        float y = _mapSize.y - _halfHeight;
                        float clampY = Mathf.Clamp(movePos.y, -y + _center.y, y + _center.y);

                        cameraTm.position = new Vector3(clampX, clampY, 0);
                        // cameraTm.Translate(pos);
                        // cameraTm.position = Vector3.SmoothDamp(cameraTm.position, new Vector3(clampX, clampY, 0), ref _velocity, 0.01f);

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        break;
                    }
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

