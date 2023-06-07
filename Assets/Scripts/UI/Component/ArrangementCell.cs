using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class ArrangementCell : UI.Base<ArrangementCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public int Id = 0;
            public string Name = string.Empty;
            public Sprite IconSprite = null;
        }

        public interface IListener
        {
            void Edit(int id);
        }

        [SerializeField] private TextMeshProUGUI nameTMP;

        [SerializeField] private RectTransform lockRootRectTm = null;
        [SerializeField] private Button buyBtn = null;
        [SerializeField] private Image iconImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetNameTMP();
            SetIconImg();
            SetButtonState();
        }

        private void SetNameTMP()
        {
            nameTMP?.SetText(_data.Name);
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = _data.IconSprite;
        }

        private void SetButtonState()
        {
            UIUtils.SetActive(buyBtn?.gameObject, false);
            
        }

        public void OnClickUnlock()
        {
            
        }
        
        public void OnClickBuy()
        {
            
        }

        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.Id);
        }
    }
}

