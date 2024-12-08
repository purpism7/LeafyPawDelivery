using System.Collections;
using System.Collections.Generic;
using GameSystem;
using TMPro;
using UnityEngine;

using UI.Component;
using UnityEngine.UI;

namespace UI
{
    public class GetReward : BasePopup<GetReward.Data>
    {
        public class Data : BaseData
        {
            public List<OpenCondition.Data> RewardDataList = null;
            public string Desc = string.Empty;
            public System.Action EndAction = null;
        }

        [SerializeField] 
        private RectTransform rewardRooRectTm = null;
        [SerializeField] 
        private TextMeshProUGUI descTMP = null;
        
        private List<OpenCondition> _rewardList = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            descTMP?.SetText(data?.Desc);
            SetRewardList();
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_rewardList != null)
            {
                for (int i = 0; i < _rewardList.Count; ++i)
                {
                    _rewardList[i]?.Deactivate();
                }
            }
        }

        private void SetRewardList()
        {
            var rewardDataList = _data?.RewardDataList;
            if (rewardDataList == null)
                return;
            
            if (_rewardList == null)
            {
                _rewardList = new();
                _rewardList.Clear();
            }

            for (int i = 0; i < rewardDataList.Count; ++i)
            {
                var rewardData = rewardDataList[i];
                if(rewardData == null)
                    continue;

                OpenCondition reward = null;
                if (_rewardList?.Count > i)
                {
                    reward = _rewardList[i];
                    reward?.Initialize(rewardData);
                }
                else
                {
                    reward = new ComponentCreator<OpenCondition, OpenCondition.Data>()
                        .SetData(rewardData)
                        .SetRootRectTm(rewardRooRectTm)
                        .Create();

                    if(reward == null)
                        continue;
                    
                    reward.transform.localScale = Vector3.one * 1.2f;
                    
                    _rewardList?.Add(reward);
                }
                
                reward?.Activate();
            }
        }

        public void OnClick()
        {
            Deactivate();
            
            _data?.EndAction?.Invoke();
        }
    }
}


