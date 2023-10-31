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

        public bool IsUpdate { get; private set; } = false;

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

            _data.IListener?.StartAction(this);
        }

        protected virtual void InProgressAction()
        {
            var animator = _data?.Animator;
            if (animator == null)
                return;

            IsUpdate = true;

            animator.SetTrigger(ActionName);
        }

        protected virtual void EndAction()
        {
            IsUpdate = false;

            _data?.IListener?.EndAction(this);
        }

        //protected void SetState(EState eState)
        //{
        //    if(_data != null)
        //    {
        //        _data.EState = eState;
        //    }
        //}
    }
}

