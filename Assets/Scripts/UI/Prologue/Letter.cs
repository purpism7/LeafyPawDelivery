using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Letter : Game.Common, Conversation.IListener
    {
        public interface IListener
        {
            void End();
        }

        [SerializeField] private Conversation conversation = null;

        private IListener _iListener = null;

        public void Initialize(IListener iListener)
        {
            _iListener = iListener;

            conversation?.Initialize(new Conversation.Data()
            {
                IListener = this,
            });
        }

        public override void Begin()
        {
            base.Begin();
           
            conversation?.Add(new Conversation.Constituent()
            {
                Speaker = PlayerPrefs.GetString(Game.Data.KeyNickName),
                Sentence = "먼저 어디?",
                KeepDelay = 3f,
            });
            conversation?.StartTyping();
        }

        void Conversation.IListener.FinishTyping()
        {
            _iListener?.End();

            _endTask = true;
        }
    }
}

