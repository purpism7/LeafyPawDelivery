using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;

using UI.Component;
using GameSystem;

namespace UI
{
    public interface IShop
    {
        Product GetProduct(string productId);
    }

    public class Shop : BasePopup<Shop.Data_>, ShopItemCell.IListener, IShop, IDetailedStoreListener
    {
        public class Data_ : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;

        private bool _initializeStore = false;
        private IStoreController _iStoreCtr = null;

        private Data.Shop _buyShopData = null;
        private System.Action _endBuyAction = null;

        public override IEnumerator CoInitialize(Data_ data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InitializeIAP();

            yield return StartCoroutine(CoSetItemList());

            InitializeChildComponent();

            yield return new WaitUntil(() => _initializeStore);
        }

        public override void Activate()
        {
            base.Activate();

            ActivateChildComponent(typeof(ShopItemCell));

            itemScrollRect?.ResetScrollPos();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void InitializeIAP()
        {
            _initializeStore = false;

            if (_iStoreCtr != null)
            {
                _initializeStore = true;

                return;
            }                

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var productItem in ProductCatalog.LoadDefaultCatalog().allValidProducts)
            {
                builder.AddProduct(productItem.id, productItem.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        private IEnumerator CoSetItemList()
        {
            var shopListDic = ShopContainer.Instance?.ShopListDic;
            if (shopListDic == null)
                yield break;

            var scrollContent = itemScrollRect?.content;
            if (scrollContent == null)
                yield break;

            foreach (var data in shopListDic)
            {
                var dataList = data.Value;

                if (dataList == null)
                    continue;

                var cell = new ComponentCreator<ShopItemGroup, ShopItemGroup.Data_>()
                    .SetData(new ShopItemGroup.Data_()
                    {
                        iShopItemCellListener = this,
                        eCategory = dataList.Count > 0 ? dataList[0].ECategory : Game.Type.ECategory.None,
                        shopDataList = dataList,
                        iShop = this,
                    })
                    .SetRootRectTm(scrollContent)
                    .Create();
            }

            itemScrollRect?.ResetScrollPos();

            yield return null;
        }

        void ShopItemCell.IListener.Buy(Data.Shop shopData, System.Action endAction)
        {
            if (shopData == null)
                return;

            _buyShopData = shopData;
            _endBuyAction = endAction;

            Game.UIManager.Instance?.ActivateSreenSaver();
        }

        Product IShop.GetProduct(string productId)
        {
            if (_iStoreCtr == null)
                return null;

            return _iStoreCtr.products?.WithID(productId);
        }

        #region IDetailedStoreListener
        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Game.UIManager.Instance?.DeactivateScreenSaver();
            Debug.Log("OnInitializeFailed = " + message);
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log("ProcessPurchase = " + purchaseEvent.purchasedProduct.metadata.localizedTitle);

            Game.UIManager.Instance?.DeactivateScreenSaver();

            if (_buyShopData == null)
                return PurchaseProcessingResult.Pending;

            _endBuyAction?.Invoke();

            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Game.UIManager.Instance?.DeactivateScreenSaver();
            Debug.Log("OnPurchaseFailed = " + failureReason);
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized");

            _iStoreCtr = controller;

            _initializeStore = true;
        }

        void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("OnPurchaseFailed = " + failureDescription.message);
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed = " + error);
        }
        #endregion
    }
}
