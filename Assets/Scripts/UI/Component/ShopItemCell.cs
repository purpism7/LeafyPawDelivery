using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class ShopItemCell : BaseComponent<ShopItemCell.Data_>
    {
        public class Data_ : BaseData
        {
            public IListener IListener = null;
            public Data.Shop ShopData = null;
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
            var shopData = _data?.ShopData;
            if (shopData == null)
                return;

            var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
            if (atlasLoader == null)
                return;


            if(shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                iconImg.sprite = atlasLoader.GetCurrencySprite("ad");
                iconImg.rectTransform.sizeDelta = new Vector2(150f, 100f);
            }
            else
            {
                iconImg.sprite = atlasLoader.GetShopItemSprite(shopData.ECategory, shopData.IconImg);
            }
        }

        private void SetValue()
        {
            var shopData = _data?.ShopData;
            if (shopData == null)
                return;

            valueTMP?.SetText(shopData.Value.ToString());
        }

        private void SetPaymentValue()
        {
            var shopData = _data?.ShopData;
            if (shopData == null)
                return;

            paymentValueTMP?.SetText(shopData.PaymentValue.ToString());
        }

        public void OnClick()
        {            
            _data?.IListener?.Buy(_data?.ShopData,
                () =>
                {
                    Game.UIManager.Instance?.Top?.CollectCashCurrency(transform.position, _data.ShopData.Value);
                });
        }
    }
}

