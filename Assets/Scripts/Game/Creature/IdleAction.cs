using System.Collections;
using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
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

            SetCurrenctPos();

            InProgressAction();
            
            // UpdateAsync().Forget();
        }

        protected override void EndAction()
        {
            base.EndAction();
        }

        public override void ChainUpdate()
        {
            if ((System.DateTime.UtcNow - _time).TotalSeconds < _duration)
                return;
            
            EndAction();
        }
        
        // private async UniTask UpdateAsync()
        // {
        //     await UniTask.WaitForSeconds(_duration);
        //     
        //     EndAction();
        // }
    }
}
