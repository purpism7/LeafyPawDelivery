using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI.Component;

namespace UI
{
    public class GetReward : Base<GetReward.Data>
    {
        public class Data : BaseData
        {
            public OpenCondition.Data rewardData = null;
        }

        [SerializeField]
        private OpenCondition reward = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetReward();
        }

        public override void Activate()
        {
            base.Activate();
        }

        private void SetReward()
        {
            if (_data == null)
                return;

            reward?.Initialize(_data.rewardData);
        }

        public void OnClick()
        {
            Deactivate();
        }
    }
}


