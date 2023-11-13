using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Game.Manager
{
    public class IAP : Singleton<IAP>, IDetailedStoreListener
    {
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
            throw new System.NotImplementedException();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            throw new System.NotImplementedException();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new System.NotImplementedException();
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            throw new System.NotImplementedException();
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new System.NotImplementedException();
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            throw new System.NotImplementedException();
        }
    }
}

