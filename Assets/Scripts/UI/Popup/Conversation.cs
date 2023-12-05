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

            public void Initialize()
            {
                if(string.IsNullOrEmpty(Speaker))
                {
                    Speaker = PlayerPrefs.GetString("KeyNickName");
                }

                if (string.IsNullOrEmpty(SpeakerSpriteName))
                {
                    SpeakerSpriteName = "StoryIcon_Map01_Player";
                }
            }
        }

        [SerializeField] private TextMeshProUGUI speakerTMP = null;
        [SerializeField] private TextMeshProUGUI typingTMP = null;
        [SerializeField]
        private UnityEngine.UI.Image speakerImg = null;
        [SerializeField]
        private TextMeshProUGUI cntTMP = null;

        private YieldInstruction _waitSec = new WaitForSeconds(0.02f);
        private Queue<Constituent> _constituentQueue = new();

        private int _allCnt = 0;
        private int _cnt = 0;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            Clear();
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
            cntTMP?.SetText(string.Empty);
        }

        private void SetEmpty()
        {
            speakerTMP?.SetText(string.Empty);
            typingTMP?.SetText(string.Empty);
            UIUtils.SetActive(speakerImg?.rectTransform, false);
        }

        private void SetSpeakerImg(string spriteName)
        {
            UIUtils.SetActive(speakerImg?.rectTransform, false);

            if (string.IsNullOrEmpty(spriteName))
                return;

            if (speakerImg == null)
                return;

            var speakerSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetAnimalIconSprite(spriteName);
            if (speakerSprite == null)
                return;

            speakerImg.sprite = speakerSprite;
            speakerImg.SetNativeSize();

            UIUtils.SetActive(speakerImg?.rectTransform, true);
        }

        private IEnumerator CoTyping(Constituent constituent)
        {
            SetEmpty();

            constituent?.Initialize();
            
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

            if(_allCnt > 0)
            {
                cntTMP?.SetText(string.Format("{0}/{1}", ++_cnt, _allCnt));
            }
        }

        public void SetAllCnt(int allCnt)
        {
            _allCnt = allCnt;
        }
    }
}

