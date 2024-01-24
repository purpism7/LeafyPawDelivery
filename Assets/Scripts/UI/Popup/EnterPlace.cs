using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using DG.Tweening;
using GameSystem;

namespace UI
{
    public class EnterPlace : BasePopup<EnterPlace.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private TextMeshProUGUI placeNameTMP = null;

        private bool _endAnim = false;
        private System.Action _endAction = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _endTask = false;
        }

        public override void Activate()
        {
            base.Activate();

            SetPlaceName();
            SetTextToZeroAlpha();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            SetTextToZeroAlpha();
        }

        private void SetPlaceName()
        {
            var placeData = Game.Data.Const.ActivityPlaceData;

            placeNameTMP?.SetText(placeData.ePlaceName.ToString());
        }

        public void PlayAnim(IGameCameraCtrProvider iGameCameraCtrProvider, System.Action endAction)
        {
            if (iGameCameraCtrProvider == null)
            {
                Deactivate();

                return;
            }

            _endAnim = false;
            _endAction = endAction;

            iGameCameraCtrProvider.SetOrthographicSize(iGameCameraCtrProvider.MaxOrthographicSize);

            Sequence sequence = DOTween.Sequence()
               .SetAutoKill(false)
               .AppendCallback(() => StartCoroutine(CoFadeTextToFullAlpha()))
               .Join(DOTween.To(() => iGameCameraCtrProvider.MaxOrthographicSize, size => iGameCameraCtrProvider.SetOrthographicSize(size), iGameCameraCtrProvider.DefaultOrthographicSize, 2f).SetEase(Ease.Linear))
               .OnComplete(() =>
               {
                   iGameCameraCtrProvider.SetSize();

                   _endAnim = true;
               });
            sequence.Restart();
        }

        private void FinishAnim()
        {
            _endAction?.Invoke();

            Deactivate();

            _endTask = true;
        }

        private void SetTextToZeroAlpha()
        {
            if (placeNameTMP == null)
                return;

            var color = placeNameTMP.color;
            color.a = 0;
            placeNameTMP.color = color;
        }

        public IEnumerator CoFadeTextToFullAlpha() // 알파값 0에서 1로 전환
        {
            if (placeNameTMP == null)
                yield break;

            while (placeNameTMP.color.a < 1.0f)
            {
                var color = placeNameTMP.color;
                color.a += Time.deltaTime * 0.5f;
                placeNameTMP.color = color;

                yield return null;
            }

            while(!_endAnim)
            {
                yield return null;
            }

            yield return null;

            FinishAnim();
        }
    }
}

