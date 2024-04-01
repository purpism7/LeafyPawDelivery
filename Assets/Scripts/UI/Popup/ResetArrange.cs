using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ResetArrange : UI.BasePopup<ResetArrange.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
        }

        public interface IListener
        {
            void Reset();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public override void Activate()
        {
            base.Activate();

            //Time.timeScale = 0;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            //_endTask = true;
        }

        public void OnClickCancel()
        {
            //Time.timeScale = 1;
            Deactivate();
        }

        public void OnClickReset()
        {
            _data?.iListener?.Reset();

            Deactivate();
        }
    }
}

