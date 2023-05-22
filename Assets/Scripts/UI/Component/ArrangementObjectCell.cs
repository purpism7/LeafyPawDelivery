using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class ArrangementObjectCell : UI.Base<ArrangementObjectCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Object ObjectData = null;
            public int ObjectUId = 0;
        }

        public interface IListener
        {
            void EditObject(int objectUId);
        }

        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] private Image iconImg = null;

        public override void Init(Data data)
        {
            base.Init(data);

            SetNameTMP();
            SetIconImg();
        }

        private void SetNameTMP()
        {
            if (_data?.ObjectData == null)
                return;
            
            nameTMP?.SetText(_data.ObjectData.Name);
        }

        private void SetIconImg()
        {
            if (_data?.ObjectData == null)
                return;

            if (iconImg == null)
                return;

            iconImg.sprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetObjectIconSprite(_data.ObjectData.IconImgName);
        }

        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.EditObject(_data.ObjectUId);
        }
    }
}

