using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

using TMPro;

namespace UI.Component
{
    public class ShopItemCell : BaseComponent<ShopItemCell.Data_>
    {
        public class Data_ : BaseData
        {
            public IListener iListener = null;
            public Data.Shop shopData = null;
            public Product product = null;
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
        [SerializeField]
        private CodelessIAPButton iAPButton = null;

        [SerializeField]
        private OpenCondition openCondition = null;

        public override void Initialize(Data_ data)
        {
            base.Initialize(data);

            SetItemType();
        }

        public override void Activate()
        {
            base.Activate();

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
                //iAPButton.onPurchaseComplete.RemoveAllListeners();
                //iAPButton.F?.AddListener(OnPurchaseComplete);

                ////iAPButton.onPurchaseFailed?.RemoveAllListeners();
                //iAPButton.onPurchaseFailed?.AddListener(OnPurchaseFailed);

                ////iAPButton.onProductFetched?.RemoveAllListeners();
                //iAPButton.onProductFetched?.AddListener(OnPurchaseFetched);
            }
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

            openCondition?.Deactivate();

            if (shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                var text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "free", LocalizationSettings.SelectedLocale);

                paymentValueTMP?.SetText(text);
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
                        Text = "x" + shopData.PaymentValue,
                        PossibleFunc = () => true,
                    });

                    openCondition?.Activate();

                    paymentValueTMP?.SetText(string.Empty);
                }
            }
        }

        //public void OnPurchaseComplete(Product product)
        //{
        //    Debug.Log("OnPurchaseComplete");

        //    //_data?.iListener?.Buy(_data?.shopData, product,
        //    //    () =>
        //    //    {
        //    //        Game.UIManager.Instance?.Top?.CollectCashCurrency(transform.position, _data.shopData.Value);
        //    //    });
        //}

        //public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
        //{
        //    Debug.Log("OnPurchaseFailed = " + description.message);
        //}

        //public void OnPurchaseFetched(Product product)
        //{
        //    Debug.Log("OnPurchaseFetched");
        //}

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

