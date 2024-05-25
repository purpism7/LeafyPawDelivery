using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing.Extension;
using System.Linq;

using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using UI.Component;
using GameSystem;
using Unity.VisualScripting;
using Product = UnityEngine.Purchasing.Product;
using Type = Game.Type;

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
        private Toggle[] tabToggles = null;
        
        [Header("Currency")]
        [SerializeField] private RectTransform currencyRootRectTm = null;
        [SerializeField] private ScrollRect currencyScrollRect = null;
        
        [Header("Item")]
        [SerializeField] private RectTransform itemRootRectTm = null;
        [SerializeField]
        private ScrollRect itemScrollRect = null;
        
        [SerializeField]
        private AD ad = null;

        private Game.Type.ETab _currETabType = Game.Type.ETab.Currency;
        private System.Action _endBuyAction = null;
        
        public override IEnumerator CoInitialize(Data_ data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            yield return StartCoroutine(CoSetList());

            InitializeChildComponent();         
        }

        public override void Activate()
        {
            base.Activate();
            
            _currETabType = Game.Type.ETab.Currency;
            
            Game.Timer.Get?.SetRootType(nameof(Shop));
            
            Game.Manager.IAP.Instance?.SetIListener(this);
            
            ActivateChildComponent<ShopItemGroup>();
            ActivateChildComponent<ShopItemCell>();

            ActiveContents();
            
            var tabToggle = tabToggles?.First();
            if (tabToggle != null)
            {
                tabToggle.SetIsOnWithoutNotify(true);
            }
            
            currencyScrollRect?.ResetScrollPos();
            itemScrollRect?.ResetScrollPos();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
      
        private IEnumerator CoSetList()
        {
            var shopListDic = ShopContainer.Instance?.ShopListDic;
            if (shopListDic == null)
                yield break;

            foreach (var keyPair in shopListDic)
            {
                var dataList = keyPair.Value;

                if (dataList == null)
                    continue;

                var data = dataList[0];
                if(data == null)
                    continue;

                RectTransform rootRectTm = null;

                if (data.ECategory == Type.ECategory.Cash ||
                    data.ECategory == Type.ECategory.AnimalCurrency ||
                    data.ECategory == Type.ECategory.ObjectCurrency)
                {
                    rootRectTm = currencyScrollRect?.content;
                }
                else if (data.ECategory == Type.ECategory.Gift)
                {
                    rootRectTm = itemScrollRect?.content;
                }

                var cell = new ComponentCreator<ShopItemGroup, ShopItemGroup.Data_>()
                    .SetData(new ShopItemGroup.Data_()
                    {
                        iShopItemCellListener = this,
                        eCategory = dataList.Count > 0 ? data.ECategory : Game.Type.ECategory.None,
                        shopDataList = dataList,
                        iShop = this,
                    })
                    .SetRootRectTm(rootRectTm)
                    .Create();
            }
        }

        private void SetItemList()

        {
            
        }

        #region IShop
        AD.Data IShop.GetADData(Game.Type.ECategory eCategory)
        {
            return ad?.GetData(eCategory);
        }
        #endregion
        
        private void BuyByCash(Data.Shop shopData, Vector3 pos)
        {
            var uiMgr = Game.UIManager.Instance;
            if (uiMgr == null)
                return;
            
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
        }

        private void BuyByMoney(Data.Shop shopData, Vector3 pos)
        {
            var uiMgr = Game.UIManager.Instance;
            if (uiMgr == null)
                return;
            
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
        }

        private void BuyByAdvertising(IShopItemCell iShopItemCell, Vector3 pos)
        {
            var uiMgr = Game.UIManager.Instance;
            if (uiMgr == null)
                return;
            
            var shopData = iShopItemCell?.ShopData;
            if (shopData == null)
                return;
            
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
        }
        
        void ShopItemCell.IListener.Buy(IShopItemCell iShopItemCell, Vector3 pos)
        {
            if (iShopItemCell == null ||
                iShopItemCell.ShopData == null)
                return;
            
            var shopData = iShopItemCell.ShopData;

            switch(shopData.EPayment)
            {
                case Game.Type.EPayment.Cash:
                {
                    BuyByCash(iShopItemCell?.ShopData, pos);

                    break;
                }

                case Game.Type.EPayment.Money:
                {
                    BuyByMoney(iShopItemCell?.ShopData, pos);
                    
                    break;
                }

                case Game.Type.EPayment.Advertising:
                { 
                    BuyByAdvertising(iShopItemCell, pos);
                    
                    break;
                }
            }
        }
        
        private async UniTask  ActiveContents()
        {
            UIUtils.SetActive(currencyRootRectTm, _currETabType == Game.Type.ETab.Currency);
            UIUtils.SetActive(itemRootRectTm, _currETabType == Game.Type.ETab.Item);
        }
        
        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Game.Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();

                EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
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
