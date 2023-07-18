using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Game;

namespace UI
{
    public abstract class BasePopup<T> : Base<T> where T : BaseData
    {
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
                .OnStart(() => { Activate();})
                .Append(rootRectTm.DOScale(Vector3.one * 0.5f, 0f))
                .Append(rootRectTm.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    
                });
            sequence.Restart();
        }
    }
}

