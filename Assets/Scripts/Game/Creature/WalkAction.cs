using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using GameSystem;

namespace Game.Creature
{
    public class WalkAction : AnimalAction
    {
        public new class Data : ActionData
        {
            public Vector3? TargetPos = null;
            public System.Action EndAction = null;
        }
        
        protected override string ActionName => "Walk";
        private Vector3 _targetPos = Vector3.zero;
        
        private Queue<Vector3> _posQueue = new();
        private Data _actionData = null;

        public override void SetActionData(ActionData data)
        {
            _actionData = data as Data;
        }
        
        public override void StartAction()
        {
            base.StartAction();

            Vector3? targetPos = null; 
            // if (_data != null &&
            //     _data.Tm)
            //     targetPos = _data.Tm.localPosition;

            if (_actionData != null &&
                _actionData.TargetPos != null)
                targetPos = _actionData.TargetPos;
            
            if (targetPos != null)
                MoveAsync(targetPos.Value).Forget();
            else
                EndAction();
        }

        protected override void InProgressAction()
        {
            base.InProgressAction();
        }

        protected override void EndAction()
        {
            _actionData?.EndAction?.Invoke();
            _actionData = null;
            
            base.EndAction();
        }

        private async UniTaskVoid MoveAsync(Vector3 targetPos)
        {
            if (_data == null ||
               !_data.Tm)
            {
                EndAction();
                return;
            }

            List<Vector3> pathPosList = await Carrier.MoveAsync(_data.Tm.localPosition, targetPos);
            
            if (_actionData != null &&
                _actionData.TargetPos != null)
                pathPosList?.Add(targetPos);
            
            if (pathPosList == null)
            {
                EndAction();
                return;
            }

            _randomSeed = GameUtils.RandomSeed;
            _randomSeed *= 1000f;

            _posQueue.Clear();

            foreach (Vector3 pos in pathPosList)
            {
                _posQueue.Enqueue(pos);
            }

            if (_posQueue.Count > 0)
            {
                InProgressAction();
                MoveToTarget();
            }
            else
                EndAction();
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

            _data.SprRenderer.flipX = animalTm.localPosition.x - _targetPos.x < 0;

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

            var animalTm = _data.Tm;
            if (!animalTm)
                return;
            
            Vector3 movePos = Vector2.MoveTowards(animalTm.localPosition, _targetPos, Time.deltaTime * 50f);
            movePos.z = movePos.y * GameUtils.PosZOffset + _randomSeed;

            animalTm.localPosition = movePos;

#if UNITY_EDITOR
            if(_posQueue != null)
            {
                var list = new List<Vector3>();
                list.AddRange(_posQueue);

                for(int i = 0; i < list.Count; ++i)
                {
                    var start = i <= 0 ? animalTm.localPosition : list[i - 1];
                    var end = list[i];

                    Debug.DrawLine(start, end, Color.cyan);
                }
            }
#endif

            if(_data.SprRenderer != null)
                _data.SprRenderer.sortingOrder = -(int)animalTm.localPosition.y;

            var distance = Vector2.Distance(animalTm.localPosition, _targetPos);
            if (distance <= 0.1f)
            {
                if (!MoveToTarget())
                    EndAction();
            }
        }
    }
}

