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

        public class Constituent
        {
            public string Speaker = string.Empty;
            public string Sentence = string.Empty;
            public float KeepDelay = 1.5f;
        }

        [SerializeField] private TextMeshProUGUI typingTMP = null;

        private YieldInstruction _waitSec = new WaitForSeconds(0.02f);
        private Queue<Constituent> _constituentQueue = new();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            _constituentQueue?.Clear();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _constituentQueue?.Clear();
        }

        public override void Activate()
        {
            base.Activate();

            typingTMP?.SetText(string.Empty);
        }

        private IEnumerator CoTyping(Constituent constituent)
        {
            typingTMP?.SetText(string.Empty);

            foreach (var typingChr in constituent.Sentence)
            {
                yield return _waitSec;

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            yield return new WaitForSeconds(constituent.KeepDelay);

            FinishTyping();
        }

        private void FinishTyping()
        {
            _data?.IListener?.FinishTyping();

            StartTyping();
        }

        public void Clear()
        {
            _constituentQueue?.Clear();
        }

        public void Add(Constituent constituent)
        {
            _constituentQueue?.Enqueue(constituent);
        }

        public void StartTyping()
        {
            if (_constituentQueue == null)
                return;

            if (_constituentQueue.Count <= 0)
                return;

            var constituent = _constituentQueue.Dequeue();
            if (constituent == null)
                return;

            StartCoroutine(CoTyping(constituent));
        }
    }
}

