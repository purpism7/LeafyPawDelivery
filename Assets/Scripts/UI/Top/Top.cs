using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Game;
using GameSystem;
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
        
        [SerializeField] private RectTransform objectCurrencyRectTm = null;
        [SerializeField] private RectTransform animalCurrencyRectTm = null;
        [SerializeField] private RectTransform cashCurrencyRectTm = null;

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
            SetCurrency();
        }

        public void SetCurrency()
        {
            if (_data == null)
                return;

            var userInfo = Info.UserManager.Instance?.User;
            if (userInfo == null)
                return;

            var currency = userInfo.GetCurrency(_data.PlaceId);
            if (currency != null)
            {
                animalCurrencyTMP?.SetText(currency.Animal + "");
                objectCurrencyTMP?.SetText(currency.Object + "");
            }

            cashTMP?.SetText(userInfo.Cash + "");
        }

        #region Collect Currency (Move Action Currnecy)
        public void CollectCashCurrency(Vector3 startPos, int currency)
        {
            if (_collectCurrencyList == null)
                return;

            var data = new CollectCurrency.Data()
            {
                StartPos = startPos,
                EndPos = cashCurrencyRectTm.position,
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                CollectEndAction =
                    () =>
                    {
                        AddCashCurrency(currency);
                    },
            };

            CollectCurrency(data);
        }

        public void CollectCurrency(Vector3 startPos, Type.EElement eElement, int currency)
        {
            if (_collectCurrencyList == null)
                return;

            var currencyInfo = Game.Data.Const.GetCurrencyInfo(_data.PlaceId);
            if (currencyInfo == null)
                return;

            var currencyName = currencyInfo.Animal.ToString();
            if(eElement == Type.EElement.Object)
            {
                currencyName = currencyInfo.Object.ToString();
            }

            var data = new CollectCurrency.Data()
            {
                StartPos = startPos,
                EndPos = eElement == Type.EElement.Object ? objectCurrencyRectTm.position : animalCurrencyRectTm.position,
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyName.ToLower()),
                CollectEndAction =
                    () =>
                    {
                        AddCurrency(eElement, currency);
                    },
            };

            CollectCurrency(data);

            // save currency value.
            Info.UserManager.Instance?.User?.SetCurrency(eElement, currency);
            MainGameManager.Instance?.AddAcquire(eElement, Game.Type.EAcquireAction.Obtain, 1); 
        }

        private void CollectCurrency(CollectCurrency.Data data)
        {
            if (data == null)
                return;

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
        #endregion

        #region Add Currency (Action Currency Text)
        private void AddCurrency(Type.EElement eElement, int currency)
        {
            if (_addCurrencyList == null)
                return;

            var startPos = objectCurrencyTMP.transform.position;
            if(eElement == Type.EElement.Animal)
            {
                startPos = animalCurrencyTMP.transform.position;
            }
           
            var data = new AddCurrency.Data()
            {
                StartPos = startPos,
                EElement = eElement,
                Currency = currency,
            };

            AddCurrency(data);
        }

        private void AddCashCurrency(int currency)
        {
            if (_addCurrencyList == null)
                return;

            if (cashTMP == null)
                return;

            var startPos = cashTMP.transform.position;

            var data = new AddCurrency.Data()
            {
                StartPos = startPos,
                Currency = currency,
            };

            AddCurrency(data);
        }

        private void AddCurrency(AddCurrency.Data data)
        {
            if (data == null)
                return;

            data.StartPos.x -= 5f;
            data.StartPos.y += 20f;

            SetCurrency();

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
        #endregion
    }
}

