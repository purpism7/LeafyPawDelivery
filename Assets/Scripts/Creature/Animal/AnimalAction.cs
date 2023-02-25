using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public abstract class AnimalAction : Action
    {
        public class Data : Action.Data<AnimalAction>
        {
            public SpriteRenderer SprRenderer = null;
        }

        protected AnimalAction.Data _data = null;

        protected abstract string ActionName { get; }

        public AnimalAction Create(AnimalAction.Data data)
        {
            _data = data;

            return this;
        }

        public abstract void ChainUpdate();

        public virtual void StartAction()
        {
            if (_data == null)
            {
                return;
            }

            var animator = _data?.Animator;
            if (animator == null)
            {
                return;
            }

            //animator.ResetTrigger(ActionName);
            animator.SetTrigger(ActionName);

            SetState(EState.Start);

            _data.IListener?.StartAction(this);
        }

        protected virtual void EndAction()
        {
            SetState(EState.End);

            _data?.IListener?.EndAction(this);
        }

        protected void SetState(EState eState)
        {
            if(_data != null)
            {
                _data.EState = eState;
            }
        }
    }
}

