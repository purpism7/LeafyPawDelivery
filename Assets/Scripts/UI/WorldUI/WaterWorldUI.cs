
using UI.Common;

namespace UI.WorldUI
{
    public class WaterWorldUI : BaseWorldUI<WaterWorldUI.Data>
    {
        public class Data : BaseWorldUI<WaterWorldUI.Data>.Data
        {

        }

        private void LateUpdate()
        {
            ChainLateUpdate();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

        }
    }
}