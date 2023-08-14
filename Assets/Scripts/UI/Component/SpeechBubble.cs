using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using static Type;
using UnityEngine.Localization.Settings;

namespace UI.Component
{
    public class SpeechBubble : Base<SpeechBubble.Data>
    {
        public class Data : BaseData
        {
            //public string Sentence = string.Empty;
            public IListener IListener = null;
        }

        public interface IListener
        {
            void End();
        }

        public class Constituent
        {
            public string Sentence = string.Empty;
            public float KeepDelay = 2f;
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI sentenceTMP = null;
        #endregion

        private bool _isPlaying = false;
        private Queue<Constituent> _constituentQueue = new Queue<Constituent>();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            Clear();
        }

        public override void Activate()
        {
            base.Activate();

            SetEmptyTMP();
        }

        private void SetEmptyTMP()
        {
            sentenceTMP?.SetText(string.Empty);
        }

        public void SetSentence(string sentence)
        {
            sentenceTMP?.SetText(sentence);
        }

        public void Clear()
        {
            _constituentQueue?.Clear();
        }

        public void Enqueue(Constituent constituent)
        {
            _constituentQueue?.Enqueue(constituent);
        }

        public new void Begin()
        {
            if (_isPlaying)
                return;

            if (_constituentQueue == null)
            {
                Deactivate();

                return;
            }

            if (_constituentQueue.Count <= 0)
            {
                Deactivate();

                return;
            }

            var constituent = _constituentQueue.Dequeue();
            if (constituent == null)
                return;

            _isPlaying = true;

            StartCoroutine(CoBegin(constituent));
        }

        private IEnumerator CoBegin(Constituent constituent)
        {
            Activate();
            SetSentence(constituent.Sentence);
            Debug.Log("CoBegin begin = " + System.DateTime.Now.Second);
            yield return new WaitForSeconds(constituent.KeepDelay);
            Debug.Log("CoBegin end = " + System.DateTime.Now.Second);

            End();
        }

        private new void End()
        {
            _isPlaying = false;

            _data?.IListener?.End();

            Begin();
        }
    }

}
