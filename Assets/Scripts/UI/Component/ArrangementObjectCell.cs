using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class ArrangementObjectCell : UI.Base<ArrangementObjectCell.Data>, SimpleCell.IListener
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Object ObjectData = null;
            public int ObjectUId = 0;
        }

        public interface IListener
        {
            void Edit(int objectUId);
        }

        [SerializeField] private SimpleCell simpleCell = null;

        public override void Init(Data data)
        {
            base.Init(data);

            simpleCell?.Init(new SimpleCell.Data()
            {
                IListener = this,
                Name =  _data?.ObjectData?.Name,
                IconSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetObjectIconSprite(_data?.ObjectData?.IconImgName),
            });
        }
        
        #region  SimpleCell.IListener
        void SimpleCell.IListener.Click()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.ObjectUId);
        }
        
        #endregion
    }
}

