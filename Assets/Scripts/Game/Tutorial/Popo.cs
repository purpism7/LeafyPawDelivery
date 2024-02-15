using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 튜토리얼 용, 심플하게 만들기.
    public class Popo : Game.Base<Popo.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public Vector3 startPos = Vector3.zero;
        }

        public interface IListener
        {
            void EndMove();
            void EndSpeechBubble();
        }

        public enum EState
        {
            None,

            Idle,
            Walk,
        }

        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;
        [SerializeField]
        private UI.Component.SpeechBubble speechBubble = null;

        private EState _eState = EState.None;
        private Vector3 _targetPos = Vector3.zero;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            if(data != null)
            {
                transform.localPosition = data.startPos;
            }

            speechBubble?.Initialize(
                new UI.Component.SpeechBubble.Data()
                {
                    PosY = 130f,
                });
            speechBubble?.Deactivate();
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            UpdateMove();
        }

        private void Idle()
        {
            if (animator == null)
                return;

            _eState = EState.Idle;

            animator.SetTrigger(_eState.ToString());
        }

        #region Move
        private void UpdateMove()
        {
            if (!transform)
                return;

            if (_eState != EState.Walk)
                return;

            var movePos = Vector2.MoveTowards(transform.localPosition, _targetPos, Time.deltaTime * 300f);
            transform.localPosition = movePos;

            var distance = Vector2.Distance(transform.localPosition, _targetPos);
            if (distance <= 0)
            {
                EndMove();
            }
        }

        public void MoveToTarget(Vector3 targetPos)
        {
            if (!transform)
                return;

            if (animator == null)
                return;

            if (spriteRenderer == null)
                return;

            _eState = EState.Walk;
            _targetPos = targetPos;

            spriteRenderer.flipX = transform.localPosition.x - targetPos.x < 0;

            animator.SetTrigger(_eState.ToString());
        }

        private void EndMove()
        {
            Idle();

            _data?.iListener?.EndMove();
            //BeginSpeechBubble();
        }
        #endregion

        public void StartSpeechBubble(string sentence, float keepDelay)
        {
            speechBubble?.Enqueue(new UI.Component.SpeechBubble.Constituent()
            {
                Sentence = sentence,
                KeepDelay = keepDelay,
                EndAction = EndSpeechBubble,
            });

            speechBubble?.Begin();
        }

        private void EndSpeechBubble()
        {
            _data?.iListener?.EndSpeechBubble();
        }
    }
}
