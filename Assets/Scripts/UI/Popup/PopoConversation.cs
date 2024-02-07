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
        private RectTransform bottomRectTm = null;
        [SerializeField]
        private TextMeshProUGUI bottomDescTMP = null;
        [SerializeField]
        private RectTransform clickRectTm = null;

        private bool _possibleTouch = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void AllDeactivate()
        {
            UIUtils.SetActive(topRectTm, false);
            UIUtils.SetActive(bottomRectTm, false);
        }

        public void ActivateTop(string sentence, bool autoDeactivate)
        {
            AllDeactivate();

            UIUtils.SetActive(clickRectTm, true);
            UIUtils.SetActive(topRectTm, true);

            SetDescAsync(topDescTMP, sentence).Forget();

            if (autoDeactivate)
            {
                UIUtils.SetActive(clickRectTm, false);

                DeactivateTopAsync().Forget();
            }
        }

        private async UniTask DeactivateTopAsync()
        {
            await UniTask.WaitForSeconds(3.5f);

            UIUtils.SetActive(topRectTm, false);
        }

        public void ActivateBottom(string sentence)
        {
            AllDeactivate();

            UIUtils.SetActive(clickRectTm, true);
            UIUtils.SetActive(bottomRectTm, true);

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

