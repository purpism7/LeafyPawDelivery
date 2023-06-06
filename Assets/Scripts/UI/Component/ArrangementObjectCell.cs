using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

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

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            simpleCell?.Initialize(new SimpleCell.Data()
            {
                IListener = this,
                Name =  _data?.ObjectData?.Name,
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

