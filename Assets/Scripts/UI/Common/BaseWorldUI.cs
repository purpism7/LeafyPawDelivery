using UnityEngine;

using Cysharp.Threading.Tasks;
using Game;


namespace UI.Common
{
    public interface IWorldUI
    {
        // bool IsActivate { get; }
        Transform Transform { get; }
        float Order { get; }
        
        void SetOrder(float order);
    }

    public abstract class BaseWorldUI<T> : Base<T>, IWorldUI where T : BaseWorldUI<T>.Data
    {
        public class Data : BaseData
        {
            public Transform TargetTm { get; private set; } = null;
            public Vector2 Offset { get; private set; } = Vector2.zero;
            public float Order { get; private set; } = 0;

            public Data WithTargetTm(Transform targetTm)
            {
                TargetTm = targetTm;
                return this;
            }

            public Data WithOffset(Vector2 offset)
            {
                Offset = offset;
                return this;
            }

            public Data WithOrder(float order)
            {
                Order = order;
                return this;
            }
        }

        [SerializeField] protected RectTransform rootRectTr = null;
        [SerializeField] private Canvas canvas = null;

        private RectTransform _rectTr = null;
        private Camera _mainCamera = null;
        private Camera _uiCamera = null;
        private RectTransform _worldUIRootRectTr = null;

        public Transform Transform => transform;

        public float Order { get; private set; } = 0;

        public override void Initialize(T data)
        {
            base.Initialize(data);

            _rectTr = GetComponent<RectTransform>();

            var gameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            
            _mainCamera = gameCameraCtr?.MainCamera;
            _uiCamera = gameCameraCtr?.UICamera;
            _worldUIRootRectTr = UIManager.Instance?.WorldUIRootRectTr;
            //_mainCamera = CameraManager.Instance.MainCamera;//Camera.main;

            SetOrder(data?.Order ?? 0);
        }

        public override void Activate()
        {
            base.Activate();

            SetCanvasSortingLayer();
        }

        protected void ChainLateUpdate()
        {
            if (!rootRectTr)
                return;

            Vector3? pos = null;
            if (_data?.TargetTm)
                pos = GetScreenPos(_data.TargetTm.position);

            if (pos != null)
            {
                rootRectTr.anchoredPosition = pos.Value;
                
                // float y = pos.Value.y;
                // float z = 5000f - y;
                //rootRectTr.anchoredPosition3D = new Vector3(pos.Value.x, pos.Value.y);
            }
        }

        protected Vector3? GetScreenPos(Vector3 targetPos)
        {
            if (_mainCamera == null)
                return null;

            if (!_worldUIRootRectTr)
                return null;

            if (_uiCamera == null)
                return null;

            //targetPos.x += _param.Offset.x;
            //targetPos.y += _param.Offset.y;

            var screenPos = _mainCamera.WorldToScreenPoint(targetPos);

            Vector2 localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_worldUIRootRectTr, screenPos, _uiCamera, out localPos);
            //localPos.y += _data.Height;

            localPos.x += _data.Offset.x;
            localPos.y += _data.Offset.y;
            //localPos.x += _param.Offset.x;
            //localPos.y += _param.Offset.y;

            return localPos;
        }

        protected void SetCanvasSortingLayer()
        {
            if (canvas != null)
                canvas.sortingLayerName = "Game";
        }
        
        public void SetOrder(float order)
        {
            Order = order;
            // _data?.WithZOrder(zOrder);
        }
    }
}

