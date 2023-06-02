using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class Fade : Common<BaseData>
    {
        #region Static
        private static Fade Fade_ = null;
        public static Fade Create()
        {
            Fade_ = GameSystem.ResourceManager.Instance.InstantiateUI<Fade>(null);
            if (Fade_ != null)
            {
                Fade_.transform.position = Vector3.zero;
                Fade_.Initialize(null);
            }

            return Fade_;
        }
        #endregion
        
        readonly float Duration = 0.5f;

        public Image Img;

        private System.Action _clickAction = null;

        public override void Initialize(BaseData data)
        {
            base.Initialize(data);

            Img?.CrossFadeAlpha(0, 0, true);
            UIUtils.SetActive(Img.transform, false);
        }

        public void Out(System.Action completeAction, System.Action clickAction)
        {
            _clickAction = clickAction;

            UIUtils.SetActive(Img.transform, true);

            FadeInOut(0.4f, Duration,
                () =>
                {
                    completeAction?.Invoke();

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

            _clickAction?.Invoke();
        }
    }
}

