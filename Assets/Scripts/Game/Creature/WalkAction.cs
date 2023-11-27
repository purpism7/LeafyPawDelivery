using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public class WalkAction : AnimalAction
    {
        protected override string ActionName => "Walk";

        private Vector3 _targetPos = Vector3.zero;

        private Queue<Vector3> _posQueue = new();

        public override void StartAction()
        {
            base.StartAction();

            Move();
        }

        protected override void InProgressAction()
        {
            base.InProgressAction();
        }

        protected override void EndAction()
        {
            base.EndAction();
        }

        private void Move()
        {
            if (Carrier.Move(_data.Tm.localPosition, out List<Vector3> pathPosList))
            {
                _posQueue.Clear();

                foreach(Vector3 pos in pathPosList)
                {
                    _posQueue.Enqueue(pos);
                }

                if(_posQueue.Count > 0)
                {
                    InProgressAction();

                    MoveToTarget();
                }
                else
                {
                    EndAction();
                }
            }
            else
            {
                EndAction();
            }
        }

        private bool MoveToTarget()
        {
            if (_posQueue.Count <= 0)
                return false;

            if (_data == null)
                return false;

            if (_data.SprRenderer == null)
                return false;

            var animalTm = _data.Tm;
            if (!animalTm)
                return false;

            _targetPos = _posQueue.Dequeue();
            _targetPos.z = _initPosZ;

            _data.SprRenderer.flipX = animalTm.localPosition.x - _targetPos.x < 0;

            //SetState(EState.InProgress);

            return true;
        }

        public override void ChainUpdate()
        {
            UpdateMove();
        }

        private void UpdateMove()
        {
            if (_data == null)
                return;

            //if (_data.EState != EState.InProgress)
            //    return;

            var animalTm = _data.Tm;
            if (!animalTm)
                return;
            
            Vector3 movePos = Vector2.MoveTowards(animalTm.localPosition, _targetPos, Time.deltaTime * 50f);
            movePos.z = _initPosZ;
            animalTm.localPosition = movePos;
            //animalTm.localPosition = new Vector3(animalTm.localPosition.x, animalTm.localPosition.y, _initPosZ);

            Debug.DrawLine(animalTm.localPosition, _targetPos);

            if(_data.SprRenderer != null)
            {
                _data.SprRenderer.sortingOrder = -(int)animalTm.localPosition.y;
            }

            var distance = Vector2.Distance(animalTm.localPosition, _targetPos);
            if (distance <= 0)
            {
                if(!MoveToTarget())
                {
                    EndAction();
                }
            }
        }
    }
}

