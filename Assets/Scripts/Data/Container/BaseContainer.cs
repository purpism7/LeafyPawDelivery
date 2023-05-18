using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public abstract class BaseContainer
    {
        public abstract void Init(string json);
    }

    public class BaseContainer<T> : BaseContainer
    {
        protected T[] _datas = null;

        public T[] GetDatas 
        {
            get
            {
                return _datas;
            }
        }

        public override void Init(string json)
        {
            var wrapper = JsonHelper.WrapperFromJson<T>(json);
            if(wrapper != null)
            {
                _datas = wrapper.Datas;
            }
        }
    }
}

