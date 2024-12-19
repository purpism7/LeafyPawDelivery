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
            public float gameCameraOrthographicSize = 0;
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
            string placeName = string.Empty;
            var placeData = MainGameManager.Get<Game.PlaceManager>()?.ActivityPlaceData;
            if (placeData != null)
            {
                placeName = placeData.ePlaceName.ToString();
            }

            placeNameTMP?.SetText(placeName);
        }

        public void PlayAnim(IGameCameraCtr iGameCameraCtr, System.Action endAction)
        {
            if (_data == null)
            {
                Deactivate();

                return;
            }
             
            if (iGameCameraCtr == null)
            {
                Deactivate();

                return;
            }

            _endAnim = false;
            _endAction = endAction;

            // iGameCameraCtr.MoveCenterGameCamera();
            iGameCameraCtr.SetOrthographicSize(iGameCameraCtr.MaxOrthographicSize);

            Sequence sequence = DOTween.Sequence()
               .SetAutoKill(false)
               .AppendCallback(() => StartCoroutine(CoFadeTextToFullAlpha()))
               // .AppendCallback(() => iGameCameraCtr.MoveCenterGameCamera())
               .Append(DOTween.To(() => iGameCameraCtr.MaxOrthographicSize, size => iGameCameraCtr.SetOrthographicSize(size), _data.gameCameraOrthographicSize, 2f).SetEase(Ease.OutCubic))
               .OnComplete(() =>
               {
                   // iGameCameraCtr.SetSize();

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

