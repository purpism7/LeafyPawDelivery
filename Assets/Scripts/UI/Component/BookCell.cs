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

            iconImg.sprite = GameUtils.GetLargeIconSprite(_data.EMain, _data.Id);
            
            if (_data.Lock)
            {
                UIUtils.SetSilhouetteColorImg(iconImg);
            }
            else
            {
                UIUtils.SetOriginColorImg(iconImg);
            }
        }
        
        public void Unlock(Type.EMain eMain, int id)
        {
            if (_data == null)
                return;

            if (_data.EMain != eMain)
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
