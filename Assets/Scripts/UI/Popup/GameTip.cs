using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

using UI.Component;

namespace UI
{
    public class GameTip : BasePopup<GameTip.Data>
    {
        public class Data : BaseData
        {
            
        }

        [SerializeField] 
        private GameTipCell[] gameTipCells = null;
        [SerializeField] 
        private Button leftBtn = null;
        [SerializeField] 
        private Button rightBtn = null;

        private GameTipCell _currGameTipCell = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _currGameTipCell = gameTipCells[0];
            
            leftBtn?.onClick?.RemoveAllListeners();
            leftBtn?.onClick?.AddListener(OnClickLeft);
            
            rightBtn?.onClick?.RemoveAllListeners();
            rightBtn?.onClick?.AddListener(OnClickRight);
        }

        private void SetBtnState()
        {
            if (_currGameTipCell != null)
            {
                GameUtils.SetActive(leftBtn, _currGameTipCell.CheckLeft());
                GameUtils.SetActive(rightBtn, _currGameTipCell.CheckRight());
            }
        }

        public override void Activate()
        {
            base.Activate();

            _currGameTipCell?.Activate();

            SetBtnState();
        }

        private void OnClickClose()
        {
            
        }
        
        private void OnClickLeft()
        {
            GameUtils.SetActive(leftBtn, false);
            
            _currGameTipCell?.SelectLeft();
            if (_currGameTipCell == null)
                return;

            SetBtnState();
        }

        private void OnClickRight()
        {
            GameUtils.SetActive(rightBtn, false);
            
            _currGameTipCell?.SelectRight();
            if (_currGameTipCell == null)
                return;

            SetBtnState();
        }
    }
}

