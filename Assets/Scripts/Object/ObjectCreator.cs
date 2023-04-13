using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class ObjectCreator<T, V> : BaseCreator<T> where T : Game.Base<V> where V : Game.BaseData
    {
        private V _data = null;
        private int _id = 0;
        private Transform _rootTm = null;

        public ObjectCreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public ObjectCreator<T, V> SetId(int id)
        {
            _id = id;

            return this;
        }

        public ObjectCreator<T, V> SetRootTm(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public override T Create()
        {
            var obj = ResourceManager.Instance.Instantiate<T>(_id, _rootTm);
            if (obj == null)
            {
                return default(T);
            }

            obj.Init(_data);

            return obj;
        }
    }
}

