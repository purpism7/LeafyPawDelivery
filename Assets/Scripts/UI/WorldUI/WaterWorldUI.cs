using UnityEngine;
using UnityEngine.UI;

using Game;
using GameSystem;
using UI.Common;

namespace UI.WorldUI
{
    public class WaterWorldUI : BaseWorldUI<WaterWorldUI.Data>
    {
        public class Data : BaseWorldUI<WaterWorldUI.Data>.Data
        {
            public IListener Listener { get; private set; } = null;

            public Data WithListener(IListener listener)
            {
                Listener = listener;
                return this;
            }
        }

        public interface IListener
        {
            void OnClick();
        }
        
        [SerializeField] private Button btn = null;

        private void LateUpdate()
        {
            ChainLateUpdate();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            btn?.onClick?.RemoveAllListeners();
            btn?.onClick?.AddListener(OnClick);
        }

        private void OnClick()
        {
            Deactivate();
            ObjectPooler.Instance?.Return(this);
            
            _data?.Listener?.OnClick();
        }
    }
}