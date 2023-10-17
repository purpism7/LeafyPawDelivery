using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Game;

namespace UI
{
    public abstract class BasePopup<T> : Base<T> where T : BaseData
    {
        protected System.Action _componentActivateAction = null;

        protected void InitializeComponent()
        {
            _componentActivateAction = null;

            var baseComponents = rootRectTm.GetComponentsInChildren<Base>();

            foreach(var baseComponent in baseComponents)
            {
                if (baseComponent == null)
                    continue;

                _componentActivateAction += baseComponent.Activate;
            }
        }

        public override void Activate()
        {
            base.Activate();

            _componentActivateAction?.Invoke();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            UIManager.Instance?.Popup?.PopPopup();
        }

        public void AnimActivate()
        {
            if(!rootRectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .OnStart(() => { Activate(); })
                .Append(rootRectTm.DOScale(Vector3.one * 0.5f, 0f))
                .Append(rootRectTm.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutSine))
                .OnComplete(() =>
                {
                    
                });
            sequence.Restart();
        }
    }
}

