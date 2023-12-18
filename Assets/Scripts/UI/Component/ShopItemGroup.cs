using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using TMPro;

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
            if (_data == null)
                return;

            foreach(var data in shopDataList)
            {
                if (data == null)
                    continue;

                if (!data.Show)
                    continue;

                var shopItemCellData = new ShopItemCell.Data_()
                {
                    iListener = _data.iShopItemCellListener,
                    shopData = data,
                };

                var cell = new ComponentCreator<ShopItemCell, ShopItemCell.Data_>()
                    .SetData(shopItemCellData)
                    .SetRootRectTm(itemRootRectTm)
                    .Create();
            }
        }
    }
}

