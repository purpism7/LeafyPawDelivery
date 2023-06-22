using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Game;
using GameSystem;
using UnityEngine;

using TMPro;
using UI.Component;

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

        [SerializeField] private RectTransform collectCurrencyRootRectTm = null;
        [SerializeField] private RectTransform addCurrencyRootRectTm = null;
        [SerializeField] private RectTransform animalCurrencyRectTm = null;

        private List<CollectCurrency> _collectCurrencyList = new();
        private List<AddCurrency> _addCurrencyList = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            _collectCurrencyList?.Clear();
            _addCurrencyList?.Clear();
            
            Initialize();
        }
        
        // a=x10000
        // b=x100000
        // c=x1000000
        private void Initialize()
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

        public void CollectCurrency(Vector3 startPos)
        {
            if (_collectCurrencyList == null)
                return;

            var data = new CollectCurrency.Data()
            {
                StartPos = startPos,
                EndPos = animalCurrencyRectTm.position,
                CollectEndAction =
                    () =>
                    {
                        EndCollectCurrency();
                    },
            };
            
            var collectCurrency = _collectCurrencyList.Find(collectCurrency => !collectCurrency.IsActivate);
            if (collectCurrency != null)
            {
                collectCurrency.Initialize(data);

                return;
            }
            
            var component = new ComponentCreator<CollectCurrency, CollectCurrency.Data>()
                .SetData(data)
                .SetRootRectTm(collectCurrencyRootRectTm)
                .Create();
            
            _collectCurrencyList.Add(component);
        }
        
        private void EndCollectCurrency()
        {
            AddCurrency();
        }

        private void AddCurrency()
        {
            if (_addCurrencyList == null)
                return;
            
            var data = new AddCurrency.Data()
            {
                StartPos = objectCurrencyTMP.transform.position,
                Currency = 1,
            };
            
            var addCurrency = _addCurrencyList.Find(addCurrency => !addCurrency.IsActivate);
            if (addCurrency != null)
            {
                addCurrency.Initialize(data);

                return;
            }
            
            var component = new ComponentCreator<AddCurrency, AddCurrency.Data>()
                .SetData(data)
                .SetRootRectTm(addCurrencyRootRectTm)
                .Create();
            
            _addCurrencyList.Add(component);
        }
    }
}

