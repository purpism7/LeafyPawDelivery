using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public interface IGameCameraCtr
    {
        float MaxOrthographicSize { get; }
        float DefaultOrthographicSize { get; }
        float OrthographicSizeForTutorial { get; }

        Vector3 Center { get; }
        float GameCameraWidth { get; }

        void SetSize();
        void SetOrthographicSize(float orthographicSize);
        void MoveCenterGameCamera();
        void SetStopUpdate(bool stopUpdate);

        float RandPosXInScreenRagne { get; }
        float RandPosYInScreenRagne { get; }
    }

    public class GameCameraController : MonoBehaviour, IFixedUpdater, IGameCameraCtr
    {
        private const float InitPosZ = -2000f;

        public Camera GameCamera = null;
        public Camera UICamera = null;
        [SerializeField]
        private AnimationCurve moveCurve = null;

        private Vector2 _center = Vector2.zero;
        private Vector2 _mapSize = new Vector2(2000f, 2000f);

        private float _halfHeight = 0;
        private float _dragWidth = 0;
        private Vector3 _velocity = Vector3.zero;
        private float _smoothTime = 0.025f;

        private float _width = 0;
        //private float _moveDeltaTime = 0;
        //private float _moveLerpTime = 10f;
        
        public float Height { get; private set; } = 0;
       
        public IGrid IGrid { get; private set; } = null;
        public bool StopUpdate { get; private set; } = false;

        public float MaxOrthographicSize { get { return 2000f; } }
        public float MinOrthographicSize { get { return 1100f; } }

        public float OrthographicSizeForTutorial { get { return 1200; } }
        public float DefaultOrthographicSize { get { return 1500f; } }

        public Vector3 Center
        {
            get
            {
                if(GameCamera != null)
                {
                    var pos = GameCamera.transform.position + GameCamera.transform.forward;
                    pos.y = IGrid != null ? IGrid.LimitPosY(pos.y) : pos.y;

                    return pos;
                }
                
                return Vector3.zero;
            }
        }

        public void Initialize(IGrid iGrid)
        {
            IGrid = iGrid;

            if(Application.isEditor)
            {
                _smoothTime = 0.01f;
            }

            SetSize();
        }

        #region IFixedUpdater
        void IFixedUpdater.ChainFixedUpdate()
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

            Gizmos.DrawWireCube(Center, new Vector3(width - 250f, height - 850f));
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

        private void SetSize()
        {
            if (GameCamera == null)
                return;

            _halfHeight = GameCamera.orthographicSize;
            Height = _halfHeight * 2f;
            _width = Height * GameCamera.aspect;

            _dragWidth = _halfHeight * Screen.width / Screen.height;
        }

        private void SetOrthographicSize(float orthographicSize, float timeOffset = 1f, bool isLerp = true)
        {
            var resOrthographicSize = Mathf.Clamp(orthographicSize, MinOrthographicSize, MaxOrthographicSize);

            if(isLerp)
            {
                GameCamera.orthographicSize = Mathf.Lerp(GameCamera.orthographicSize, resOrthographicSize, Time.deltaTime * timeOffset);
            }
            else
            {
                GameCamera.orthographicSize = resOrthographicSize;
            }
        }

        private float GetClampX(float posX)
        {
            float x = _mapSize.x - _dragWidth;

            return Mathf.Clamp(posX, -x + _center.x, x + _center.x);
        }

        private float GetClampY(float posY)
        {
            float y = _mapSize.y - _halfHeight;

            return Mathf.Clamp(posY, -y + _center.y, y + _center.y);
        }

        private void Move(Touch touch)
        {
            var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0);
            var cameraTm = GameCamera.transform;
            var movePos = cameraTm.position - pos;

            float clampX = GetClampX(movePos.x);
            float clampY = GetClampY(movePos.y);

            var targetPos = new Vector3(clampX, clampY, InitPosZ);

            cameraTm.position = Vector3.SmoothDamp(cameraTm.position, targetPos, ref _velocity, moveCurve.Evaluate(_smoothTime));
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
            
            SetOrthographicSize(GameCamera.orthographicSize + deltaMagnitudeDiff, 12f);

            SetSize();
            
            var cameraTm = GameCamera.transform;

            float clampX = GetClampX(cameraTm.position.x);
            float clampY = GetClampY(cameraTm.position.y);
            var targetPos = new Vector3(clampX, clampY, InitPosZ);
            cameraTm.position = targetPos;
        }

        public void SetStopUpdate(bool stopUpdate)
        {
            StopUpdate = stopUpdate;
        }

        #region IGameCameraCtr
        void IGameCameraCtr.SetSize()
        {
            SetSize();
        }

        void IGameCameraCtr.SetOrthographicSize(float orthographicSize)
        {
            SetOrthographicSize(orthographicSize, 1f, false);
        }

        void IGameCameraCtr.MoveCenterGameCamera()
        {
            GameCamera.transform.position = new Vector3(0, 0, InitPosZ);
        }

        float IGameCameraCtr.GameCameraWidth
        {
            get
            {
                return _width;
            }
        }

        float IGameCameraCtr.RandPosXInScreenRagne
        {
            get
            {
                var center = Center;
                var halfWidth = (_width - 250f) / 2f;

                var randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);
                Random.InitState((int)randomX);

                return randomX;
            }
        }

        float IGameCameraCtr.RandPosYInScreenRagne
        {
            get
            {
                var center = Center;
                var halfHeight = (Height - 850f) / 2f;

                var randomY = Random.Range(center.y - halfHeight, center.y + halfHeight);
                Random.InitState((int)randomY);

                return IGrid.LimitPosY(randomY);
            }
        }
        #endregion
    }
}

