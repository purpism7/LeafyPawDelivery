using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class ObjectCreator<T, V> : BaseCreator<T> where T : Game.BaseElement<V> where V : Game.BaseData
    {
        private V _data = null;
        private int _id = 0;
        private Transform _rootTm = null;
        private bool _isEdit = true;

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

        public ObjectCreator<T, V> SetIsEdit(bool isEdit)
        {
            _isEdit = isEdit;

            return this;
        }
        
        // public ObjectCreator<T, V> SetPlotData(PlotData )
        // {
        //     
        //
        //     return this;
        // }

        public override T Create()
        {
            var obj = ResourceManager.Instance.Instantiate<T>(_id, _rootTm);
            if (obj == null)
            {
                return default;
            }

            obj.Id = _id;
            obj.Initialize(_data);

            return obj;
        }
    }
}

