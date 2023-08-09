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

        public class ConversationData
        {
            public string Speaker = string.Empty;
            public string Sentence = string.Empty;
            public float KeepDelay = 1.5f;
        }

        [SerializeField] private TextMeshProUGUI typingTMP = null;

        private YieldInstruction _waitSec = new WaitForSeconds(0.02f);
        private Queue<ConversationData> _conversationDataQueue = new();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            _conversationDataQueue?.Clear();
        }

        public override void Activate()
        {
            base.Activate();

            typingTMP?.SetText(string.Empty);
        }

        private IEnumerator CoTyping(ConversationData data)
        {
            typingTMP?.SetText(string.Empty);

            foreach (var typingChr in data.Sentence)
            {
                yield return _waitSec;

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            yield return new WaitForSeconds(data.KeepDelay);

            FinishTyping();
        }

        private void FinishTyping()
        {
            _data?.IListener?.FinishTyping();

            StartTyping();
        }

        public void Clear()
        {
            _conversationDataQueue?.Clear();
        }

        public void Add(ConversationData data)
        {
            _conversationDataQueue?.Enqueue(data);
        }

        public void StartTyping()
        {
            if (_conversationDataQueue == null)
                return;

            if (_conversationDataQueue.Count <= 0)
                return;

            var data = _conversationDataQueue.Dequeue();
            if (data == null)
                return;

            StartCoroutine(CoTyping(data));
        }
    }
}

