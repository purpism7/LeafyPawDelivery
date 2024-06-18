using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class FriendshipGiftCell : BaseComponent<FriendshipGiftCell.Data>
    {
        public class Data : BaseData
        {
            public int Point = 0;
        }

        [SerializeField] 
        private Image giftImg = null;
        [SerializeField] 
        private TextMeshProUGUI pointTMP = null;
        [SerializeField] 
        private Button btn = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            DisableGiftImg();
        }

        public override void Activate(Data data)
        {
            base.Activate(data);
            
            pointTMP?.SetText($"{data.Point}");
        }

        private void DisableGiftImg()
        {
            if (giftImg == null)
                return;

            giftImg.color = new Color32(100, 100, 100, 255);
        }

        public void Onclick()
        {
            
        }
        
    }
}
