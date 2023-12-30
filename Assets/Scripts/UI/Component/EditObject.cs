using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class EditObject : UI.Base<EditObject.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public int ObjectId = 0;
            public int Count = 0;
            public int RemainCount = 0;
        }

        public interface IListener
        {
            void Select(int id);
        }

        [SerializeField] private Image iconImg = null;
        [SerializeField] private TextMeshProUGUI countTMP = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            SetIconImg();
            SetCount();
        }
        
        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(Game.Type.EElement.Object, _data.ObjectId);
        }

        private void SetCount()
        {
            if (_data == null)
                return;

            countTMP?.SetText(_data.RemainCount + "/" + _data.Count);
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            _data.iListener?.Select(_data.ObjectId);
        }
    }
}

