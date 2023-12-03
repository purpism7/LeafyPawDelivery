using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class BGMPlayer : MonoBehaviour
    {
        public enum EPlayType
        {
            Sequence,
            Random,
        }

        [SerializeField]
        private AudioSource audioSource = null;
        [SerializeField]
        private EPlayType ePlayType = EPlayType.Sequence;
        [SerializeField]
        private AudioClip[] audioClips = null;

        private Coroutine _playCoroutine = null;
        private int _sequenceIndex = 0;

        public void Play()
        {
            if(audioSource != null)
            {
                if (audioSource.isPlaying)
                    return;
            }

            if (audioClips == null)
                return;

            if (audioClips.Length <= 0)
                return;

            var audioClip = audioSource?.clip;
            if (audioClip == null)
            {
                SetAudioClip();
            }

            StopCoroutine();
            _playCoroutine = StartCoroutine(CoPlay());
        }

        public void Stop()
        {
            StopCoroutine();
            audioSource?.Stop();
        }

        public void Pause()
        {
            audioSource?.Pause();
        }

        private void finish()
        {
            SetAudioClip();
        }

        private void StopCoroutine()
        {
            if (_playCoroutine == null)
                return;

            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        private IEnumerator CoPlay()
        {
            float duration = audioSource.clip.length;
            audioSource?.Play();

            yield return new WaitForSeconds(duration);
            //yield return new WaitUntil(() => audioSource.isPlaying);

            finish();
        }

        //private async UniTask AsyncPlay()
        //{
        //    float duration = audioSource.clip.length;
        //    audioSource?.Play();

        //    await UniTask.WaitForSeconds(duration);

        //    finish();
        //}

        private void SetAudioClip()
        {
            if (audioSource == null)
                return;

            var audioClip = AudioClip;
            if (audioClip == null)
                return;

            audioSource.clip = audioClip;

            Play();
        }

        private AudioClip AudioClip
        {
            get
            {
                if (audioClips == null)
                    return null;

                if (audioClips.Length <= 0)
                    return null;

                switch (ePlayType)
                {
                    case EPlayType.Sequence:
                        {
                            ++_sequenceIndex;
                            if (audioClips.Length <= _sequenceIndex)
                            {
                                _sequenceIndex = 0;
                            }

                            return audioClips[_sequenceIndex];
                        }

                    case EPlayType.Random:
                        {
                            int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);

                            return audioClips[randomIndex];
                        }
                }

                return null;
            }
        }
    }
}

