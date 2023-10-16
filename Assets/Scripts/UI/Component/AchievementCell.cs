using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class AchievementCell : Base<AchievementCell.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private TextMeshProUGUI titleTMP = null;
        [SerializeField]
        private Image progressImg = null;
        [SerializeField]
        private TextMeshProUGUI progressTMP = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

           
        }
    }
}

