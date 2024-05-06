using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;

using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

using UI.Component;
using GameSystem;

namespace UI
{
    public interface IShop
    {
        AD.Data GetADData(Game.Type.ECategory eCategory);
    }

    public class Shop : BasePopup<Shop.Data_>, IShop, ShopItemCell.IListener, BuyCash.IListener, Game.Manager.IAP.IListener
    {
        public class Data_ : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;
        [SerializeField]
        private AD ad = null;

        private System.Action _endBuyAction = null;

        public override IEnumerator CoInitialize(Data_ data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            yield return StartCoroutine(CoSetItemList());

            InitializeChildComponent();         
        }

        public override void Activate()
        {
            base.Activate();

            Game.Manager.IAP.Instance?.SetIListener(this);

            ActivateChildComponent(typeof(ShopItemGroup));
            ActivateChildComponent(typeof(ShopItemCell));
            
            itemScrollRect?.ResetScrollPos();
        }

        public override void Deactivate()
        {
            base.Deactivate();
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

        #region IShop
        AD.Data IShop.GetADData(Game.Type.ECategory eCategory)
        {
            return ad?.GetData(eCategory);
        }
        #endregion

        void ShopItemCell.IListener.Buy(IShopItemCell iShopItemCell, Vector3 pos)
        {
            if (iShopItemCell == null ||
                iShopItemCell.ShopData == null)
                return;

            var uiMgr = Game.UIManager.Instance;
            if (uiMgr == null)
                return;

            var shopData = iShopItemCell.ShopData;

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
                            uiMgr.Top?.CollectCurrency(pos, shopData.ECategory == Game.Type.ECategory.AnimalCurrency ? Game.Type.EElement.Animal : Game.Type.EElement.Object, shopData.Value, false);

                            Info.UserManager.Instance?.User?.SetCash(-shopData.PaymentValue);

                            ITop iTop = uiMgr.Top;
                            iTop?.SetCurrency();
                        };

                        break;
                    }

                case Game.Type.EPayment.Money:
                    {
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            var localKey = "check_internet_connection";
                            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                            Game.Toast.Get?.Show(local, localKey);

                            return;
                        }

                        var iapManager = Game.Manager.IAP.Instance;
                        if (iapManager == null ||
                           !iapManager.ValidateIAP)
                        {
                            var localKey = "fail_initialize_inapp";
                            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                            Game.Toast.Get?.Show(local, localKey);

                            return;
                        }

                        _endBuyAction = () =>
                        {
                            uiMgr.Top?.CollectCashCurrency(pos, shopData.Value);
                        };

                        uiMgr?.ActivateSreenSaver(Game.Type.EScreenSaverType.InappPurchase);

                        break;
                    }

                case Game.Type.EPayment.Advertising:
                    {
                        var adData = ad?.GetData(shopData.ECategory);
                        if (adData == null)
                            return;

                        AdProvider.Get?.ShowAd(adData,
                            (rewardValue) =>
                            {
                                if(rewardValue > 0)
                                {
                                    if (shopData.ECategory == Game.Type.ECategory.Cash)
                                    {
                                        uiMgr.Top?.CollectCashCurrency(pos, shopData.Value);
                                    }
                                    else
                                    {
                                        var eElement = shopData.ECategory == Game.Type.ECategory.AnimalCurrency ? Game.Type.EElement.Animal : Game.Type.EElement.Object;
                                        uiMgr.Top?.CollectCurrency(pos, eElement, shopData.Value, false);
                                    }
                                }

                                iShopItemCell.End();
                            });

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
