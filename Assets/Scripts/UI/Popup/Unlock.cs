using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI
{
    public class Unlock : BasePopup<Unlock.Data>
    {
        [SerializeField] private Image iconImg = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;

        public class Data : BaseData
        {
            public Type.EMain EMain = Type.EMain.None;
            public int Id = 0;
            public Action ClickAction = null;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            SetIconImg();
            SetNameTMP();
        }

        public override void Activate()
        {
            base.Activate();
            
            
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            if (_data != null)
            {
                MainGameManager.Instance?.AddInfo(_data.EMain, _data.Id);
                
                _data?.ClickAction?.Invoke();
            }
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }

        public override bool End
        {
            get
            {
                return _endTask;
            }
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            if (iconImg == null)
                return;

            var sprite = GameUtils.GetLargeIconSprite(_data.EMain, _data.Id);
            iconImg.sprite = sprite;
        }

        private void SetNameTMP()
        {
            if (_data == null)
                return;
            
            nameTMP?.SetText(GameUtils.GetName(_data.EMain, _data.Id));
        }

        public void OnClick()
        {
            Deactivate();

            _endTask = true;
        }
    }
}

