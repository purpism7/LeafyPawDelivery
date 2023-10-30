using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public abstract class AnimalAction : Action
    {
        public class Data : Action.Data<AnimalAction>
        {
            public SpriteRenderer SprRenderer = null;
        }

        protected Data _data = null;
        protected float _initPosZ { get; private set; } = 0;

        protected abstract string ActionName { get; }

        public AnimalAction Create(Data data)
        {
            _data = data;

            if(data.Tm)
            {
                _initPosZ = data.Tm.localPosition.z;
            }

            return this;
        }

        public abstract void ChainUpdate();

        public virtual void StartAction()
        {
            if (_data == null)
                return;

            var animator = _data?.Animator;
            if (animator == null)
                return;

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

