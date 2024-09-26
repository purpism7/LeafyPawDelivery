using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Component
{
    public class SkinCell : Base<SkinCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public AnimalSkin AnimalSkin = null;
            public Sprite Sprite = null;
            public ToggleGroup ToggleGroup = null;
            public bool ToggleOn = false;
        }

        public interface IListener
        {
            void Select(SkinCell skinCell);
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private Toggle toggle = null;
        [SerializeField]
        private RectTransform buyRootRectTm = null;
        [SerializeField]
        private OpenCondition openCondition = null;

        public int SkinId
        {
            get
            {
                var animalSkin = _data?.AnimalSkin;
                if (animalSkin == null)
                    return 0;

                return  _data.AnimalSkin.Id;
            }
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetToggle();
            SetIconImg();
            SetOpenCondition();

            EnableBuyRoot(false);   
        }

        public override void Activate()
        {
            base.Activate();

            openCondition?.Activate();
        }

        private void SetIconImg()
        {
            if (iconImg == null)
                return;

            if (_data == null)
                return;

            iconImg.sprite = _data.Sprite;
        }

        private void SetToggle()
        {
            if (toggle == null)
                return;

            if (_data == null)
                return;

            if (_data.ToggleGroup == null)
                return;

            toggle.group = _data.ToggleGroup;
            toggle.SetIsOnWithoutNotify(_data.ToggleOn);
        }

        private void SetOpenCondition()
        {
            var animalSkin = _data?.AnimalSkin;
            if (animalSkin == null)
                return;

            if (openCondition == null)
                return;

            var user = Info.UserManager.Instance?.User;
            long userCash = 0;
            if(user != null)
            {
                userCash = user.Cash;
            }

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = animalSkin.Cash.ToString(),
                PossibleFunc = () => userCash >= animalSkin.Cash,
                refreshLayout = false,
            };

            openCondition.Initialize(openConditionData);
        }

        public void EnableBuyRoot(bool enable)
        {
            GameUtils.SetActive(buyRootRectTm, enable);
        }

        public void OnValuChanged()
        {
            if (_data == null)
                return;

            if (toggle == null)
                return;

            if(toggle.isOn)
            {
                _data.IListener?.Select(this);
            }
            else
            {
                EnableBuyRoot(false);
            }

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }
    }
}

