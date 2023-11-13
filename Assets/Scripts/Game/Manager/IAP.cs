using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Game.Manager
{
    public class IAP : Singleton<IAP>, IDetailedStoreListener
    {
        private IStoreController _iStoreCtr = null;
        private IExtensionProvider _iExtensionProvider = null;

        protected override void Initialize()
        {
            InitializeIAP();
        }

        private void InitializeIAP()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            UnityPurchasing.Initialize(this, builder);
        }

        void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {

        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _iStoreCtr = controller;
            _iExtensionProvider = extensions;
        }
    }
}

