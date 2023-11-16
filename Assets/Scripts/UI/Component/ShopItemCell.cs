using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Component
{
    public class ShopItemCell : BaseComponent<ShopItemCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public string IconImgName = string.Empty;
            public int Value = 0;
        }

        private GameSystem.AdMob _adMob = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void OnClick()
        {
            _adMob?.LoadRewardedInterstitialAd("ca-app-pub-3940256099942544/1712485313");
        }
    }
}

