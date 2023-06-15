using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace UI
{
    public class Top : Common<Top.Data>
    {
        public class Data : BaseData
        {
            public int PlaceId = 0;
        }
        
        [SerializeField] private TextMeshProUGUI lvTMP = null;
        [SerializeField] private TextMeshProUGUI animalCurrencyTMP = null;
        [SerializeField] private TextMeshProUGUI objectCurrencyTMP = null;
        [SerializeField] private TextMeshProUGUI cashTMP = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            Set();
        }
        
        // a=x10000
        // b=x100000
        // c=x1000000
        private void Set()
        {
            if (_data == null)
                return;
            
            var userInfo = Info.UserManager.Instance?.User;
            if (userInfo == null)
                return;
            
            lvTMP?.SetText(userInfo.Lv + "");

            var currency = userInfo.GetCurrency(_data.PlaceId);
            if (currency != null)
            {
                animalCurrencyTMP?.SetText(currency.AnimalCurrency + "");
                objectCurrencyTMP?.SetText(currency.ObjectCurrency + "");
            }

            cashTMP?.SetText(userInfo.Cash + "");
        }

        public void OnAttracted()
        {
            Debug.Log("OnAttracted");
        }
    }
}

