using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using TMPro;
using System.Linq;

using GameSystem;

namespace UI.Component
{
    public class ShopItemGroup : BaseComponent<ShopItemGroup.Data_>
    {
        public class Data_ : BaseData
        {
            public ShopItemCell.IListener iShopItemCellListener = null;
            public Game.Type.ECategory eCategory = Game.Type.ECategory.None;
            public List<Data.Shop> shopDataList = null;
            public IShop iShop = null;
        }

        [SerializeField]
        private TextMeshProUGUI categoryTMP = null;
        [SerializeField]
        private RectTransform itemRootRectTm = null;

        public override void Initialize(Data_ data)
        {
            base.Initialize(data);

            SetCategory();
            SetItemList();
        }

        private void SetCategory()
        {
            if (_data == null)
                return;

            var category = LocalizationSettings.StringDatabase.GetLocalizedString("UI", _data.eCategory.ToString().ToLower(), LocalizationSettings.SelectedLocale);

            categoryTMP?.SetText(category);
        }

        private void SetItemList()
        {
            var shopDataList = _data?.shopDataList;
            if (shopDataList == null)
                return;

            var sortShopDatas = shopDataList.OrderBy(data => data.Order);

            foreach (var data in sortShopDatas)
            {
                if (data == null)
                    continue;

                if (!data.Show)
                    continue;

                var shopItemCellData = new ShopItemCell.Data_()
                {
                    iListener = _data.iShopItemCellListener,
                    shopData = data,
                    product = _data.iShop?.GetProduct(data.ProductId),
                };

                var cell = new ComponentCreator<ShopItemCell, ShopItemCell.Data_>()
                    .SetData(shopItemCellData)
                    .SetRootRectTm(itemRootRectTm)
                    .Create();
            }
        }
    }
}

