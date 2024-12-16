using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI.Component
{
    public class GameTipCell : BaseComponent<GameTipCell.Data>
    {
        public class Data : BaseData
        {
            
        }

        private Transform _childRootTm = null;
        private int _selectIndex = 0;
        
        public override void Activate()
        {
            base.Activate();

            AllDeactive();
            SetChildByLocale();
            ActivateChild();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void SetChildByLocale()
        {
            if (!rootRectTm)
                return;
            
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (locales == null)
                return;
            
            int findIndex = locales.FindIndex(locale => locale.Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code);  
            if (locales.Count > findIndex)
            {
                _childRootTm = rootRectTm.GetChild(findIndex);
                GameUtils.SetActive(_childRootTm, true);
            }
        }
        
        private void AllDeactive()
        {
            if (!rootRectTm)
                return;
            
            for (int i = 0; i < rootRectTm.childCount; ++i)
            {
                GameUtils.SetActive(rootRectTm.GetChild(i), false);   
            }
        }

        private void ActivateChild()
        {
            if (!_childRootTm)
                return;
            
            for (int i = 0; i < _childRootTm.childCount; ++i)
            {
                GameUtils.SetActive(_childRootTm.GetChild(i),_selectIndex == i);
            }
        }

        public void SelectLeft()
        {
            _selectIndex -= 1;
            
            if (_selectIndex < 0)
                _selectIndex = 0;

            ActivateChild();
        }

        public void SelectRight()
        {
            if (!_childRootTm)
                return;
            
            _selectIndex += 1;
            if (_selectIndex >= _childRootTm.childCount - 1)
                _selectIndex = _childRootTm.childCount - 1;

            ActivateChild();
        }
        
        public bool CheckLeft()
        {
            if (!_childRootTm)
                return false;
            
            return _selectIndex > 0;
        }

        public bool CheckRight()
        {
            if (!_childRootTm)
                return false;
        
            return _selectIndex < _childRootTm.childCount - 1;
        }
    }
}


