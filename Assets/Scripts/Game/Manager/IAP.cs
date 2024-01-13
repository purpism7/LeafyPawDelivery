using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Game.Manager
{
    public class IAP : Singleton<IAP>, IDetailedStoreListener
    {
        public interface IListener
        {
            void SuccessPurchase(Product product);
            void FailPurchase(Product product);
        }

        private IStoreController _iStoreCtr = null;
        private IExtensionProvider _iExtensionProvider = null;
        private IListener _iListener = null;

        protected override void Initialize()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var productItem in ProductCatalog.LoadDefaultCatalog().allValidProducts)
            {
                builder.AddProduct(productItem.id, productItem.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void SetIListener(IListener iListener)
        {
            _iListener = iListener;
        }

        public Product GetProduct(string productId)
        {
            if (_iStoreCtr == null)
                return null;

            return _iStoreCtr.products.WithID(productId);
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

            var product = purchaseEvent?.purchasedProduct;
            if(product == null)
            {
                Toast.Get?.Show("product is null");

                return PurchaseProcessingResult.Pending;
            }

            Game.UIManager.Instance?.DeactivateScreenSaver();

            //if (_buyShopData == null)
            //    return PurchaseProcessingResult.Pending;

            //_endBuyAction?.Invoke();
            _iListener?.SuccessPurchase(product);

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

            //_initializeStore = true;
        }

        void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("OnPurchaseFailed = " + failureDescription.message);
            Game.UIManager.Instance?.DeactivateScreenSaver();

            _iListener?.FailPurchase(product);

            Game.Toast.Get?.Show(failureDescription?.message);
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed = " + error);
        }

       
        #endregion

        //void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        //{

        //}

        //void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        //{

        //}

        //void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        //{

        //}

        //PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        //{
        //    return PurchaseProcessingResult.Complete;
        //}

        //void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        //{

        //}

        //void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        //{
        //    _iStoreCtr = controller;
        //    _iExtensionProvider = extensions;
        //}
    }
}

