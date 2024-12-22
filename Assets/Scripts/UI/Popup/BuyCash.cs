using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

using UI.Component;
using GameSystem;
using TMPro;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class BuyCash : BasePopup<BuyCash.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Sprite targetSprite = null;
            public Type.EPayment EPayment = Type.EPayment.None;
            public int Price = 0;
            public float scale = 1f;
            
            public AD.Data ADData = null;
        }

        public interface IListener
        {
            void Buy();
        }
        
        [SerializeField] 
        private RectTransform buyCurrencyRootRectTm = null;
        [SerializeField]
        private Button buyBtn = null;

        [SerializeField]
        private Image buyTargetImg = null;
        [SerializeField]
        private Component.OpenCondition openCondition = null;
        
        [Header("Advertising")] 
        [SerializeField] 
        private RectTransform buyADRootRectTm = null;
        [SerializeField] 
        private TextMeshProUGUI remainPlayTimeTMP = null;
        [SerializeField] 
        private Image adIconImg = null;
        [SerializeField]
        private Button buyADBtn = null;
        
        private bool _possibleBuy = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetImg();
            SetOpenCondition();
            
            // if (_data.EPayment == Type.EPayment.Advertising)
            //     SetPlayTimer(true);
            
            if (_data.EPayment == Type.EPayment.Advertising)
                SetPlayTimer(true);
        }

        public override void Activate()
        {
            base.Activate();

            GameUtils.SetActive(buyADRootRectTm, _data.EPayment == Type.EPayment.Advertising);
            GameUtils.SetActive(buyADBtn, _data.EPayment == Type.EPayment.Advertising);
            GameUtils.SetActive(buyCurrencyRootRectTm, _data.EPayment != Type.EPayment.Advertising);
            GameUtils.SetActive(buyBtn, _data.EPayment != Type.EPayment.Advertising);
            
            openCondition?.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _endTask = true;
        }

        private void SetImg()
        {
            if (buyTargetImg == null)
                return;

            if (_data == null)
                return;

            buyTargetImg.sprite = _data.targetSprite;
            buyTargetImg.SetNativeSize();

            var rectTm = buyTargetImg.GetComponent<RectTransform>();
            if(rectTm)
            {
                rectTm.localScale = Vector3.one * _data.scale;
            }
        }

        private void SetOpenCondition()
        {
            if (_data == null)
                return;

            if (openCondition == null)
                return;

            _possibleBuy = CheckPossibleBuy;
            
            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = CurrencySprite,
                Text = _data.Price.ToString(),
                PossibleFunc = () => _possibleBuy,
            };

            openCondition.Initialize(openConditionData);
        }
        
        private Sprite CurrencySprite
        {
            get
            {
                switch (_data.EPayment)
                {
                    case Type.EPayment.Cash:
                        return ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite;
                    
                    case Type.EPayment.ObjectCurrency:
                    {
                        var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
                        if (placeData == null)
                            return null;
                        
                        return ResourceManager.Instance?.AtalsLoader.GetCurrencySprite(placeData.ObjectSpriteName);
                    }
                    
                    default:
                        return null;
                }
            }   
        }

        private bool CheckPossibleBuy
        {
            get
            {
                switch (_data.EPayment)
                {
                    case Type.EPayment.Cash:
                    {
                        var user = Info.UserManager.Instance?.User;
                        long userCash = 0;
                        if (user != null)
                            userCash = user.Cash;

                        return userCash >= _data.Price;
                    }
                    
                    case Type.EPayment.ObjectCurrency:
                    {
                        var user = Info.UserManager.Instance.User;
                        if (user == null)
                            return false;

                        var currency = user.CurrenctCurrency;
                        if (currency == null)
                            return false;

                        return currency.Object >= _data.Price;
                    }
                    
                    default:
                        return false;
                }
            }
        }
        
        private void SetPlayTimer(bool initialize)
        {
            if (_data == null)
                return;
            
            var timer = Game.Timer.Get;
            if (timer == null)
                return;

            var adData = _data.ADData;
            if (adData == null)
                return;

            // float addSec = 0;
            // if (!initialize)
            //     addSec = adData.coolTimeSec;

            var rootType = adData.eCategory.ToString();
            
            adIconImg?.SetActive(false);
            
            Game.Timer.Get?.SetRootType(rootType)?.Add(
                new Game.Timer.Data
                {
                    initialize = initialize,
                    key = adData.adId,
                    ShowRootType = rootType,
                    timeTMP = remainPlayTimeTMP,
                    btn = buyADBtn,
                    addSec = adData.coolTimeSec,
                    endAction = () =>
                    {
                        remainPlayTimeTMP?.SetText(string.Empty);
                        _possibleBuy = true;
                        adIconImg?.SetActive(true);
                    }
                });
        }

        public void OnClickCancel()
        {
            Deactivate();
        }

        public void OnClickBuy()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
            
            Deactivate();

            if (_data.EPayment == Type.EPayment.ObjectCurrency)
            {
                var currency = new Info.User.Currency
                {
                    PlaceId = GameUtils.ActivityPlaceId,
                    Object = -_data.Price,
                };

                if (!Info.UserManager.Instance.CheckCurrency(currency))
                {
                    var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_object_currency", LocalizationSettings.SelectedLocale);
                    Game.Toast.Get?.Show(localDesc);

                    return;
                }
            }
            
            if (!_possibleBuy)
            {
                if (_data.EPayment == Type.EPayment.Cash)
                {
                    var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_jewel", LocalizationSettings.SelectedLocale);
                    Game.Toast.Get?.Show(localDesc);
                }
                
                return;
            }
            
            if (_data.EPayment == Type.EPayment.Advertising)
            {
                var adData = _data.ADData;
                if (adData == null)
                    return;

                AdProvider.Get?.ShowAd(adData,
                    (rewardValue) =>
                    {
                        if (rewardValue > 0)
                            _data?.IListener?.Buy();
                        
                        SetPlayTimer(false);
                    });

                return;
            }
            
            _data?.IListener?.Buy();
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }
    }
}

