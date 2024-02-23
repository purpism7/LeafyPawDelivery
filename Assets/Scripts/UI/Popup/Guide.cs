using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using TMPro;

namespace UI
{
    public class Guide : BasePopup<Guide.Data>
    {
        public class Data : BaseData
        {
            public Queue<string> sentenceQueue = null;
        }

        [SerializeField]
        private TextMeshProUGUI typingTMP = null;
        [SerializeField]
        private UnityEngine.UI.Button clickBtn = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _endTask = false;
        }

        public override void Activate()
        {
            base.Activate();

            if (_data != null)
            {
                AsyncTyping().Forget();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void SetInteractable(bool interactable)
        {
            if (clickBtn == null)
                return;

            clickBtn.interactable = interactable;
        }

        private void FinishTyping()
        {
            SetInteractable(true);
        }

        private void Finish()
        {
            _endTask = true;

            Deactivate();
        }

        private async UniTask AsyncTyping()
        {
            SetInteractable(false);

            if (_data.sentenceQueue == null)
                return;

            if (_data.sentenceQueue.Count <= 0)
            {
                Finish();

                return;
            }

            string sentence = _data.sentenceQueue.Dequeue();
            typingTMP?.SetText(sentence);

            //foreach (var typingChr in sentence)
            //{
            //    await UniTask.WaitForSeconds(0.02f);

            //    typingTMP?.SetText(typingTMP.text + typingChr);
            //}

            await UniTask.WaitForSeconds(1.5f);

            FinishTyping();
        }

        public void OnClick()
        {
            //if (_isTyping)
            //    return;

            AsyncTyping().Forget();
            //AsyncTyping().Forget();
        }
    }
}


