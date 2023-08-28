using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public class IdleAction : AnimalAction
    {
        protected override string ActionName => "Idle";

        private System.DateTime _time;
        private float _duration = 0;

        public override void StartAction()
        {
            base.StartAction();

            _time = System.DateTime.UtcNow;
            _duration = Random.Range(5f, 10f);

            if(_data != null &&
               _data.Tm)
            {
                _data.Tm.localPosition = new Vector3(_data.Tm.localPosition.x, _data.Tm.localPosition.y, _initPosZ);
            }

            SetState(EState.InProgress);
        }

        protected override void EndAction()
        {
            base.EndAction();
        }

        public override void ChainUpdate()
        {
            if (_data == null)
                return;

            if (_data.EState == EState.End)
                return;

            var animator = _data?.Animator;
            if (animator == null)
                return;

            if ((System.DateTime.UtcNow - _time).TotalSeconds < _duration)
                return;

            EndAction();
        }
    }
}
