using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace GameSystem
{
    public class ComponentCreator<T, V> : BaseCreator<T>  where T : UI.Base<V> where V : UI.BaseData 
    {
        private V _data = null;
        private RectTransform _rootRectTm = null;

        public ComponentCreator<T, V> SetData(V vData)
        {
            _data = vData;
            
            return this;
        }
        
        public ComponentCreator<T, V> SetRootRectTm(RectTransform rootRectTm)
        {
            _rootRectTm = rootRectTm;
            
            return this;
        }

        public override T Create()
        {
            var baseComponent = UIManager.Instance?.Instantiate<T>(_rootRectTm);
            if (baseComponent == null)
                return default(T);

            var component = baseComponent.GetComponent<T>();
            if (component == null)
                return default(T);
            
            component.Initialize(_data);

            return component;
        }
    }
}

