using System;
using UnityEngine;

using UI.Common;

namespace UI.WorldUI
{
    public class BloomTimerWorldUI : BaseWorldUI<BloomTimerWorldUI.Data>
    {
        public class Data : BaseWorldUI<BloomTimerWorldUI.Data>.Data
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
