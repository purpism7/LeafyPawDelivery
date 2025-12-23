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
            var data = new UI.WorldUI.SowSeeds.Data
            {

            };

            data.WithTargetTm(_data?.TargetTm)
                .WithOffset(new Vector2(0, 100f));

            var sowSeeds = new GameSystem.ComponentCreator<UI.WorldUI.SowSeeds, UI.WorldUI.SowSeeds.Data>()
                .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                .SetData(data)
                .Create();
            sowSeeds?.Activate();

            Deactivate();
            ObjectPooler.Instance?.Return(this);
        }
    }
}