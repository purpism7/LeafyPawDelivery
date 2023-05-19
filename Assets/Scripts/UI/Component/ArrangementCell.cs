using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Component
{
    public class ArrangementCell : UI.Base<ArrangementCell.Data>
    {
        public class Data : BaseData
        {
            public Animal animalData = null;
        }

        [SerializeField] private TextMeshProUGUI nameTMP;

        public override void Init(Data data)
        {
            base.Init(data);

            SetNameTMP();
        }

        private void SetNameTMP()
        {
            nameTMP?.SetText(_data.animalData?.Name);
        }
    }
}

