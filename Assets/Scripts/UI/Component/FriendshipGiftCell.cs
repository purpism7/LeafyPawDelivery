using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class FriendshipGiftCell : BaseComponent<FriendshipGiftCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public int Point = 0;
            public int Index = 0;
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
        }

        public override void Activate(Data data)
        {
            base.Activate(data);
            
            pointTMP?.SetText($"{data.Point}");
         
            Refresh();
        }

        private void DisableGiftImg()
        {
            if (giftImg == null)
                return;

            giftImg.color = new Color32(100, 100, 100, 255);

            InteractableBtn(false);
        }

        private void EnableGiftImg()
        {
            if (giftImg == null)
                return;

            giftImg.color = Color.white;
        }

        private void InteractableBtn(bool interactable)
        {
            if (btn == null)
                return;

            btn.interactable = interactable;
        }

        private void ResetGiftImg()
        {
            _sequence?.Kill();
            // _sequence = null;
            
            giftImg?.ResetLocalScale();
        }

        public void Refresh()
        {
            DisableGiftImg();
            ResetGiftImg();
            
            if (_data == null)
                return;

            var animalInfo = MainGameManager.Get<AnimalManager>()?.GetAnimalInfo(_data.Id);
            if (animalInfo == null)
                return;

            if (animalInfo.FriendshipPoint < _data.Point)
                return;
            
            bool getReward = MainGameManager.Get<AnimalManager>().CheckGetFriendshipReward(_data.Id, _data.Index);
            
            EnableGiftImg();
            InteractableBtn(!getReward);

            if (!getReward)
            {
                _sequence = DOTween.Sequence()
                    .SetAutoKill(false)
                    .Append(giftImg.transform.DOShakeScale(1f, 0.3f, 5));
                _sequence.SetLoops(-1);
            }
        }

        public void Onclick()
        {
            if (giftImg == null)
                return;

            InteractableBtn(false);

            ResetGiftImg();
        }
    }
}
