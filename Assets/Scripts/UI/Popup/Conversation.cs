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
            void FinishTyping(int remainCnt);
        }

        [System.Serializable]
        public class Constituent
        {
            public string Speaker = string.Empty;
            public string SpeakerSpriteName = string.Empty;
            public string Sentence = string.Empty;
            public float KeepDelay = 2f;
        }

        [SerializeField] private TextMeshProUGUI speakerTMP = null;
        [SerializeField] private TextMeshProUGUI typingTMP = null;
        [SerializeField]
        private UnityEngine.UI.Image speakerImg = null;

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

            Clear();
        }

        public override void Activate()
        {
            base.Activate();

            SetEmpty();
        }

        private void SetEmpty()
        {
            speakerTMP?.SetText(string.Empty);
            typingTMP?.SetText(string.Empty);
            UIUtils.SetActive(speakerImg?.transform, false);
        }

        private void SetSpeakerImg(string spriteName)
        {
            UIUtils.SetActive(speakerImg?.transform, false);

            if (string.IsNullOrEmpty(spriteName))
                return;

            if (speakerImg == null)
                return;

            var speakerSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetAnimalIconSprite(spriteName);
            if (speakerSprite == null)
                return;

            speakerImg.sprite = speakerSprite;
            speakerImg.SetNativeSize();

            UIUtils.SetActive(speakerImg?.transform, true);
        }

        private IEnumerator CoTyping(Constituent constituent)
        {
            SetEmpty();

            speakerTMP?.SetText(constituent.Speaker);
            SetSpeakerImg(constituent?.SpeakerSpriteName);

            Debug.Log("CoTyping = " + constituent.Sentence);
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
            _data?.IListener?.FinishTyping(_constituentQueue.Count);

            StartTyping();
        }

        public void Clear()
        {
            _constituentQueue?.Clear();
        }

        public void Enqueue(Constituent constituent)
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

