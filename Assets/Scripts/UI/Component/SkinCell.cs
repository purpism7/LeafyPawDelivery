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
            public int Id = 0;
            public Sprite Sprite = null;
            public ToggleGroup ToggleGroup = null;
        }

        public interface IListener
        {
            void Select(int id, bool isOn);
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private Toggle toggle = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetToggle();
            SetIconImg();
        }

        public override void Activate()
        {
            base.Activate();
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
            toggle.SetIsOnWithoutNotify(false);
        }

        public void OnValuChanged()
        {
            if (_data == null)
                return;

            if (toggle == null)
                return;

            _data.IListener?.Select(_data.Id, toggle.isOn);
        }
    }
}

