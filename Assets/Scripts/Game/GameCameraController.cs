using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;
using DG.Tweening;

using Game;
using UnityEditor.Rendering;
using Random = UnityEngine.Random;

namespace GameSystem
{
    public interface IGameCameraCtr
    {
        float MaxOrthographicSize { get; }
        float DefaultOrthographicSize { get; }
        float OrthographicSizeForTutorial { get; }

        Vector3 Center { get; }
        float GameCameraWidth { get; }

        void SetOrthographicSize(float orthographicSize);
        void MoveCenterGameCamera();
        UniTask MoveCenterGameCameraAsync();
        void SetStopUpdate(bool stopUpdate);

        float RandPosXInScreenRagne { get; }
        float RandPosYInScreenRagne { get; }

        Vector3 ScreenToWorldPoint(Vector3 pos);
        Vector3 WorldToScreenPoint(Vector3 pos);

        void SetPositionUICamera(bool origin, Vector3 pos);

        void ZoomIn(Vector3 targetPos, System.Action endAction);
        void ZoomOut(System.Action endAction);

        void SetConfinerBoundingShape(Collider2D collider);
    }

    public class GameCameraController : MonoBehaviour, IFixedUpdater, IGameCameraCtr
    {
        private const float InitPosZ = -10000f;

        public Camera GameCamera = null;
        public Camera UICamera = null;
        // [SerializeField]
        // private AnimationCurve moveCurve = null;
        [SerializeField] 
        private CinemachineVirtualCamera virtualCamera = null;

        private readonly Vector3 _originUICameraPos = new Vector3(3000f, 0, 0);
        private readonly Vector2 _center = Vector2.zero;
        private readonly Vector2 _mapSize = new Vector2(2000f, 2000f);

        private float _halfHeight = 0;
        // private float _dragWidth = 0;
        private Vector3 _velocity = Vector3.zero;
        private float _smoothTime = 0.045f;
        //private float _smoothTime = 1f;

        private float _width = 0;
        private bool _zoomIn = false;
        private Vector3 _zoomInTargetPos = Vector3.zero;
        private System.Action _zoomInEndAction = null; 
        private float _timeElapsed = 0;

        private bool _isMoveCenter = false;
        
        public float Height { get; private set; } = 0;
       
        public IGrid IGrid { get; private set; } = null;
        public bool StopUpdate { get; private set; } = false;

        public float MaxOrthographicSize { get { return 2000f; } }
        public float MinOrthographicSize { get { return 1200f; } }

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
            _isMoveCenter = false;
            _zoomIn = false;
            
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

#if UNITY_EDITOR
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

            Gizmos.DrawWireCube(Center, new Vector3(width - 300f, height - 850f));
        }
#endif

        private void LateUpdate()
        {
            if (_isMoveCenter)
            {
                var centerPos = new Vector3(0, 0, InitPosZ);
                GameCamera.transform.position = centerPos;

                var distance = Vector3.Distance(GameCamera.transform.position, centerPos);
                Debug.Log(distance);
                if (distance <= 0.1f)
                    _isMoveCenter = false;

                return;
            }
            
            if (_zoomIn)
            {
                float duration = 1.5f;
            
                _timeElapsed += Time.deltaTime;
                var t = _timeElapsed / duration;
                
                GameCamera.transform.position =
                    Vector3.Lerp(GameCamera.transform.position, _zoomInTargetPos, t);
                
                if (t >= duration)
                {
                    _zoomIn = false;
                    _zoomInEndAction?.Invoke();
                    _zoomInEndAction = null;
                }

                return;
            }
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
        
            // _dragWidth = _halfHeight * Screen.width / Screen.height;
        }

        private void SetOrthographicSize(float orthographicSize)
        {
            var resOrthographicSize = Mathf.Clamp(orthographicSize, MinOrthographicSize, MaxOrthographicSize);
            //
            // if(isLerp)
            // {
            //     GameCamera.orthographicSize = Mathf.Lerp(GameCamera.orthographicSize, resOrthographicSize, Time.deltaTime * timeOffset);
            // }
            // else
            // {
            //     GameCamera.orthographicSize = resOrthographicSize;
            // }

            virtualCamera.m_Lens.OrthographicSize = resOrthographicSize;
            // virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize,
            //     resOrthographicSize, Time.deltaTime);
        }

        // private float GetClampX(float posX)
        // {
        //     float x = _mapSize.x - _dragWidth;
        //
        //     return Mathf.Clamp(posX, -x + _center.x, x + _center.x);
        // }
        //
        // private float GetClampY(float posY)
        // {
        //     float y = _mapSize.y - _halfHeight;
        //
        //     return Mathf.Clamp(posY, -y + _center.y, y + _center.y);
        // }

        private void Move(Touch touch)
        {
            var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0);
            var cameraTm = GameCamera.transform;
            var targetPos = cameraTm.position - pos;
            targetPos.z = InitPosZ;
            
            
            // float clampX = GetClampX(movePos.x);
            // float clampY = GetClampY(movePos.y);

            // var targetPos = new Vector3(clampX, clampY, InitPosZ);

            cameraTm.position = Vector3.SmoothDamp(cameraTm.position, targetPos, ref _velocity, _smoothTime * 0.7f);
            // cameraTm.position = Vector3.Lerp(cameraTm.position, targetPos, Time.deltaTime * 2f);
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
            
            SetOrthographicSize(GameCamera.orthographicSize + deltaMagnitudeDiff);

            SetSize();
            //
            // var cameraTm = GameCamera.transform;
            //
            // float clampX = GetClampX(cameraTm.position.x);
            // float clampY = GetClampY(cameraTm.position.y);
            // var targetPos = new Vector3(clampX, clampY, InitPosZ);
            // cameraTm.position = targetPos;
        }

        public void SetStopUpdate(bool stopUpdate)
        {
            StopUpdate = stopUpdate;
        }

        #region IGameCameraCtr
        void IGameCameraCtr.SetOrthographicSize(float orthographicSize)
        {
            SetOrthographicSize(orthographicSize);
        }

        void IGameCameraCtr.MoveCenterGameCamera()
        {
            // _isMoveCenter = true;
            // Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, GameCamera.nearClipPlane);
            // Vector3 worldCenter = GameCamera.ScreenToWorldPoint(screenCenter);
            // worldCenter.z = InitPosZ;

            // GameCamera.transform.position = new Vector3(0, 0, InitPosZ);
            GameCamera.transform.DOMove(new Vector3(0, 0, InitPosZ), 1f);
        }
        
        public async UniTask MoveCenterGameCameraAsync()
        {
            await GameCamera.transform.DOMove(new Vector3(0, 0, InitPosZ), 1f);
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
                // var halfWidth = (_width - 300f) / 2f;
                
                float height = GameCamera.orthographicSize * 2f;
                float width = height * GameCamera.aspect - 300f;
                var halfWidth = width * 0.5f;
                var randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);

                return randomX;
            }
        }

        float IGameCameraCtr.RandPosYInScreenRagne
        {
            get
            {
                var center = Center;
                // var halfHeight = (Height - 850f) / 2f;
                
                float height = GameCamera.orthographicSize * 2f - 850f;
                // float width = height * GameCamera.aspect;
                var halfHeight = height * 0.5f;
                var randomY = Random.Range(center.y - halfHeight, center.y + halfHeight);

                return IGrid.LimitPosY(randomY);
            }
        }

        Vector3 IGameCameraCtr.ScreenToWorldPoint(Vector3 pos)
        {
            if (UICamera == null)
                return pos;

            return UICamera.ScreenToWorldPoint(pos);
        }
        
        Vector3 IGameCameraCtr.WorldToScreenPoint(Vector3 pos)
        {
            if (GameCamera == null)
                return pos;

            RectTransform rectTm = UIManager.Instance?.UIRootRectTm;
            if (!rectTm)
                return pos;
    
            Vector2 viewportPos= GameCamera.WorldToViewportPoint(pos);
            
            return new Vector2(
                (viewportPos.x * rectTm.sizeDelta.x) - (rectTm.sizeDelta.x * 0.5f),
                (viewportPos.y * rectTm.sizeDelta.y) - (rectTm.sizeDelta.y * 0.5f));
        }

        void IGameCameraCtr.SetPositionUICamera(bool origin, Vector3 pos)
        {
            var uiCameraTm = UICamera?.GetComponent<Transform>();
            if (!uiCameraTm)
                return;
            
            if (origin)
            {
                UICamera.GetComponent<Transform>().position = _originUICameraPos;
            }
            else
            {
                UICamera.GetComponent<Transform>().position = pos;
            }
        }

        void IGameCameraCtr.ZoomIn(Vector3 targetPos, System.Action endAction)
        {
            if (GameCamera == null)
                return;
            
            StopUpdate = true;

            _timeElapsed = 0;
            _zoomInTargetPos = new Vector3(targetPos.x, targetPos.y, InitPosZ);
            _zoomInEndAction = endAction;
            _zoomIn = true;

            var distance = virtualCamera.m_Lens.OrthographicSize - 500f;
            Debug.Log(distance);
            float duration = distance * 0.001f;
            Debug.Log(duration);
            
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, size => virtualCamera.m_Lens.OrthographicSize = size, 500f, duration).SetEase(Ease.Linear);

            // var resTargetPos = GameCamera.transform.TransformPoint(targetPos);//GameCamera.WorldToScreenPoint(targetPos); 
            // var resTargetPos = 
            
            // float duration = 2f;
            //
            // Sequence sequence = DOTween.Sequence()
            //     .SetAutoKill(false)
            //     .Append(virtualCamera.transform.DOMove(resTargetPos, duration).SetEase(Ease.OutQuad))
            //     // .Append(DOTween.To(() => GameCamera.transform.position, pos => GameCamera.transform.position = pos, endPos, duration).SetEase(Ease.OutQuad))
            //     .Join(DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, size => virtualCamera.m_Lens.OrthographicSize = size, 500f, duration).SetEase(Ease.OutQuad))
            //     .OnComplete(() =>
            //     {
            //         endAction?.Invoke();
            //     });
            // sequence.Restart();

            // ZoomInAsync(targetPos, endAction).Forget();
        }

        // private async UniTask ZoomInAsync(Vector3 targetPos, System.Action endAction)
        // {
        //     float duration = 1f;
        //     
        //     
        //
        //     float timeElapsed = 0;
        //     var resTargetPos = new Vector3(targetPos.x, targetPos.y, InitPosZ);
        //     
        //     while (Vector3.Distance(resTargetPos, GameCamera.transform.position) >= 0.1f)
        //     {
        //         timeElapsed += Time.deltaTime;
        //         var t = timeElapsed / duration;
        //         
        //         GameCamera.transform.position =
        //             Vector3.Lerp(GameCamera.transform.position, resTargetPos, t);
        //         // Vector3.MoveTowards(GameCamera.transform.position, resTargetPos, Time.deltaTime);
        //         
        //         await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        //     }
        //
        //     await UniTask.Yield();
        //     
        //     endAction?.Invoke();
        // }

        void IGameCameraCtr.ZoomOut(System.Action endAction)
        {
            float duration = 0.5f;
            
            StopUpdate = false;
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, size => virtualCamera.m_Lens.OrthographicSize = size, DefaultOrthographicSize, duration).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    endAction?.Invoke();
                    
                    SetSize();
                });
            sequence.Restart();
        }

        void IGameCameraCtr.SetConfinerBoundingShape(Collider2D collider)
        {
            var confiner = virtualCamera?.GetComponent<CinemachineConfiner>();
            if (confiner == null)
                return;

            confiner.m_BoundingShape2D = collider;
        }
        #endregion
    }
}

