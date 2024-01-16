using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;

using UnityEngine.Localization.Settings;

using UI.Component;
using GameSystem;

namespace UI
{
    public class Shop : BasePopup<Shop.Data_>, ShopItemCell.IListener, BuyCash.IListener, Game.Manager.IAP.IListener
    {
        public class Data_ : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;

        //private bool _initializeStore = false;

        private Data.Shop _buyShopData = null;
        private System.Action _endBuyAction = null;

        public override IEnumerator CoInitialize(Data_ data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            //InitializeIAP();

            yield return StartCoroutine(CoSetItemList());

            InitializeChildComponent();

            yield return null;            
        }

        public override void Activate()
        {
            base.Activate();

            Game.Manager.IAP.Instance?.SetIListener(this);

            ActivateChildComponent(typeof(ShopItemCell));
            
            itemScrollRect?.ResetScrollPos();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        //private void InitializeIAP()
        //{
        //    _initializeStore = false;

        //    if (_iStoreCtr != null)
        //    {
        //        _initializeStore = true;

        //        return;
        //    }                

        //    ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //    foreach (var productItem in ProductCatalog.LoadDefaultCatalog().allValidProducts)
        //    {
        //        builder.AddProduct(productItem.id, productItem.type);
        //    }

        //    UnityPurchasing.Initialize(this, builder);
        //}

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
                    })
                    .SetRootRectTm(scrollContent)
                    .Create();
            }

            itemScrollRect?.ResetScrollPos();

            yield return null;
        }

        void ShopItemCell.IListener.Buy(Data.Shop shopData, Vector3 pos)
        {
            if (shopData == null)
                return;

            var uiMgr = Game.UIManager.Instance;
            if (uiMgr == null)
                return;

            _buyShopData = shopData;

            switch(shopData.EPayment)
            {
                case Game.Type.EPayment.Cash:
                    {
                        new PopupCreator<BuyCash, BuyCash.Data>()
                            .SetReInitialize(true)
                            .SetData(new BuyCash.Data()
                            {
                                IListener = this,
                                Cash = shopData.PaymentValue,
                                targetSprite = ResourceManager.Instance?.AtalsLoader?.GetShopItemSprite(shopData.ECategory, shopData.IconImg),
                            })
                            .Create();

                        _endBuyAction = () =>
                        {
                            uiMgr.Top?.CollectCurrency(pos, shopData.ECategory == Game.Type.ECategory.AnimalCurrency ? Game.Type.EElement.Animal : Game.Type.EElement.Object, shopData.Value);

                            Info.UserManager.Instance?.User?.SetCash(-shopData.PaymentValue);

                            ITop iTop = uiMgr.Top;
                            iTop?.SetCurrency();
                        };

                        break;
                    }

                case Game.Type.EPayment.Money:
                    {
                        _endBuyAction = () =>
                        {
                            uiMgr.Top?.CollectCashCurrency(pos, shopData.Value);
                        };

                        uiMgr?.ActivateSreenSaver();

                        break;
                    }
            }
        }

        #region BuyCash.IListener,
        void BuyCash.IListener.Buy(bool possible)
        {
            if(!possible)
            {
                var text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_jewel", LocalizationSettings.SelectedLocale);
                Game.Toast.Get?.Show(text);

                return;
            }

            _endBuyAction?.Invoke();
        }
        #endregion

        #region Game.Manager.IAP.IListener
        void Game.Manager.IAP.IListener.SuccessPurchase(Product product)
        {
            _endBuyAction?.Invoke();
        }

        void Game.Manager.IAP.IListener.FailPurchase(Product product)
        {

        }
        #endregion
    }
}
