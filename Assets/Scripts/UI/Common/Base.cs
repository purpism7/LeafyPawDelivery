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
        protected bool _isTouch = false;
    }

    public abstract class Common<T> : Base where T : BaseData
    {
        protected BaseData _data = null;

        public virtual void Init(T data)
        {
            _data = data;
        }
    }

    public abstract class Base<T> : Base where T : BaseData
    {       
        protected T _data = default(T);

        public virtual void Init(T data)
        {
            InternalInit(data: data);
        }

        public virtual IEnumerator CoInit(T data)
        {
            InternalInit(data: data);

            yield return null;
        }

        private void InternalInit(T data)
        {
            _isTouch = false;

            _data = data;
        }

        public virtual void Close()
        {
            transform.SetActive(false);
        }

        protected void ShowAnim()
        {
            GameSystem.UIManager.Instance?.Fade?.Out(null,
               () =>
               {
                   // HideAnim();
               });
        }

        //private void HideAnim()
        //{
        //    _isTouch = false;

        //    transform.SetActive(false);

        //    GameSystem.UIManager.Instance.Fade.In(null);
        //}
    }
}

