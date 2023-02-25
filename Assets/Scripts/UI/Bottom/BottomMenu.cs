using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class BottomMenu : Base<BottomMenu.Data>
    {
        public enum EType
        {
            None,

            Arrangement,
            Shop,
        }

        public interface IListener
        {
            void ClickBottomMenu();
        }

        public class Data : UI.Data
        {
            public IListener ILisener = null;
        }

        public override void Init(Data data)
        {
            base.Init(data);
        }

        public void OnClick(string eTypeStr)
        {
            if (System.Enum.TryParse(eTypeStr, out EType eType))
            {
                _data?.ILisener?.ClickBottomMenu();
            }
        }
    }
}

