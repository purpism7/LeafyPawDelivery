using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UI.Component;
using GameSystem;

namespace UI
{
    public class Shop : BasePopup<Shop.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            SetItemList();

            yield return null;
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void SetItemList()
        {
            var shopDatas = ShopContainer.Instance?.Datas;
            if (shopDatas == null)
                return;

            foreach(var data in shopDatas)
            {
                if (data == null)
                    continue;

                var shopItemCellData = new ShopItemCell.Data()
                {
                    Id = data.Id,
                };


                var cell = new ComponentCreator<ShopItemCell, ShopItemCell.Data>()
                    .SetData(shopItemCellData)
                    .SetRootRectTm(itemScrollRect?.content)
                    .Create();
            }
        }
    }
}
