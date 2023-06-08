using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Unlock : Base<Unlock.Data>
    {
        [SerializeField] private Image iconImg = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        
        public class Data : BaseData
        {
            public Type.EOpen EOpenType = Type.EOpen.None;
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

        private void SetIconImg()
        {
               
        }

        public void OnClick()
        {
            _data?.ClickAction?.Invoke();
            
            Deactivate();
        }
    }
}

