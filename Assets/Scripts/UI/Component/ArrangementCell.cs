using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
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
            public Type.EMain EMain = Type.EMain.None; 
            public string Name = string.Empty;
            public Sprite IconSprite = null;
            public bool Lock = true;
        }
        
        public interface IListener
        {
            void Edit(int id);
        }

        [SerializeField] private TextMeshProUGUI nameTMP;

        [SerializeField] private RectTransform lockRootRectTm = null;
        [SerializeField] private Button arrangementBtn = null;
        // [SerializeField] private Button buyBtn = null;
        [SerializeField] private Image iconImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetNameTMP();
            SetIconImg();
            SetButtonState();

            UIUtils.SetActive(lockRootRectTm, _data.Lock);
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

            if (_data.Lock)
            {
                UIUtils.SetSilhouetteColorImg(iconImg);
            }
            else
            {
                UIUtils.SetOriginColorImg(iconImg);
            }
        }

        private void SetButtonState()
        {
            // UIUtils.SetActive(buyBtn?.gameObject, _data.Lock);
            UIUtils.SetActive(arrangementBtn?.gameObject, !_data.Lock);
        }

        private void Unlock()
        {
            if (_data == null)
                return;

            if (Enum.TryParse(_data.EMain.ToString(), out Type.EOpen eOpen))
            {
                var openCondition = MainGameManager.Instance?.OpenCondition;
                if (openCondition == null)
                    return;
                
                if (openCondition.CheckOpenCondition(eOpen, _data.Id))
                {
                    new PopupCreator<Unlock, Unlock.Data>()
                        .SetData(new Unlock.Data()
                        {
                            EMain = _data.EMain,
                            Id = _data.Id,
                            ClickAction = () =>
                            {
                        
                            },
                        })
                        .SetCoInit(true)
                        .Create();
                }
            }
        }

        public void OnClickUnlock()
        {
            Unlock();
        }
        
        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.Id);
        }
    }
}

