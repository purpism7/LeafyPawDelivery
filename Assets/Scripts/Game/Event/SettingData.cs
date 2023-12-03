using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public class SettingData : BaseData
    {
        public enum EType
        {
            None,

            BGM,
        }

        protected EType _eType = EType.None;

        public SettingData(EType eType)
        {
            _eType = eType;
        }

        public EType GetEType()
        {
            return _eType;
        }
    }

    public class BGMData : SettingData
    {
        public BGMData() : base(EType.BGM)
        {

        }

        public bool on = true;
    }
}
 
