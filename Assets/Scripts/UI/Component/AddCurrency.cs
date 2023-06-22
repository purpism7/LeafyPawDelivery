using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace UI.Component
{
    public class AddCurrency : Base<AddCurrency.Data>
    {
        [SerializeField] private TextMeshProUGUI currencyTMP;
        
        public class Data : BaseData
        {
            public int Currency = 0;
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Initialize();
        }

        private void Initialize()
        {
            SetText();
        }

        private void SetText()
        {
            currencyTMP?.SetText("+" + _data.Currency);
        }
    }
}
