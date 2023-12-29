using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ButtonEffectPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource = null;

        public void Play()
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            if (!setting.OnEffect)
                return;

            audioSource?.Play();
        }
    }
}

