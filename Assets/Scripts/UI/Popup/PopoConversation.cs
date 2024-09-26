using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Cysharp.Threading.Tasks;

namespace UI
{
    public class PopoConversation : BasePopup<PopoConversation.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
        }

        public interface IListener
        {
            void Click();
        }

        [SerializeField]
        private RectTransform topRectTm = null;
        [SerializeField]
        private TextMeshProUGUI topDescTMP = null;

        [SerializeField]
        private RectTransform centerRectTm = null;
        [SerializeField]
        private TextMeshProUGUI centerDescTMP = null;

        [SerializeField]
        private RectTransform bottomRectTm = null;
        [SerializeField]
        private TextMeshProUGUI bottomDescTMP = null;
        [SerializeField]
        private RectTransform clickRectTm = null;

        private bool _possibleTouch = false;
        private bool _playing = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void AllDeactivate()
        {
            GameUtils.SetActive(topRectTm, false);
            GameUtils.SetActive(centerRectTm, false);
            GameUtils.SetActive(bottomRectTm, false);
        }

        public void ActivateTop(string sentence, bool autoDeactivate)
        {
            AllDeactivate();

            GameUtils.SetActive(clickRectTm, true);
            GameUtils.SetActive(topRectTm, true);

            SetDescAsync(topDescTMP, sentence).Forget();

            if (autoDeactivate)
            {
                GameUtils.SetActive(clickRectTm, false);

                DeactivateTopAsync().Forget();
            }
        }

        private async UniTask DeactivateTopAsync()
        {
            await UniTask.WaitForSeconds(3.5f);

            GameUtils.SetActive(topRectTm, false);
        }

        public void ActivateCenter(string sentence)
        {
            AllDeactivate();

            GameUtils.SetActive(clickRectTm, true);
            GameUtils.SetActive(centerRectTm, true);

            SetDescAsync(centerDescTMP, sentence).Forget();
        }

        public void ActivateBottom(string sentence)
        {
            AllDeactivate();

            GameUtils.SetActive(clickRectTm, true);
            GameUtils.SetActive(bottomRectTm, true);

            SetDescAsync(bottomDescTMP, sentence).Forget();
        }

        private async UniTask SetDescAsync(TextMeshProUGUI tmp, string sentence)
        {
            _possibleTouch = false;

            tmp?.SetText(sentence);

            await UniTask.WaitForSeconds(1.5f);

            _possibleTouch = true;
        }

        public void OnClick()
        {
            if (!_possibleTouch)
                return;

            _data?.iListener?.Click();
        }
    }
}

