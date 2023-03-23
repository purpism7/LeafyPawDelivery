using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class ObjectCreator<T, V> : BaseCreator<T> where T : Game.Base<V> where V : Game.BaseData
    {
        private V _data = null;
        private int _id = 0;
        private RectTransform _rootRecTm = null;

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

        public override T Create()
        {
            var objectRootTm = GameManager.Instance?.ObjectRootTm;

            var popup = ResourceManager.Instance.Instantiate<T>(_id, objectRootTm);
            if (popup == null)
            {
                return default(T);
            }

            popup.Init(_data);

            return popup;
        }
    }
}

