using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class Data
    {

    }

    public class Base : MonoBehaviour
    {
        protected bool _isTouch = false;
    }

    public abstract class Common<T> : Base where T : Data
    {
        protected Data _data = null;

        public virtual void Init(T data)
        {
            _data = data;
        }
    }

    public abstract class Base<T> : Base where T : Data
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

            ShowAnim();
        }

        public virtual void Close()
        {
            transform.SetActive(false);
        }

        private void ShowAnim()
        {
            GameSystem.UIManager.Instance.Fade.Out(null,
               () =>
               {
                   HideAnim();
               });

            //Sequence sequence = DOTween.Sequence()
            //    .Append(transform.DOScale(0, 0))
            //    .Append(transform.DOScale(1f, 0.1f).SetEase(Ease.Linear))
            //    .OnComplete(() =>
            //    {
            //        _isTouch = true;
            //    });
            //sequence.Restart();
        }

        private void HideAnim()
        {
            _isTouch = false;

            //Sequence sequence = DOTween.Sequence()
            //    .Append(transform.DoF(0f, 0.1f).SetEase(Ease.Linear))
            //    .OnComplete(() =>
            //    {
            //        Close();
            //    });
            //sequence.Restart();
            transform.SetActive(false);

            GameSystem.UIManager.Instance.Fade.In(null);
        }
    }
}

