using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UI.Component;
using GameSystem;

namespace UI
{
    public class BuyCash : BasePopup<BuyCash.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public int Cash = 0;
            public Sprite targetSprite = null;
            public float scale = 1f;
        }

        public interface IListener
        {
            void Buy(bool possible);
        }

        [SerializeField]
        private Image buyTargetImg = null;
        [SerializeField]
        private Component.OpenCondition openCondition = null;

        private bool _possibleBuy = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetImg();
            SetOpenCondition();
        }

        public override void Activate()
        {
            base.Activate();

            openCondition?.Activate();
        }

        private void SetImg()
        {
            if (buyTargetImg == null)
                return;

            if (_data == null)
                return;

            buyTargetImg.sprite = _data.targetSprite;
            buyTargetImg.SetNativeSize();

            var rectTm = buyTargetImg.GetComponent<RectTransform>();
            if(rectTm)
            {
                rectTm.localScale = Vector3.one * _data.scale;
            }
        }

        private void SetOpenCondition()
        {
            if (_data == null)
                return;

            if (openCondition == null)
                return;

            var user = Info.UserManager.Instance?.User;
            long userCash = 0;
            if (user != null)
            {
                userCash = user.Cash;
            }

            _possibleBuy = userCash >= _data.Cash;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = _data.Cash.ToString(),
                PossibleFunc = () => _possibleBuy,
            };

            openCondition.Initialize(openConditionData);
        }

        public void OnClickCancel()
        {
            Deactivate();
        }

        public void OnClickBuy()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            _data?.IListener?.Buy(_possibleBuy);

            Deactivate();
        }
    }
}

