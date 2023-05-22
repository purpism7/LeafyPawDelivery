using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Component
{
    public class ArrangementObjectCell : UI.Base<ArrangementObjectCell.Data>
    {
        public class Data : BaseData
        {
            public Object ObjectData = null;
            public int ObjectUId = 0;
        }

        [SerializeField] private TextMeshProUGUI nameTMP;

        public override void Init(Data data)
        {
            base.Init(data);

            SetNameTMP();
        }

        private void SetNameTMP()
        {
            if (_data?.ObjectData == null)
                return;
            
            nameTMP?.SetText(_data.ObjectData.Name);
        }

        public void OnClickArrangement()
        {

        }
    }
}

