using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UI.Component;
using GameSystem;

namespace UI
{
    public class Shop : BasePopup<Shop.Data_>, ShopItemCell.IListener
    {
        public class Data_ : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;

        private AdMob _adMob = null;

        public override IEnumerator CoInitialize(Data_ data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            SetItemList();

            _adMob = AdMob.Create();

            yield return null;
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void SetItemList()
        {
            var shopListDic = ShopContainer.Instance?.ShopListDic;
            if (shopListDic == null)
                return;

            var scrollContent = itemScrollRect?.content;
            if (scrollContent == null)
                return;

            foreach (var data in shopListDic)
            {
                var dataList = data.Value;

                if (dataList == null)
                    continue;

                var cell = new ComponentCreator<ShopItemGroup, ShopItemGroup.Data_>()
                    .SetData(new ShopItemGroup.Data_()
                    {
                        eCategory = dataList.Count > 0 ? dataList[0].ECategory : Game.Type.ECategory.None,
                        shopDataList = dataList,
                    })
                    .SetRootRectTm(scrollContent)
                    .Create();
            }

            itemScrollRect?.ResetScrollPos();
        }

        void ShopItemCell.IListener.Buy(Data.Shop shopData, System.Action endAction)
        {
            if (shopData == null)
                return;

            if(shopData.EPayment == Game.Type.EPayment.Advertising)
            {
                _adMob?.LoadRewardedInterstitialAd(shopData.Key);
            }
            else
            {

            }

            endAction?.Invoke();
        }
    }
}
