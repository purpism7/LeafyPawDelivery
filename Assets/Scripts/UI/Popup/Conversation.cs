using System.Collections;
using System.Collections.Generic;

using UI;
using UnityEngine;

using TMPro;

namespace UI
{
    public class Conversation : BasePopup<Conversation.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void FinishTyping();
        }

        [SerializeField] private TextMeshProUGUI typingTMP = null;

        private YieldInstruction _waitSec = new WaitForSeconds(0.05f);

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
        }

        public override void Activate()
        {
            base.Activate();

            typingTMP?.SetText(string.Empty);
        }

        private IEnumerator CoTyping(string typingStr)
        {
            foreach(var typingChr in typingStr)
            {
                yield return _waitSec;

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            _data?.IListener?.FinishTyping();
        }

        public void StartTyping(string typingStr)
        {
            typingTMP.SetText(string.Empty);

            StartCoroutine(CoTyping(typingStr));
        }
    }
}

