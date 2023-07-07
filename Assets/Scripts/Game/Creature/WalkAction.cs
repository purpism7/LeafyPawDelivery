using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public class WalkAction : AnimalAction
    {
        protected override string ActionName => "Walk";

        private Vector3 _targetPos = Vector3.zero;

        public override void StartAction()
        {
            base.StartAction();

            MoveToTarget();
        }

        protected override void EndAction()
        {
            base.EndAction();
        }

        private void MoveToTarget()
        {
            MoveToTarget(new Vector3(Random.Range(-180, 180), Random.Range(-50, 100)));
        }

        private void MoveToTarget(Vector3 targetPos)
        {
            if(_data == null)
                return;

            if (_data.SprRenderer == null)
                return;

            var animalTm = _data.Tm;
            if (!animalTm)
                return;

            _data.SprRenderer.flipX = animalTm.localPosition.x - targetPos.x < 0;
            _targetPos = targetPos;

            SetState(EState.InProgress);
        }

        public override void ChainUpdate()
        {
            UpdateMove();
        }

        private void UpdateMove()
        {
            if (_data == null)
                return;

            if (_data.EState != EState.InProgress)
            {
                return;
            }

            var animalTm = _data.Tm;
            if (!animalTm)
            {
                return;
            }

            animalTm.localPosition = Vector2.MoveTowards(animalTm.localPosition, _targetPos, 1f);

            var distance = Vector2.Distance(animalTm.localPosition, _targetPos);
            if (distance <= 0)
            {
                EndAction();
            }
        }
    }
}

