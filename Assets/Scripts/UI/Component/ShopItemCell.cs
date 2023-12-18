using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

namespace UI.Component
{
    public class ShopItemCell : BaseComponent<ShopItemCell.Data_>
    {
        public class Data_ : BaseData
        {
            public IListener iListener = null;
            public Data.Shop shopData = null;
        }

        public interface IListener
        {
            void Buy(Data.Shop shopData, System.Action endAction);
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI valueTMP = null;
        [SerializeField]
        private TextMeshProUGUI paymentValueTMP = null;

        public override void Initialize(Data_ data)
        {
            base.Initialize(data);

            SetIconImg();
            SetValue();
            SetPaymentValue();
        }

        private void SetIconImg()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
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
                iconImg.sprite = atlasLoader.GetShopItemSprite(shopData.ECategory, shopData.IconImg);
            }
        }

        private void SetValue()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            valueTMP?.SetText(shopData.Value.ToString());
        }

        private void SetPaymentValue()
        {
            var shopData = _data?.shopData;
            if (shopData == null)
                return;

            if (shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                var text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "free", LocalizationSettings.SelectedLocale);

                paymentValueTMP?.SetText(text);
            }
            else
            {
                paymentValueTMP?.SetText(shopData.PaymentValue.ToString());
            }
        }

        public void OnClick()
        {            
            _data?.iListener?.Buy(_data?.shopData,
                () =>
                {
                    Game.UIManager.Instance?.Top?.CollectCashCurrency(transform.position, _data.shopData.Value);
                });
        }
    }
}

