using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public abstract class AnimalAction : Action
    {
        public class Data : Action.Data<AnimalAction>
        {
            public int id = 0;
            public SpriteRenderer SprRenderer = null;
        }

        protected Data _data = null;
        protected float _randomSeed = 0;

        protected abstract string ActionName { get; }

        public bool IsUpdate { get; private set; } = false;

        public AnimalAction Create(Data data)
        {
            _data = data;

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

        protected float ClipLength
        {
            get
            {
                var animator = _data?.Animator;
                if (animator == null)
                    return 0;

                RuntimeAnimatorController animatorCtr = animator.runtimeAnimatorController;
                if (animatorCtr == null)
                    return 0;

                foreach(var clip in animatorCtr.animationClips)
                {
                    if (clip == null)
                        continue;

                    if (!clip.name.ToLower().Contains(ActionName.ToLower()))
                        continue;

                    return clip.length;
                }

                return 0;
            }
        }

        protected void SetCurrenctPos()
        {
            if (_data == null)
                return;

            if (!_data.Tm)
                return;

            float posZ = GameUtils.CalcPosZ(_data.Tm.localPosition.y);
            _data.Tm.localPosition = new Vector3(_data.Tm.localPosition.x, _data.Tm.localPosition.y, posZ);
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

