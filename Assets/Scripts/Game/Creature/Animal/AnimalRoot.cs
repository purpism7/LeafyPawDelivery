using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI.Component;
using UnityEngine.SocialPlatforms;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Creature
{
    public class AnimalRoot : MonoBehaviour, SpeechBubble.IListener
    {
        //public Transform RewardRootTm = null;
        [SerializeField] private RectTransform editRootRectTm = null;
        [SerializeField] private RectTransform speechBubbleRootRectTm = null;
        [SerializeField] private BaseLocalData[] localDatas = null;

        private SpeechBubble _speechBubble = null;
        private float _animalHeigh = 0;

        public RectTransform EditRootRectTm { get { return editRootRectTm; } }

        public void Initialize(float animalHeigh)
        {
            _animalHeigh = animalHeigh;
        }

        private void CreateSpeechBubble()
        {
            if (_speechBubble != null)
                return;

            _speechBubble = new GameSystem.ComponentCreator<UI.Component.SpeechBubble, UI.Component.SpeechBubble.Data>()
                .SetData(new UI.Component.SpeechBubble.Data()
                {
                    IListener = this,
                    PosY = _animalHeigh,
                })
                .SetRootRectTm(speechBubbleRootRectTm)
                .Create();

            ChangeLayersRecursively(_speechBubble.transform, LayerMask.NameToLayer("Game"));
        }

        public void ChangeLayersRecursively(Transform tm, int layer)
        {
            if (!tm)
                return;

            tm.gameObject.layer = layer;

            foreach (Transform childTm in tm)
            {
                ChangeLayersRecursively(childTm, layer);
            }
        }

        #region SpeechBubble
        public void ActivateSpeechBubble(System.Action endAction)
        {
            if (localDatas == null ||
                localDatas.Length <= 0)
            {
                endAction?.Invoke();

                return;
            }

            var randomIndex = UnityEngine.Random.Range(0, localDatas.Length);
            var localData = localDatas[randomIndex];
            if (localData == null)
            {
                endAction?.Invoke();

                return;
            }

            if (_speechBubble == null)
            {
                CreateSpeechBubble();
            }

            var sentence = LocalizationSettings.StringDatabase.GetLocalizedString(localData.TableName, localData.Key, LocalizationSettings.SelectedLocale);

            _speechBubble.Enqueue(new SpeechBubble.Constituent()
            {
                Sentence = sentence,
                KeepDelay = UnityEngine.Random.Range(3f, 5f),
                EndAction = endAction,
            }) ;

            _speechBubble.Begin();
        }

        public void DeactivateSpeechBubble()
        {
            _speechBubble?.Deactivate();
        }

        #endregion

        #region SpeechBubble.IListener
        void SpeechBubble.IListener.End()
        {

        }
        #endregion
    }
}

