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

            public int Id = 0;
            public Type.EMain EMain = Type.EMain.None;
            public string Name = string.Empty;
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

            iconImg.sprite = GameUtils.GetLargeIconSprite(_data.EMain, _data.Id);
        }

        public void OnClick()
        {
            _data?.IListener?.Click();
        }
    }
}
