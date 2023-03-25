using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Edit : Base<Edit.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void Close();
        }

        public override void Init(Data data)
        {
            base.Init(data); 
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

