using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class BookCell : BaseComponent<BookCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;

            public int Id = 0;
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public bool Lock = true;
        }

        public interface IListener
        {
            void Click(Game.Type.EElement eElment, int id);
        }

        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private Image iconImg;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetIconImg();
        }

        public override void Activate()
        {
            base.Activate();

            SetNameTMP();
        }

        private void SetNameTMP()
        {
            if (_data == null)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id, Game.Data.Const.AnimalBaseSkinId);

            nameTMP?.SetText(localName);
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
        
        public void Unlock(Game.Type.EElement EElement, int id)
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
            if (_data == null)
                return;

            _data.IListener?.Click(_data.EElement, _data.Id);
        }
    }
}
