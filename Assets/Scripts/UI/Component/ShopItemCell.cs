using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Purchasing;
using System;

using TMPro;

using GameSystem;
using Unity.VisualScripting;
using Product = UnityEngine.Purchasing.Product;
using Type = Game.Type;

namespace UI.Component
{
    public interface IShopItemCell
    {
        Data.Shop ShopData { get; }

        void End();
    }

    public class ShopItemCell : BaseComponent<ShopItemCell.Data_>, IShopItemCell
    {
        public class Data_ : BaseData
        {
            public IListener iListener = null;

            public Data.Shop shopData = null;
            public Product product = null;
            public AD.Data adData = null;
            public bool empty = false;
        }

        public interface IListener
        {
            void Buy(IShopItemCell iShopItemCell, Vector3 pos);
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI valueTMP = null;
        [SerializeField]
        private TextMeshProUGUI paymentValueTMP = null;
        [SerializeField]
        private CodelessIAPButton iAPButton = null;

        [SerializeField]
        private OpenCondition openCondition = null;

        public override void Initialize(Data_ data)
        {
            base.Initialize(data);

            SetItemType();
            SetPlayTimer(true);
        }

        public override void Activate()
        {
            base.Activate();

            if(_data.empty)
            {
                gameObject.GetOrAddComponent<Image>()?.CrossFadeAlpha(0, 0, false);

                Deactivate();

                return;
            }

            SetIconImg();
            SetValue();
            SetPaymentValue();
        }

        private void SetItemType()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            if(shopData.EPayment == Game.Type.EPayment.Money)
            {
                if (string.IsNullOrEmpty(shopData.ProductId))
                    return;

                if (iAPButton == null)
                    return;

                iAPButton.enabled = true;
                iAPButton.productId = shopData.ProductId;
                iAPButton.buttonType = CodelessButtonType.Purchase;
            }
        }

        private void SetIconImg()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            if (iconImg == null)
                return;

            var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
            if (atlasLoader == null)
                return;
            
            if(shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                iconImg.sprite = atlasLoader.GetCurrencySprite("ad");
                iconImg.rectTransform.sizeDelta = new Vector2(190f, 170f);
            }
            else
            {
                if (shopData.ECategory == Type.ECategory.Gift)
                {
                        // atlasLoader.GetSprite("Icon", )
                }
                
                iconImg.sprite = atlasLoader.GetShopItemSprite(shopData.ECategory, shopData.IconImg);
                iconImg.SetNativeSize();
            }
        }

        private void SetValue()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            valueTMP?.SetText("x" + shopData.Value);
        }

        private void SetFreeTMP()
        {
            var text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "free", LocalizationSettings.SelectedLocale);

            paymentValueTMP?.SetText(text);
        }

        private void SetPaymentValue()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            openCondition?.Deactivate();

            if (shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                SetFreeTMP();
            }
            else
            {
                if(shopData.ECategory == Game.Type.ECategory.Cash)
                {
                    var productMetaData = _data?.product?.metadata;
                    if (productMetaData == null)
                        return;

                    paymentValueTMP?.SetText(productMetaData.localizedPriceString);
                }
                else
                {
                    openCondition?.Initialize(new OpenCondition.Data()
                    {
                        ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                        Text = shopData.PaymentValue.ToString(),
                        PossibleFunc = () => true,
                    });

                    openCondition?.Activate();

                    paymentValueTMP?.SetText(string.Empty);
                }
            }
        }

        private void SetPlayTimer(bool initialize)
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            if (shopData.EPayment != Game.Type.EPayment.Advertising)
                return;

            var adData = _data.adData;
            if (adData == null)
                return;

            // var rootType = GetType().Name;
            Game.Timer.Get?.Add(
                new Game.Timer.Data()
                {
                    initialize = initialize,
                    key = adData.adId,
                    ShowRootType = nameof(Shop),
                    timeTMP = paymentValueTMP,
                    btn = iAPButton?.button,
                    addSec = adData.coolTimeSec,
                    endAction = () =>
                    {
                        //iAPButton?.button?.SetInteractable(true);
                        SetFreeTMP();
                    },
                });
        }

        #region IShopItemCell
        Data.Shop IShopItemCell.ShopData
        {
            get
            {
                if (_data == null)
                    return null;

                return _data.shopData;
            }
        }

        //void IShopItemCell.ChainUpdate(System.DateTime dateTime, float timeSec, bool isSync)
        //{
        //    if (_endDateTime == null ||
        //       !_endDateTime.HasValue)
        //        return;

        //    if (!isSync)
        //    {
        //        iAPButton.button.interactable = false;
        //        paymentValueTMP?.SetText("-");

        //        return;
        //    }
   
        //    double remainSec = (_endDateTime.Value - dateTime.ToUniversalTime()).TotalSeconds - timeSec;
        //    if (remainSec > 0)
        //    {
        //        paymentValueTMP?.SetText(TimeSpan.FromSeconds(remainSec).ToString(@"hh\:mm\:ss"));
        //    }
        //    else
        //    {
        //        _endDateTime = null;
        //    }
        //}

        //bool IShopItemCell.CheckSetRemainPlayTime(System.DateTime dateTime)
        //{
        //    var utc = dateTime.ToUniversalTime();

        //    if (_endDateTime != null &&
        //        _endDateTime.HasValue)
        //    {
        //        if ((_endDateTime.Value - utc).TotalSeconds > 0)
        //        {
        //            return false;
        //        }
        //    }

        //    _endDateTime = utc.AddSeconds(60 * 60 * 3f);

        //    return true;
        //}

        void IShopItemCell.End()
        {
            //iAPButton?.button?.SetInteractable(false);

            SetPlayTimer(false);
        }
        #endregion

        public void OnClick()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            _data?.iListener?.Buy(this, transform.position);
        }
    }
}

