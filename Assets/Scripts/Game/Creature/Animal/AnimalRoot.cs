using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using UI.Component;
using UnityEngine.SocialPlatforms;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Creature
{
    public class AnimalRoot : MonoBehaviour, SpeechBubble.IListener
    {
        [SerializeField] 
        private RectTransform editRootRectTm = null;
        [SerializeField] 
        private RectTransform speechBubbleRootRectTm = null;
        [SerializeField] 
        private BaseLocalData[] localDatas = null;

        private SpeechBubble _speechBubble = null;
        private HeartCell _heartCell = null;
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

            var data = new UI.Component.SpeechBubble.Data
            {
                IListener = this,
                PosY = _animalHeigh,
            };
            data.WithTargetTm(speechBubbleRootRectTm)
                .WithOffset(new Vector2(0, 20f));
            
            _speechBubble = new GameSystem.ComponentCreator<UI.Component.SpeechBubble, UI.Component.SpeechBubble.Data>()
                .SetData(data)
                .SetRootRectTm(UIManager.Instance?.WorldUISpeechBubbleRootRectTr)
                .Create();

            ChangeLayersRecursively(_speechBubble.transform, LayerMask.NameToLayer("Game"));
        }

        private void ChangeLayersRecursively(Transform tm, int layer)
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
            Random.InitState(randomIndex);

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
                KeepDelay = UnityEngine.Random.Range(4f, 5f),
                EndAction = endAction,
            }) ;

            _speechBubble.Begin();
        }

        public void DeactivateSpeechBubble()
        {
            if (_speechBubble == null)
                return;
            
            _speechBubble?.Deactivate();
            ObjectPooler.Instance?.Return(_speechBubble?.Poolable);
            _speechBubble = null;
        }

        #endregion
        
        #region Friendship Point
        public void AddFriendshipPoint(int id, int point, RectTransform rootRectTm)
        {
            var data = new HeartCell.Data
            {
                Id = id,
                AddFriendShipPoint =  point,
            };
            
            if (_heartCell == null)
            {
                _heartCell = new ComponentCreator<HeartCell, HeartCell.Data>()
                    .SetRootRectTm(rootRectTm)
                    .Create();
            }
            
            MainGameManager.Get<AnimalManager>()?.AddFriendshipPoint(id, point);
            
            _heartCell?.Activate(data);
        }
        #endregion

        #region SpeechBubble.IListener
        void SpeechBubble.IListener.End()
        {

        }
        #endregion
    }
}

