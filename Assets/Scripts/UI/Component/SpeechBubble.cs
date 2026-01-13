using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using TMPro;
using Cysharp.Threading.Tasks;

using GameSystem;
using UI.Common;

namespace UI.Component
{
    public class SpeechBubble : BaseWorldUI<SpeechBubble.Data>
    {
        public class Data : BaseWorldUI<SpeechBubble.Data>.Data
        {
            //public string Sentence = string.Empty;
            public IListener IListener = null;
            public float PosY = 240f;
        }

        public interface IListener
        {
            void End();
        }

        public class Constituent
        {
            public string Sentence = string.Empty;
            public float KeepDelay = 2f;
            public System.Action EndAction = null;
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI sentenceTMP = null;
        #endregion

        private bool _isPlaying = false;
        private Queue<Constituent> _constituentQueue = new Queue<Constituent>();
        
        public IPoolable Poolable => this;

        //private void LateUpdate()
        //{
        //    ChainLateUpdate();
        //}
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            InitializePos();

            Clear();
        }

        public override void Activate()
        {
            base.Activate();

            SetOrder(9999);
            SetEmptyTMP();
        }

        private void InitializePos()
        {
            if (_data == null)
                return;

            transform.localPosition = new Vector3(transform.localPosition.x, _data.PosY, transform.localPosition.z);
        }

        private void SetEmptyTMP()
        {
            sentenceTMP?.SetText(string.Empty);
        }

        private void SetSentence(string sentence)
        {
            sentenceTMP?.SetText(sentence);
        }

        private void Clear()
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

            AsyncBegin(constituent).Forget();
            //StartCoroutine(CoBegin(constituent));
        }

        private async UniTask AsyncBegin(Constituent constituent)
        {
            Activate();
            SetSentence(constituent.Sentence);

            await UniTask.WaitForSeconds(constituent.KeepDelay);

            constituent?.EndAction?.Invoke();

            await UniTask.Delay(1);

            End();
        }

        //private IEnumerator CoBegin(Constituent constituent)
        //{
            
            
        //    yield return new WaitForSeconds(constituent.KeepDelay);

            

        //    yield return new WaitForEndOfFrame();

        //    End();
        //}

        private new void End()
        {
            _isPlaying = false;

            _data?.IListener?.End();

            Begin();
        }
    }
}
