using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Game;

namespace UI
{
    public abstract class BasePopup<T> : Base<T> where T : BaseData
    {
        protected Dictionary<System.Type, System.Action> _compActivateActionDic = new();

        protected void InitializeChildComponent()
        {
            _compActivateActionDic.Clear();

            var baseComponents = rootRectTm.GetComponentsInChildren<Base>();

            foreach(var baseComponent in baseComponents)
            {
                if (baseComponent == null)
                    continue;

                System.Type type = baseComponent.GetType();

                _compActivateActionDic.TryAdd(type, null);
                _compActivateActionDic[type] += baseComponent.Activate;
            }
        }

        protected void ActivateChildComponent(System.Type type)
        {
            var activateAction = _compActivateActionDic[type];
            activateAction?.Invoke();
        }

        public override void Activate()
        {
            base.Activate();
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
                .Append(rootRectTm.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart))
                .OnComplete(() =>
                {
                    
                });
            sequence.Restart();
        }
    }
}

