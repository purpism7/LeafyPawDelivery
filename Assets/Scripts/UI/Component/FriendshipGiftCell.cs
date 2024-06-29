using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
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

        private Sequence _sequence = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            DisableGiftImg();
            
            _sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(giftImg.transform.DOShakeScale(1f, 0.3f, 5));
            _sequence.SetLoops(-1);
        }

        public override void Activate(Data data)
        {
            base.Activate(data);
            
            pointTMP?.SetText($"{data.Point}");
            
            _sequence?.Restart();
        }

        private void DisableGiftImg()
        {
            if (giftImg == null)
                return;

            giftImg.color = new Color32(100, 100, 100, 255);



            // giftImg.transform.DOShakeScale(1f);
        }

        public void Onclick()
        {
            if (giftImg == null)
                return;

            btn.interactable = false;
            
            _sequence?.Kill();
            _sequence = null;
            
            giftImg?.ResetLocalScale();
        }
    }
}
