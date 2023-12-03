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

        public override void Initialize(Data data)
        {
            base.Initialize(data);
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

        private void FinishTyping()
        {
            Debug.Log("FinishTyping");
            _endTask = true;
        }

        private async UniTask AsyncTyping()
        {
            if (_data.sentenceQueue == null)
                return;

            if (_data.sentenceQueue.Count <= 0)
            {
                FinishTyping();

                return;
            }

            string sentence = _data.sentenceQueue.Dequeue();

            foreach(var typingChr in sentence)
            {
                await UniTask.WaitForSeconds(0.02f);

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            await UniTask.WaitForSeconds(1f);

            AsyncTyping().Forget();
        }
    }
}


