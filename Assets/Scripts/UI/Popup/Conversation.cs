using System.Collections;
using System.Collections.Generic;

using UI;
using UnityEngine;

using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

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
            public float KeepDelay = 3.5f;

            public bool isPlayer = false;

            public void Initialize()
            {
                isPlayer = false;

                if (string.IsNullOrEmpty(Speaker))
                {
                    Speaker = PlayerPrefs.GetString(Games.Data.PlayPrefsKeyNickName);
                    isPlayer = true;
                }

                if (string.IsNullOrEmpty(SpeakerSpriteName))
                {
                    SpeakerSpriteName = "StoryIcon_Map01_Player";
                    isPlayer = true;
                }
            }
        }

        [SerializeField]
        private TextMeshProUGUI speakerTMP = null;
        [SerializeField]
        private TextMeshProUGUI typingTMP = null;
        [SerializeField]
        private UnityEngine.UI.Image speakerImg = null;
        [SerializeField]
        private TextMeshProUGUI cntTMP = null;
        [SerializeField]
        private UnityEngine.UI.Button clickBtn = null;

        private CancellationTokenSource _typingCancellationTokenSource = null;
        private CancellationTokenSource _keepDelayCancellationTokenSource = null;
        private Queue<Constituent> _constituentQueue = new();

        private int _allCnt = 0;
        private int _cnt = 0;
        private bool _isTyping = false;

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

        private void SetSpeakerImg(string spriteName, bool isPlayer)
        {
            UIUtils.SetActive(speakerImg?.rectTransform, false);

            if (string.IsNullOrEmpty(spriteName))
                return;

            if (speakerImg == null)
                return;

            Sprite speakerSprite = null;
            if(isPlayer)
            {
                speakerSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetSprite("Icon", spriteName);
            }
            else
            {
                speakerSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetAnimalIconSprite(spriteName);
            }

            if (speakerSprite == null)
                return;

            speakerImg.sprite = speakerSprite;
            speakerImg.SetNativeSize();

            UIUtils.SetActive(speakerImg?.rectTransform, true);
        }

        private async UniTask StartTypingAsync(Constituent constituent)
        {
            _keepDelayCancellationTokenSource = new CancellationTokenSource();

            await TypingAsync(constituent);

            float keepDelay = 0;
            while (keepDelay < constituent.KeepDelay)
            {
                await UniTask.Yield();

                keepDelay += Time.deltaTime;

                if (_keepDelayCancellationTokenSource.IsCancellationRequested)
                    break;
            }

            FinishTyping();
        }

        private async UniTask TypingAsync(Constituent constituent)
        {
            _isTyping = true;

            _typingCancellationTokenSource = new CancellationTokenSource();

            SetEmpty();

            if(constituent != null)
            {
                constituent.Initialize();

                speakerTMP?.SetText(constituent.Speaker);
                SetSpeakerImg(constituent?.SpeakerSpriteName, constituent.isPlayer);
            }

            foreach (var typingChr in constituent.Sentence)
            {
                await UniTask.WaitForSeconds(0.02f);

                if (_typingCancellationTokenSource != null &&
                    _typingCancellationTokenSource.IsCancellationRequested)
                {
                    typingTMP?.SetText(constituent.Sentence);

                    break;
                }

                typingTMP?.SetText(typingTMP.text + typingChr);
            }

            _isTyping = false;
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

            StartTypingAsync(constituent).Forget();

            if (_allCnt > 0)
            {
                cntTMP?.SetText(string.Format("{0}/{1}", ++_cnt, _allCnt));
            }
        }

        public void SetAllCnt(int allCnt)
        {
            _allCnt = allCnt;
        }

        public void OnClick()
        {
            if(_isTyping)
            {
                _typingCancellationTokenSource?.Cancel();
                //_typingCancellationTokenSource?.Dispose();
            }
            else
            {
                if (_cnt >= _allCnt)
                    return;

                _keepDelayCancellationTokenSource.Cancel();
                //_keepDelayCancellationTokenSource.Dispose();
            }
        }
    }
}

