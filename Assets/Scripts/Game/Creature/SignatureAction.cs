using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Creature
{
    public class SignatureAction : AnimalAction
    {
        protected override string ActionName => "Signature";

        private System.DateTime _time;
        private float _duration = 0;

        public override void StartAction()
        {
            base.StartAction();

            _time = System.DateTime.UtcNow;
            _duration = ClipLength * 2f - 0.1f;
            //Debug.Log("Duration = " + _duration);
            SetCurrenctPos();

            InProgressAction();
            
            UpdateAsync().Forget();
        }

        protected override void InProgressAction()
        {
            base.InProgressAction();
        }

        protected override void EndAction()
        {
            base.EndAction();
        }

        public override void ChainUpdate()
        {
            // if ((System.DateTime.UtcNow - _time).TotalSeconds < _duration)
            //     return;
            //
            // EndAction();
        }

        private async UniTask UpdateAsync()
        {
            await UniTask.WaitForSeconds(_duration);
            
            EndAction();
        }
    }
}
