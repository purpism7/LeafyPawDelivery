using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;

namespace GameSystem
{
    public class UICreator<T, V> : BaseCreator<T> where T : UI.Base<V> where V : BaseData
    {
        private V _data = null;
        private Transform _rootTm = null;

        public UICreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public UICreator<T, V> SetRooTm(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public override T Create()
        {
            var ui = Game.UIManager.Instance.Instantiate<T>(_rootTm);
            if (ui == null)
                return default(T);

            ui.Initialize(_data);

            return ui;
        }
    }
}
