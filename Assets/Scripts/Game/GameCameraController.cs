using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;
using DG.Tweening;

using Game;
using Random = UnityEngine.Random;

namespace GameSystem
{
    public interface IGameCameraCtr
    {
        Camera MainCamera { get; }
        Camera UICamera { get; }
        
        float MaxOrthographicSize { get; }
        float DefaultOrthographicSize { get; }
        float OrthographicSizeForTutorial { get; }

        Vector3 Center { get; }
        float GameCameraWidth { get; }

        void SetOrthographicSize(float orthographicSize);
        // void MoveCenterGameCamera();
        UniTask MoveCenterGameCameraAsync();
        void SetStopUpdate(bool stopUpdate);

        float RandPosXInScreenRagne { get; }
        float RandPosYInScreenRagne { get; }

        Vector3 ScreenToWorldPoint(Vector3 pos);
        Vector3 WorldToScreenPoint(Vector3 pos);

        void SetPositionUICamera(bool origin, Vector3 pos);

        void ZoomIn(Transform targetTr, System.Action endAction);
        void ZoomOut(System.Action endAction);

        void SetConfinerBoundingShape(Collider2D collider);
    }

    public class GameCameraController : MonoBehaviour, IFixedUpdater, IGameCameraCtr
    {
        private const float InitPosZ = -10000f;

        [SerializeField] private Camera uiCamera = null;
        [SerializeField] private CinemachineCamera cinemachineCamera = null;
        [SerializeField] private CinemachineCamera follwCamera = null;
        // [SerializeField] private Transform targetTr = null;
        
        public Camera GameCamera = null;
        
        private readonly Vector3 _originUICameraPos = new Vector3(3000f, 0, 0);
        private readonly Vector2 _center = Vector2.zero;
        private readonly Vector2 _mapSize = new Vector2(2000f, 2000f);

        private Vector3 _lastWorldPosition;
        private Vector2 _lastTouchPosition;
        private bool _isDragging = false;
        private float _halfHeight = 0;
        // private float _dragWidth = 0;
        private Vector3 _velocity = Vector3.zero;
        private float _smoothTime = 0.045f;
        //private float _smoothTime = 1f;

        private float _width = 0;
        private bool _zoomIn = false;
        // private Vector3 _zoomInTargetPos = Vector3.zero;
        private System.Action _zoomInEndAction = null; 
        private float _timeElapsed = 0;

        private bool _isMoveCenter = false;

        public Camera MainCamera => GameCamera;
        public Camera UICamera => uiCamera;

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
                if(cinemachineCamera != null)
                {
                    var pos = cinemachineCamera.transform.position + cinemachineCamera.transform.forward;
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

            follwCamera?.SetActive(false);
            
            SetSize();
            SetStopUpdate(true);
        }

        private void LateUpdate()
        {
            if (GameCamera is null)
                return;

            if (StopUpdate)
                return;

            // 에디터 시뮬레이터에서는 터치 입력도 처리
            int touchCnt = Input.touchCount;
            if (touchCnt > 0)
            {
                var touch = Input.GetTouch(0);
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                ZoomInOut();
                Drag();
            }
#if UNITY_EDITOR
            // 에디터에서 터치가 없을 때만 마우스 입력 처리
            else
            {
                // 마우스 드래그 처리
                if (Input.GetMouseButton(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;

                    DragEditor();
                }
                
                // 마우스 휠 줌인/줌아웃 처리
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;

                    ZoomInOutEditor(scroll);
                }
            }
#endif
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

            // ZoomInOut();
            // Drag();
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

            float height = cinemachineCamera.Lens.OrthographicSize * 2f;
            float width = height * cinemachineCamera.Lens.Aspect;

            Gizmos.DrawWireCube(Center, new Vector3(width - 300f, height - 850f));
        }
#endif

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
                        // 드래그 시작 시 Cinemachine 비활성화
                        if (cinemachineCamera != null && !_isDragging)
                        {
                            cinemachineCamera.SetActive(false);
                            _isDragging = true;
                        }
                        
                        _lastTouchPosition = touch.position;
                        var screenPosition = new Vector3(touch.position.x, touch.position.y, 0);
                        _lastWorldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
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
                        // 드래그 종료 시에도 Cinemachine은 활성화하지 않음
                        // (다음 드래그 시작 시 다시 비활성화하므로 상태 유지)
                        if (_isDragging)
                        {
                            _isDragging = false;
                        }
                        break;
                    }
            }
        }

        private void SetSize()
        {
            if (cinemachineCamera == null)
                return;
        
            _halfHeight = cinemachineCamera.Lens.OrthographicSize;
            Height = _halfHeight * 2f;
            _width = Height * cinemachineCamera.Lens.Aspect;
        
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

            cinemachineCamera.Lens.OrthographicSize = resOrthographicSize;
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
            // Cinemachine이 비활성화된 상태에서만 직접 제어
            if (!_isDragging || cinemachineCamera == null)
                return;
            
            // GameCamera를 직접 이동 (Cinemachine이 비활성화되어 있으므로)
            var cameraTransform = GameCamera.transform;
            
            // 이전 터치 위치와 현재 터치 위치의 차이 계산
            Vector2 deltaScreen = _lastTouchPosition - touch.position;
            
            // orthographic 카메라에서 스크린 픽셀을 월드 단위로 변환
            float orthographicSize = cinemachineCamera.Lens.OrthographicSize;
            float worldUnitsPerPixel = (orthographicSize * 2f) / Screen.height;
            
            // 월드 좌표 델타 계산 (X, Y 모두 같은 비율 사용)
            Vector3 deltaWorld = new Vector3(
                deltaScreen.x * worldUnitsPerPixel,
                deltaScreen.y * worldUnitsPerPixel,
                0f
            );
            
            // 카메라 위치 업데이트
            var targetPos = cameraTransform.position + deltaWorld;
            targetPos.z = InitPosZ;

            // Confiner 영역 내로 제한
            targetPos = ClampToConfinerBounds(targetPos, orthographicSize);

            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPos, ref _velocity, _smoothTime * 0.7f);
            
            // 현재 터치 위치 저장
            _lastTouchPosition = touch.position;
        }

        private Vector3 ClampToConfinerBounds(Vector3 position, float orthographicSize)
        {
            var confiner = cinemachineCamera?.GetComponent<CinemachineConfiner2D>();
            if (confiner == null || confiner.BoundingShape2D == null)
                return position;

            var bounds = confiner.BoundingShape2D.bounds;
            
            // 카메라의 뷰포트 크기 계산
            float cameraHeight = orthographicSize * 2f;
            float cameraWidth = cameraHeight * GameCamera.aspect;
            
            // 카메라가 bounds 내에 있도록 제한
            float minX = bounds.min.x + cameraWidth * 0.5f;
            float maxX = bounds.max.x - cameraWidth * 0.5f;
            float minY = bounds.min.y + cameraHeight * 0.5f;
            float maxY = bounds.max.y - cameraHeight * 0.5f;
            
            // bounds가 카메라보다 작은 경우 중앙에 고정
            if (minX > maxX)
            {
                minX = maxX = bounds.center.x;
            }
            if (minY > maxY)
            {
                minY = maxY = bounds.center.y;
            }
            
            return new Vector3(
                Mathf.Clamp(position.x, minX, maxX),
                Mathf.Clamp(position.y, minY, maxY),
                position.z
            );
        }

        private void SyncCinemachinePosition()
        {
            if (cinemachineCamera == null || GameCamera == null)
                return;

            // GameCamera의 현재 위치를 Cinemachine Transform에 동기화
            var cameraPos = GameCamera.transform.position;
            var cinemachineTransform = cinemachineCamera.transform;
            
            // Center 속성: cinemachineCamera.transform.position + cinemachineCamera.transform.forward
            // 따라서 GameCamera 위치에서 forward를 빼면 Cinemachine 위치가 됨
            var targetPos = cameraPos - cinemachineTransform.forward;
            targetPos.z = InitPosZ;
            
            // Cinemachine을 비활성화한 상태에서 위치 설정
            cinemachineTransform.position = targetPos;
            
            // 한 프레임 대기 후 활성화하여 위치가 적용되도록 함
            // (이미 호출하는 곳에서 활성화하므로 여기서는 위치만 설정)
        }

#if UNITY_EDITOR
        private void DragEditor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 드래그 시작 시 Cinemachine 비활성화
                if (cinemachineCamera != null && !_isDragging)
                {
                    cinemachineCamera.SetActive(false);
                    _isDragging = true;
                }
                
                var screenPosition = Input.mousePosition;
                screenPosition.z = GameCamera.nearClipPlane;
                _lastWorldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                var screenPosition = Input.mousePosition;
                screenPosition.z = GameCamera.nearClipPlane;
                var currentWorldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
                Vector3 delta = _lastWorldPosition - currentWorldPosition;

                // GameCamera를 직접 이동 (Cinemachine이 비활성화되어 있으므로)
                var cameraTransform = GameCamera.transform;
                var targetPos = cameraTransform.position + delta;
                targetPos.z = InitPosZ;

                // Confiner 영역 내로 제한
                float orthographicSize = cinemachineCamera != null ? cinemachineCamera.Lens.OrthographicSize : DefaultOrthographicSize;
                targetPos = ClampToConfinerBounds(targetPos, orthographicSize);

                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPos, ref _velocity, _smoothTime * 0.7f);
                _lastWorldPosition = currentWorldPosition;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                // 드래그 종료 시에도 Cinemachine은 활성화하지 않음
                // (다음 드래그 시작 시 다시 비활성화하므로 상태 유지)
                if (_isDragging)
                {
                    _isDragging = false;
                }
            }
        }

        private void ZoomInOutEditor(float scrollDelta)
        {
            if (cinemachineCamera == null)
                return;

            // 마우스 휠 스크롤 값을 줌인/줌아웃으로 변환
            // 스크롤 값이 클수록 더 빠르게 줌인/줌아웃
            float zoomSpeed = 100f; // 줌 속도 조절
            float deltaSize = -scrollDelta * zoomSpeed; // 스크롤 업(양수) = 줌인(크기 감소), 스크롤 다운(음수) = 줌아웃(크기 증가)
            
            float currentSize = cinemachineCamera.Lens.OrthographicSize;
            float newSize = currentSize + deltaSize;
            
            SetOrthographicSize(newSize);
            SetSize();
        }
#endif

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
            
            SetOrthographicSize(cinemachineCamera.Lens.OrthographicSize + deltaMagnitudeDiff);

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

        // void IGameCameraCtr.MoveCenterGameCamera()
        // {
        //     // _isMoveCenter = true;
        //     // Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, GameCamera.nearClipPlane);
        //     // Vector3 worldCenter = GameCamera.ScreenToWorldPoint(screenCenter);
        //     // worldCenter.z = InitPosZ;
        //
        //     GameUtils.SetActive(virtualCamera, false);
        //     GameCamera.transform.position = new Vector3(0, 0, InitPosZ);
        //     GameUtils.SetActive(virtualCamera, true);
        //     // GameCamera.transform.DOMove(new Vector3(0, 0, InitPosZ), 1f);
        // }
        
        public async UniTask MoveCenterGameCameraAsync()
        {
            GameUtils.SetActive(cinemachineCamera, false);
            
            await GameCamera.transform.DOMove(new Vector3(0, 0, InitPosZ), 0.5f);
            
            GameUtils.SetActive(cinemachineCamera, true);
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
                
                float height = cinemachineCamera.Lens.OrthographicSize * 2f;
                float width = height * cinemachineCamera.Lens.Aspect - 300f;
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
                
                float height = cinemachineCamera.Lens.OrthographicSize * 2f - 850f;
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

        void IGameCameraCtr.ZoomIn(Transform targetTr, System.Action endAction)
        {
            if (GameCamera == null)
                return;

            if (!targetTr)
                return;
            
            SetStopUpdate(true);
            ZoomInAsync(targetTr, endAction).Forget();
        }

        private async UniTask ZoomInAsync(Transform targetTr, System.Action endAction)
        {
            if (follwCamera == null)
                return;

            if (cinemachineCamera == null)
                return;
            
            cinemachineCamera.SetActive(false);
            follwCamera.SetActive(true);

            follwCamera.Lens.OrthographicSize = DefaultOrthographicSize;
            follwCamera.Follow = targetTr;
  
            // 2. 초기값 설정
            float startSize = follwCamera.Lens.OrthographicSize;
            float targetSize = 500f; // 매우 큰 값인데, 2D라면 보통 5 전후를 사용하니 확인 필요!
            
            // 이동 거리에 따른 동적 시간 계산
            float distance = Mathf.Abs(startSize - targetSize);
            float duration = Mathf.Max(distance * 0.002f, 0.5f); // 너무 빠르지 않게 최소값 조정
            float timeElapsed = 0f;

            // 3. 부드러운 이동 루프
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(timeElapsed / duration);
                float curveT = Mathf.SmoothStep(0f, 1f, t);

                // [중요] Lens 크기 변경
                follwCamera.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, curveT);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 4. 최종 수치 고정
            follwCamera.Lens.OrthographicSize = targetSize;
            cinemachineCamera.Lens.OrthographicSize = targetSize;

            await UniTask.Yield();
            endAction?.Invoke();
        }

        void IGameCameraCtr.ZoomOut(System.Action endAction)
        {
            if (cinemachineCamera == null)
                return;

            ZoomOutAsync(endAction).Forget();
            // float duration = 1f;
            // DOTween.To(() => cinemachineCamera.Lens.OrthographicSize,
            //         size => cinemachineCamera.Lens.OrthographicSize = size, DefaultOrthographicSize, duration)
            //     .SetEase(Ease.Linear);
            //
            // await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            // Sequence sequence = DOTween.Sequence()
            //     .SetAutoKill(false)
            //     .Append()
            //     .OnComplete(() =>
            //     {
            //         
            //     });
            // sequence.Restart();
        }

        private async UniTask ZoomOutAsync(System.Action endAction)
        {
            if (cinemachineCamera == null)
                return;
            
            follwCamera.SetActive(false);
            cinemachineCamera?.SetActive(true);
            
            follwCamera.Follow = null;
            
            float duration = 1f;
            
            await DOTween.To(() => cinemachineCamera.Lens.OrthographicSize,
                    size => cinemachineCamera.Lens.OrthographicSize = size, DefaultOrthographicSize, duration)
                .SetEase(Ease.Linear);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            endAction?.Invoke();
                    
            SetSize();
            SetStopUpdate(false);
        }

        void IGameCameraCtr.SetConfinerBoundingShape(Collider2D collider)
        {
            var confiner = cinemachineCamera?.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
                return;

            confiner.BoundingShape2D = collider;
        }
        #endregion
    }
}

