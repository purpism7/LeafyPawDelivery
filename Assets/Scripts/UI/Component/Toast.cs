using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using TMPro;
using DG.Tweening;



namespace UI.Component
{
    public class Toast : Base<Toast.Data>
    {
        public

            class Data : BaseData
        {
            public string localKey = string.Empty;
        }

        [SerializeField]
        private TextMeshProUGUI textTMP = null;
        [SerializeField]
        private UnityEngine.UI.Image bgImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetText();

            AnimActiavte();

            //AsyncDelayDeactivate().Forget();
        }

        private void SetText()
        {
            if (_data == null)
                return;

            string text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", _data.localKey, LocalizationSettings.SelectedLocale);

            textTMP?.SetText(text);
        }

        private void AnimActiavte()
        {
            Sequence sequence = DOTween.Sequence()
              .SetAutoKill(false)
              .OnStart(() =>
              {
                  Activate();
                  bgImg.DOFade(0, 0);
              })
              .Append(bgImg.DOFade(1f, 0.2f).SetEase(Ease.Linear))
              .AppendInterval(2.5f)
              //.Append(rootRectTm.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart))
              .OnComplete(() =>
              {
                  AnimDeactive();
              });
            sequence.Restart();
        }

        private void AnimDeactive()
        {
            Sequence sequence = DOTween.Sequence()
             .SetAutoKill(false)
             .OnStart(() =>
             {
                 //Activate();
                 //bgImg.DOFade(0, 0);
             })
             .Append(bgImg.DOFade(0, 0.2f).SetEase(Ease.Linear))
             //.Append(rootRectTm.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart))
             .OnComplete(() =>
             {
                 Deactivate();
             });
            sequence.Restart();
        }
    }
}

