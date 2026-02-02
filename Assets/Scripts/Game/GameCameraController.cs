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
        private float _halfHeight = 0;
        // private float _dragWidth = 0;
        private Vector3 _velocity = Vector3.zero;
        private float _smoothTime = 0.045f;
    

        private float _width = 0;
        private bool _zoomIn = false;
        // private Vector3 _zoomInTargetPos = Vector3.zero;
        private System.Action _zoomInEndAction = null; 
        private float _timeElapsed = 0;
        private Vector3 _lastPreZoomPos = Vector3.zero;

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

            if(follwCamera != null)
            {
                follwCamera.Follow = null;
                follwCamera.SetActive(false);
            }

            SetSize();
            SetStopUpdate(true);
        }

        private void LateUpdate()
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

            float height = GameCamera.orthographicSize * 2f;
            float width = height * GameCamera.aspect;

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
                        //var screenPosition = Input.mousePosition;
                        //screenPosition.z = 10f;
            
                        //_lastWorldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
                        
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
        
        private Vector3 ClampToConfinerBounds(Vector3 targetPos, Camera camera = null)
        {
            // 1. 시네머신 및 Confiner 컴포넌트 가져오기
            if (cinemachineCamera == null) 
                return targetPos;
    
            var confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
    
            // Confiner가 없거나, BoundingShape(콜라이더)가 연결 안 되어 있으면 제한 없이 리턴
            if (confiner == null || confiner.BoundingShape2D == null)
                return targetPos;

            // 2. 현재 카메라의 화면 크기(절반) 계산
            // camera 파라미터가 제공되면 해당 카메라 사용, 없으면 GameCamera 사용
            Camera cam = camera != null ? camera : GameCamera;
            if (cam == null)
                return targetPos;
                
            float vertExtent = cam.orthographicSize; 
            float horzExtent = vertExtent * cam.aspect;
            
            return ClampToConfinerBoundsInternal(targetPos, vertExtent, horzExtent);
        }

        private Vector3 ClampToConfinerBounds(Vector3 targetPos, float orthographicSize, float aspect)
        {
            // 1. 시네머신 및 Confiner 컴포넌트 가져오기
            if (cinemachineCamera == null) 
                return targetPos;
    
            var confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
    
            // Confiner가 없거나, BoundingShape(콜라이더)가 연결 안 되어 있으면 제한 없이 리턴
            if (confiner == null || confiner.BoundingShape2D == null)
                return targetPos;

            // 2. 제공된 orthographic size와 aspect 사용
            float vertExtent = orthographicSize; 
            float horzExtent = vertExtent * aspect;
            
            return ClampToConfinerBoundsInternal(targetPos, vertExtent, horzExtent);
        }

        private Vector3 ClampToConfinerBoundsInternal(Vector3 targetPos, float vertExtent, float horzExtent)
        {
            if (cinemachineCamera == null) 
                return targetPos;
    
            var confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null || confiner.BoundingShape2D == null)
                return targetPos;

            // 3. 콜라이더 영역(Bounds) 가져오기
            Bounds bounds = confiner.BoundingShape2D.bounds;

            // 4. 제한 영역 계산 (콜라이더 내부 - 카메라 크기)
            // 카메라가 맵 밖으로 나가지 않으려면 [min + 카메라반크기 ~ max - 카메라반크기] 사이여야 함
            float minX = bounds.min.x + horzExtent;
            float maxX = bounds.max.x - horzExtent;
            float minY = bounds.min.y + vertExtent;
            float maxY = bounds.max.y - vertExtent;

            // 5. 맵이 카메라보다 작은 경우 예외 처리 (항상 중앙에 위치)
            if (minX > maxX)
            {
                // 맵 폭이 카메라보다 좁으면 맵의 중심 X좌표로 고정
                minX = maxX = bounds.center.x; 
            }
            if (minY > maxY)
            {
                // 맵 높이가 카메라보다 낮으면 맵의 중심 Y좌표로 고정
                minY = maxY = bounds.center.y;
            }

            // 6. 좌표 클램핑 (Clamp)
            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

            return new Vector3(clampedX, clampedY, targetPos.z);
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
            GameCamera.orthographicSize = resOrthographicSize;
        }

        private void Move(Touch touch)
        {
            var targetTr = GameCamera.transform;
            
            var pos = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0);
            // var cameraTm = cinemachineCamera.Target.TrackingTarget.transform;
            var targetPos = targetTr.position - pos;
            targetPos.z = InitPosZ;
            targetPos = ClampToConfinerBounds(targetPos);
   
            targetTr.position = Vector3.SmoothDamp(targetTr.position, targetPos, ref _velocity, _smoothTime * 0.7f);
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

            // 1. 터치 델타 계산
            float prevTouchDeltaMag = (firPrevPos - secPrevPos).magnitude;
            float touchDeltaMag = (firTouch.position - secTouch.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 2. 새로운 오쏘그래픽 사이즈 계산 및 적용
            // (SetOrthographicSize 내부에서 이미 Clamp가 되어 있다고 가정)
            float targetSize = GameCamera.orthographicSize + (deltaMagnitudeDiff * 2.0f); // 감도 조절 필요 시 곱셈 추가
            SetOrthographicSize(targetSize);
    
            // 3. 사이즈 정보 갱신 (Height, Width 등 업데이트)
            SetSize();

            Vector3 clampedPos = ClampToConfinerBounds(GameCamera.transform.position);
            GameCamera.transform.position = clampedPos;
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

        public async UniTask MoveCenterGameCameraAsync()
        {
            // GameUtils.SetActive(cinemachineCamera, false);
            
            await GameCamera.transform.DOMove(new Vector3(0, 0, InitPosZ), 0.5f);
            
            // GameUtils.SetActive(cinemachineCamera, true);
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

        private async UniTask ZoomInAsync(Transform targetTr, System.Action endAction)
        {
            if (follwCamera == null) return;

            // 1. 현재 메인 카메라의 상태 캡처
            Vector3 startPos = GameCamera.transform.position;
            startPos.z = InitPosZ;
            float startSize = GameCamera.orthographicSize;

            _lastPreZoomPos = startPos;

            // 2. Follow 카메라 초기 설정
            follwCamera.Follow = null;
            follwCamera.transform.position = startPos;
            follwCamera.Lens.OrthographicSize = startSize;
            follwCamera.SetActive(true);

            float targetSize = 500f; // [설정] 목표 줌 사이즈 (예시값)

            // =========================================================
            // [핵심 수정 1] 타겟을 화면 정중앙에 배치하기 위한 카메라 위치 계산
            // Orthographic 카메라에서 타겟을 정중앙에 배치하려면 카메라 위치 = 타겟 위치
            // =========================================================
            follwCamera.Lens.OrthographicSize = targetSize;

            // 타겟의 월드 좌표를 카메라 위치로 설정 (타겟이 화면 정중앙에 오도록)
            Vector3 desiredCameraPos = new Vector3(targetTr.position.x, targetTr.position.y, InitPosZ);
            
            // 목표 사이즈 기준으로 경계 체크 (follwCamera의 orthographic size와 GameCamera의 aspect 사용)
            float aspect = GameCamera != null ? GameCamera.aspect : 16f / 9f;
            Vector3 targetPos = ClampToConfinerBounds(desiredCameraPos, targetSize, aspect);

            // 다시 시작 사이즈로 되돌림 (애니메이션을 위해)
            follwCamera.Lens.OrthographicSize = startSize;
            // =========================================================

            // 애니메이션 시간 설정
            float distance = Vector3.Distance(startPos, targetPos);
            float duration = Mathf.Max(distance * 0.001f, 1f);
            float timeElapsed = 0f;

            // 5. 부드러운 이동 루프
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(timeElapsed / duration);
                float curveT = Mathf.SmoothStep(0f, 1f, t);

                // 현재 프레임의 orthographic size 계산
                float currentSize = Mathf.Lerp(startSize, targetSize, curveT);
                follwCamera.Lens.OrthographicSize = currentSize;

                // 타겟을 화면 정중앙에 배치하기 위한 카메라 위치 계산
                Vector3 desiredPos = new Vector3(targetTr.position.x, targetTr.position.y, InitPosZ);
                // 현재 사이즈 기준으로 경계 체크 (사이즈가 변하면서 경계도 달라질 수 있음)
                Vector3 currentTargetPos = ClampToConfinerBounds(desiredPos, currentSize, aspect);
                
                follwCamera.transform.position = Vector3.Lerp(startPos, currentTargetPos, curveT);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 6. 도착 처리 - 최종 사이즈로 다시 경계 체크하여 정확한 위치 설정
            follwCamera.Lens.OrthographicSize = targetSize;
            Vector3 finalDesiredPos = new Vector3(targetTr.position.x, targetTr.position.y, InitPosZ);
            Vector3 finalPos = ClampToConfinerBounds(finalDesiredPos, targetSize, aspect);
            follwCamera.transform.position = finalPos;
            follwCamera.ForceCameraPosition(finalPos, Quaternion.identity);

            // [중요] 타겟을 계속 따라다니게 하려면 여기서 Follow 연결
            // follwCamera.Follow = targetTr; 

            await UniTask.Yield();
            endAction?.Invoke();
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

        void IGameCameraCtr.ZoomOut(System.Action endAction)
        {
            if (cinemachineCamera == null)
                return;

            ZoomOutAsync(endAction).Forget();
        }

        private async UniTask ZoomOutAsync(System.Action endAction)
        {
            if (follwCamera == null || GameCamera == null) return;

            // 1. 시작 상태 (현재 줌인된 상태)
            Vector3 startPos = follwCamera.transform.position;
            float startSize = follwCamera.Lens.OrthographicSize;

            // 2. 목표 상태 (원래 기본 상태)
            // _lastPreZoomPos는 ZoomIn 시작 시 저장해둔 GameCamera.transform.position입니다.
            // 만약 저장된 값이 없다면 특정 기본 좌표를 사용하세요.
            Vector3 targetPosCenter = _lastPreZoomPos; 
            float targetSize = DefaultOrthographicSize;
            float aspect = GameCamera.aspect;

            float duration = 1.0f; // 줌아웃 시간
            float timeElapsed = 0f;

            // 3. 부드러운 복귀 루프
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(timeElapsed / duration);
                float curveT = Mathf.SmoothStep(0f, 1f, t); // 부드러운 가속/감속

                // 현재 프레임의 사이즈 계산
                float currentSize = Mathf.Lerp(startSize, targetSize, curveT);
                follwCamera.Lens.OrthographicSize = currentSize;

                // [핵심] 현재 사이즈에 맞춰서 목표 위치를 실시간으로 제한(Clamp)
                // 줌아웃되면서 화면이 커지면 카메라 중심이 이동할 수 있는 범위가 좁아집니다.
                Vector3 currentTargetPos = ClampToConfinerBounds(targetPosCenter, currentSize, aspect);
        
                // 위치 보간 이동
                Vector3 lerpedPos = Vector3.Lerp(startPos, currentTargetPos, curveT);
                
                // [수정] 보간된 위치도 현재 사이즈 기준으로 다시 클램핑하여 맵 바깥 영역이 보이지 않도록 보장
                follwCamera.transform.position = ClampToConfinerBounds(lerpedPos, currentSize, aspect);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 4. 최종 상태 고정
            follwCamera.Lens.OrthographicSize = targetSize;
            Vector3 finalPos = ClampToConfinerBounds(targetPosCenter, targetSize, aspect);
            follwCamera.transform.position = finalPos;

            // 5. 메인 카메라로 제어권 전환
            // 메인 카메라의 위치를 줌아웃이 끝난 최종 위치로 옮겨준 뒤 가상 카메라를 끕니다.
            GameCamera.transform.position = finalPos;
            GameCamera.orthographicSize = targetSize;
    
            follwCamera.SetActive(false);
            follwCamera.Follow = null;

            // 6. 후속 처리
            SetSize();
            SetStopUpdate(false);
    
            await UniTask.Yield();
            endAction?.Invoke();
        }
        
        // private async UniTask ZoomOutAsync(System.Action endAction)
        // {
        //     if (GameCamera == null)
        //         return;
        //     
        //     follwCamera.SetActive(false);
        //     // cinemachineCamera?.SetActive(true);
        //     
        //     follwCamera.Follow = null;
        //     
        //     float duration = 1f;
        //     
        //     await DOTween.To(() => GameCamera.orthographicSize,
        //             size => GameCamera.orthographicSize = size, DefaultOrthographicSize, duration)
        //         .SetEase(Ease.Linear);
        //     await UniTask.Delay(TimeSpan.FromSeconds(duration));
        //     
        //     endAction?.Invoke();
        //             
        //     SetSize();
        //     SetStopUpdate(false);
        // }

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

