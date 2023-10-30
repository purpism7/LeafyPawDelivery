    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GameSystem
{
    public class GameCameraController : MonoBehaviour, IUpdater
    {
        private const float MaxOrthographicSize = 1800f;
        private const float MinOrthographicSize = 1200f;

        public Camera GameCamera = null;
        public Camera UICamera = null;
        
        private Vector2 _center = Vector2.zero;
        private Vector2 _mapSize = new Vector2(2000f, 2000f);

        private Vector3 _prevPos = Vector3.zero;

        private float _halfHeight = 0;
        private float _dragWidth = 0;

        public float Height { get; private set; } = 0;
        public float Width { get; private set; } = 0;
        public Vector3 Center { get { return GameCamera != null ? GameCamera.transform.position + GameCamera.transform.forward : Vector3.zero; } }
        public IGrid IGrid { get; private set; } = null;
        public bool StopUpdate { get; private set; } = false;

        public void Initialize(IGrid iGrid)
        {
            IGrid = iGrid;

            SetSize();
        }

        private void SetSize()
        {
            if (GameCamera == null)
                return;

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

            ZoomInOut();
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
                        Move(touch);

                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        break;
                    }
            }
        }

        private void Move(Touch touch)
        {
            var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0);
            var cameraTm = GameCamera.transform;
            var movePos = cameraTm.position - pos;

            float x = _mapSize.x - _dragWidth;
            float clampX = Mathf.Clamp(movePos.x, -x + _center.x, x + _center.x);

            float y = _mapSize.y - _halfHeight;
            float clampY = Mathf.Clamp(movePos.y, -y + _center.y, y + _center.y);

            var time = Time.deltaTime * 50f;

            var targetPos = new Vector3(clampX, clampY, -1000f);
            cameraTm.position = Vector3.Lerp(cameraTm.position, targetPos, time);
        }

        private void ZoomInOut()
        {
            int touchCnt = Input.touchCount;
            if (touchCnt != 2)
                return;
            
            var firTouch = Input.GetTouch(0);
            var secTouch = Input.GetTouch(1);

            var firPrevPos = firTouch.position - firTouch.deltaPosition;
            var secPrevPos = secTouch.position - secTouch.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            float prevTouchDeltaMag = (firPrevPos - secPrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            float touchDeltaMag = (firTouch.position - secTouch.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            //Debug.Log("res = " + res);
            GameCamera.orthographicSize += deltaMagnitudeDiff * 0.5f;
            GameCamera.orthographicSize = Mathf.Clamp(GameCamera.orthographicSize, MinOrthographicSize, MaxOrthographicSize);

            SetSize();
            // 카메라 영역 제한하는 부분 다시 체크 할 것. (그대로 쓸 것인지 여부.)
            Move(firTouch);
        }

        public void SetStopUpdate(bool stopUpdate)
        {
            StopUpdate = stopUpdate;
        }
    }
}

