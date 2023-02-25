using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class Fade : Base<Data>
    {
        readonly float Duration = 0.5f;

        public Image Img;

        public override void Init(Data data)
        {
            base.Init(data);

            Img?.CrossFadeAlpha(0, 0, true);
            UIUtils.SetActive(Img.transform, false);
        }

        public void Out(System.Action completeAction)
        {
            UIUtils.SetActive(Img.transform, true);

            FadeInOut(0.4f, Duration,
                () =>
                {
                    return;
                });
        }

        public void In(System.Action completeAction)
        {
            FadeInOut(0, Duration,
                () =>
                {
                    UIUtils.SetActive(Img.transform, false);
                });
        }

        private void FadeInOut(float alpha, float duration, System.Action completeAction)
        {
            if (Img == null)
            {
                return;
            }

            Sequence sequence = DOTween.Sequence()
                .Append(Img.DOFade(alpha, duration))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });

            sequence.Restart();
        }

        public void OnClick()
        {
            In(null);
        }
    }
}

