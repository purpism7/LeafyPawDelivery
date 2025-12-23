
using System;
using UnityEngine;
using UnityEngine.UI;

using UI.Common;

namespace UI.WorldUI
{
    public class SowSeeds : BaseWorldUI<SowSeeds.Data>
    {
        public class Data : BaseWorldUI<SowSeeds.Data>.Data
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

        public override void Activate()
        {
            base.Activate();
        }
    }
}