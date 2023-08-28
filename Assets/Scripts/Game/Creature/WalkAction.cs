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
            var gameCameraCtr = MainGameManager.Instance?.GameCameraCtr;
            if (gameCameraCtr == null)
                return;

            var center = gameCameraCtr.Center;
            var halfWidth = (gameCameraCtr.Width - 200f) / 2f;
            var halfHeight = (gameCameraCtr.Height - 850f) / 2f;

            var randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);
            var randomY = Random.Range(center.y - halfHeight, center.y + halfHeight);
            randomY = Mathf.Clamp(randomY, gameCameraCtr.IGridProvider.LimitBottom.y, gameCameraCtr.IGridProvider.LimitTop.y);

            MoveToTarget(new Vector3(randomX, randomY, _initPosZ));
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
                return;

            var animalTm = _data.Tm;
            if (!animalTm)
                return;
            
            animalTm.localPosition = Vector2.MoveTowards(animalTm.localPosition, _targetPos, Time.deltaTime * 50f);
            //animalTm.localPosition = new Vector3(animalTm.localPosition.x, animalTm.localPosition.y, _initPosZ);

            Debug.DrawLine(animalTm.localPosition, _targetPos);

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

