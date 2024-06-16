using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public interface IGiftCell
    {
        
    }
    
    public class GiftCell : BaseComponent<GiftCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Sprite Sprite = null;
            public ToggleGroup ToggleGroup = null;
            public bool ToggleOn = false;
        }
        
        public interface IListener
        {
            void Select(IGiftCell iGiftCell);
        }
        
        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private Toggle toggle = null;
        [SerializeField]
        private RectTransform buyRootRectTm = null;
        [SerializeField]
        private OpenCondition openCondition = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetToggle();
            SetIconImg();
            SetOpenCondition();
        }

        public override void Activate()
        {
            
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
            // var animalSkin = _data?.AnimalSkin;
            // if (animalSkin == null)
            //     return;
            //
            // if (openCondition == null)
            //     return;
            //
            // var user = Info.UserManager.Instance?.User;
            // long userCash = 0;
            // if(user != null)
            // {
            //     userCash = user.Cash;
            // }
            //
            // var openConditionData = new OpenCondition.Data()
            // {
            //     ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
            //     Text = animalSkin.Cash.ToString(),
            //     PossibleFunc = () => userCash >= animalSkin.Cash,
            //     refreshLayout = false,
            // };
            //
            // openCondition.Initialize(openConditionData);
        }
    }
}

