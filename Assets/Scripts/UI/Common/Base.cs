using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class BaseData
    {

    }

    public class Base : MonoBehaviour
    {
        [SerializeField]
        public RectTransform rootRectTm = null; 
        
        protected bool _isTouch = false;
        
        public virtual void Activate()
        {
            if (!rootRectTm)
                return;

            if (IsActivate)
                return;

            rootRectTm.SetActive(true);
        }

        public virtual void Deactivate()
        {
            if (!rootRectTm)
                return;

            if (!IsActivate)
                return;
            
            rootRectTm.SetActive(false);
        }
        
        public bool IsActivate
        {
            get
            {
                if (!rootRectTm)
                    return false;
            
                return rootRectTm.gameObject.activeSelf;
            }
        }
    }

    public abstract class Common<T> : Base where T : BaseData
    {
        protected BaseData _data = null;

        public virtual void Initialize(T data)
        {
            _data = data;
        }
    }

    public abstract class Base<T> : Base where T : BaseData
    {
        protected T _data = default(T);
        
        public virtual void Initialize(T data)
        {
            InternalInitialize(data: data);
        }

        public virtual IEnumerator CoInitialize(T data)
        {
            InternalInitialize(data: data);

            yield return null;
        }

        private void InternalInitialize(T data)
        {
            _isTouch = false;

            _data = data;
        }
    }
}

