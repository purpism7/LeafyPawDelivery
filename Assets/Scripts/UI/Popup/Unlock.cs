using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private void SetIconImg()
        {
               
        }

        public void OnClick()
        {
            Deactivate();
        }
    }
}

