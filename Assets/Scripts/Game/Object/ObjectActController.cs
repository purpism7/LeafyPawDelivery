using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class ObjectActController : MonoBehaviour
    {
        private Animator _animator = null;
        
        public ObjectActController Initialize(Animator animator)
        {
            _animator = animator;

            return this;
        }

        private void PlayAnimation(string key)
        {
            _animator?.SetTrigger(key);
        }

        public void PlaySpecial(System.Action endAction)
        {
            if (_animator == null)
                return;
            
            PlaySpecialAsync(endAction).Forget();
        }

        private async UniTaskVoid PlaySpecialAsync(System.Action endAction)
        {
            PlayAnimation("Special");

            float length = _animator.GetCurrentAnimatorStateInfo(0).length;
            await UniTask.Delay(TimeSpan.FromSeconds(length * 3f));
            
            PlayAnimation("Idle");

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            endAction?.Invoke();
        }
    }
}

