using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace GameSystem
{
    public class AnimalOpenCondition : GenericCondition<AnimalOpenCondition.Data>
    {
        public class Data : ConditionData
        {
            public int AnimalId = 0;
            public OpenCondition OpenCondition = null;
            public System.Action<int> OpenAction = null;
        }

        private Data _data = null;

        public override bool Init(Data data)
        {
            if(data == null)
            {
                return false;
            }

            _data = data;

            return true;
        }

        public override bool Check()
        {
            if(_data == null)
            {
                return false;
            }

            var openCondition = _data.OpenCondition;
            if (openCondition == null)
            {
                return false;
            }

            var userInfo = Info.UserManager.Instance?.User;
            if(userInfo == null)
            {
                return false;
            }

            if(openCondition.ReqLv > userInfo.Lv)
            {
                return false;
            }

            _data.OpenAction?.Invoke(_data.AnimalId);

            return true;
        }
    }
}

