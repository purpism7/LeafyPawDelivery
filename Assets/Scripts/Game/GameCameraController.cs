    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GameSystem
{
    public class GameCameraController : MonoBehaviour, IUpdater
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
        private float _dragWidth = 0;

        public void Initialize(IGridProvider iGridProvider)
        {
            IGridProvider = iGridProvider;

            if (GameCamera != null)
            {
                SetSize();
            }
        }

        private void SetSize()
        {
            _halfHeight = GameCamera.orthographicSize;
            Height = _halfHeight * 2f;
            Width = Height * GameCamera.aspect;

            _dragWidth = _halfHeight * Screen.width / Screen.height;
        }

        public void ChainFixedUpdate()
        {

        }

        #region IUpdate
        void IUpdater.ChainUpdate()
        {
            if (GameCamera is null)
                return;

            if (StopUpdate)
                return;

            int touchCnt = Input.touchCount;
            if (touchCnt <= 0)
                return;

            var touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            Drag();
        }
        #endregion

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

            Gizmos.DrawWireCube(Center, new Vector3(width - 200f, height - 850f));
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
                        
                        float x = _mapSize.x - _dragWidth;
                        float clampX = Mathf.Clamp(movePos.x, -x + _center.x, x + _center.x);

                        float y = _mapSize.y - _halfHeight;
                        float clampY = Mathf.Clamp(movePos.y, -y + _center.y, y + _center.y);

                        var targetPos = new Vector3(clampX, clampY, -1000f);


                        //cameraTm.position = Vector3.Lerp(cameraTm.position, targetPos, Time.deltaTime * 100f);
                        cameraTm.position = new Vector3(clampX, clampY, -1000f);
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
            Debug.Log("res = " + res);
            GameCamera.orthographicSize += res * 1f;
            Mathf.Clamp(GameCamera.orthographicSize, 1000, 2000);
        }

        public void SetStopUpdate(bool stopUpdate)
        {
            StopUpdate = stopUpdate;
        }
    }
}

