using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class BookCell : UI.Base<BookCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public string Name = string.Empty;
            public Sprite IconSprite = null;
        }

        public interface IListener
        {
            void Click();
        }

        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private Image iconImg;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetNameTMP();
            SetIconImg();
        }

        private void SetNameTMP()
        {
            nameTMP?.SetText(_data.Name);
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            if (iconImg == null)
                return;

            iconImg.sprite = _data.IconSprite;
        }

        public void OnClick()
        {
            _data?.IListener?.Click();
        }
    }
}
