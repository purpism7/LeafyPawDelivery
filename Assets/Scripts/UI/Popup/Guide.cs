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
            
        }

        private void Finish()
        {
            _endTask = true;

            Deactivate();
        }

        private async UniTask AsyncTyping()
        {
            if (_data.sentenceQueue == null)
                return;

            if (_data.sentenceQueue.Count <= 0)
            {
                Finish();

                return;
            }

            typingTMP?.SetText(string.Empty);

            string sentence = _data.sentenceQueue.Dequeue();

            foreach(var typingChr in sentence)
            {
                await UniTask.WaitForSeconds(0.02f);

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            await UniTask.WaitForSeconds(1f);

            FinishTyping();

            await UniTask.WaitForSeconds(1.5f);

            AsyncTyping().Forget();
        }

        //public void OnClick()
        //{
        //    if (_isTyping)
        //        return;


        //    //AsyncTyping().Forget();
        //}
    }
}


