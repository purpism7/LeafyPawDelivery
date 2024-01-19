using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class EffectPlayer : MonoBehaviour
    {
        private static EffectPlayer _instance = null;
        public static EffectPlayer Get
        {
            get
            {
                return _instance;
            }
        }

        [System.Serializable]
        public class AudioClipData
        {
            public enum EType
            {
                None,

                TouchButton,
                TouchLetter,
                PickItem,
            }

            public EType eType = EType.None;
            public AudioClip audioClip = null;
        }

        [SerializeField]
        private AudioClipData[] audioClipDatas = null;

        private AudioSource _audioSource = null;

        private void Awake()
        {
            _instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClipData.EType eType)
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            if (!setting.OnEffect)
                return;

            if (eType == AudioClipData.EType.None)
                return;

            if (_audioSource == null)
                return;

            var audioClip = GetAudioClip(eType);
            if (audioClip == null)
                return;

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        private AudioClip GetAudioClip(AudioClipData.EType eType)
        {
            if (audioClipDatas == null)
                return null;

            foreach(var data in audioClipDatas)
            {
                if (data == null)
                    continue;

                if (data.eType != eType)
                    continue;

                return data.audioClip;
            }

            return null;
        }
    }
}

