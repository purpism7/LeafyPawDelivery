using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class Letter : Game.Common, Conversation.IListener
    {
        public interface IListener
        {
            void End();
        }

        [SerializeField] private Conversation conversation = null;
        [SerializeField] private Conversation.Constituent[] constituents = null;

        private IListener _iListener = null;

        public void Initialize(IListener iListener)
        {
            _iListener = iListener;

            conversation?.Initialize(new Conversation.Data()
            {
                IListener = this,
            });

            conversation?.SetAllCnt(constituents.Length);

            if (constituents != null)
            {
                foreach (var constituent in constituents)
                {
                    if (constituent == null)
                        continue;

                    conversation?.Enqueue(new Conversation.Constituent()
                    {
                        Speaker = GameSystem.Auth.NickName,
                        Sentence = LocalizationSettings.StringDatabase.GetLocalizedString("Story", constituent.Sentence, LocalizationSettings.SelectedLocale),
                        KeepDelay = constituent.KeepDelay,
                    });
                }
            }
        }

        public override void Begin()
        {
            base.Begin();

            conversation?.StartTyping();
        }

        private void EnqueueConversationConstituent(string sentenceKey, float keepDleay)
        {
           
        }

        void Conversation.IListener.FinishTyping(int remainCnt)
        {
            if (remainCnt > 0)
                return;

            _iListener?.End();

            _endTask = true;
        }
    }
}

