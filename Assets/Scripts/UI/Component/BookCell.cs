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
            public Type.EElement EElement = Type.EElement.None;
            public string Name = string.Empty;
            public bool Lock = true;
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

            iconImg.sprite = GameUtils.GetLargeIconSprite(_data.EElement, _data.Id);
            
            if (_data.Lock)
            {
                UIUtils.SetSilhouetteColorImg(iconImg);
            }
            else
            {
                UIUtils.SetOriginColorImg(iconImg);
            }
        }
        
        public void Unlock(Type.EElement EElement, int id)
        {
            if (_data == null)
                return;

            if (_data.EElement != EElement)
                return;

            if (_data.Id != id)
                return;
            
            _data.Lock = false;
            
            SetIconImg();
        }

        public void OnClick()
        {
            _data?.IListener?.Click();
        }
    }
}
