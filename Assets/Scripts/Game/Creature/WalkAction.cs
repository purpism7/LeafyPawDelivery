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
            Vector3 pos = Vector3.zero;
            var camera = MainGameManager.Instance?.GameCamera;
            if (camera)
            {
                pos = camera.transform.position + camera.transform.forward;
            }

            var halfWidth = Screen.width / 2f - 20f;
            var halfHeight = Screen.height / 2f;

            MoveToTarget(new Vector3(Random.Range(pos.x - halfWidth, pos.x + halfWidth), Random.Range(pos.y - halfHeight + 50f, pos.y)));
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

            animalTm.localPosition = Vector2.MoveTowards(animalTm.localPosition, _targetPos, Time.deltaTime * 50f);

            if(_data.SprRenderer != null)
            {
                _data.SprRenderer.sortingOrder = -(int)animalTm.localPosition.y;
            }

            var distance = Vector2.Distance(animalTm.localPosition, _targetPos);
            if (distance <= 0)
            {
                EndAction();
            }
        }
    }
}

