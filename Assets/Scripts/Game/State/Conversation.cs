using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

using UI;
using GameSystem;

namespace Game.State
{
    public class Conversation : Base, UI.Conversation.IListener
    {
        // private Vector3 _initCameraPos = Vector3.zero;

        public interface IListener
        {
            void Start();
            void Finish();
        }
        
        private UI.Conversation _conversation = null;
        private IListener _iListener = null;
        
        public override void Initialize(MainGameManager mainGameMgr)
        {
            _iListener = null;

            var conversationAnimal = MainGameManager.Get<AnimalManager>()?.Conversation;
            if (conversationAnimal == null)
                return;

            _iListener = conversationAnimal;
            
            var animalId = conversationAnimal.Id;

            SetAlpha(0.2f, animalId);
            
            var position = conversationAnimal.transform.position;
            position.y -= 5f;
            
            UIManager.Instance?.DeactivateAnim();
            MainGameManager.Instance?.IGameCameraCtr.ZoomIn(position,
                () =>
                {
                    StartConversation(animalId);
                });
        
            MainGameManager.Get<PlaceManager>()?.ActivityPlace?.Bust();
            
            _iListener?.Start();
        }
        
        public override void End()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;
            
            var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (activityPlace == null)
                return;
            
            _iListener?.Finish();
            
            SetAlpha(1f);
            
            mainGameMgr.IGameCameraCtr?.ZoomOut(
                () =>
                {
                    UIManager.Instance?.ActivateAnim(
                        () =>
                        {
                            activityPlace.Boom();
                        });
                });
        }

        private void SetAlpha(float alpha, int animalId = 0)
        {
            var placeMgr = MainGameManager.Get<PlaceManager>();
            if (placeMgr == null)
                return;
            
            placeMgr.SetAlphaActivityAnimal(alpha, animalId);
            placeMgr.SetAlphaActivityObject(alpha);
        }

        private void StartConversation(int animalId)
        {
            if (_conversation == null)
            {
                _conversation = new PopupCreator<UI.Conversation, UI.Conversation.Data>()
                    .SetShowBackground(false)
                    .SetData(new UI.Conversation.Data()
                    {
                        IListener = this,
                    })
                    .Create();
            }
            
            _conversation?.Activate();
                    
            var tables = LocalizationSettings.StringDatabase.GetTable("Conversation");
            foreach (var pair in tables)
            {
                if(pair.Value == null)
                    continue;

                var key = $"{animalId}_1_";
                        
                if(!pair.Value.Key.Contains(key))
                    continue;
                        
                _conversation?.Enqueue(
                    new UI.Conversation.Constituent()
                    {
                        SpeakerSpriteName = "-",
                        Speaker = GameUtils.GetName(Type.EElement.Animal, animalId, Games.Data.Const.AnimalBaseSkinId),
                        Sentence =  pair.Value.GetLocalizedString(),
                        KeepDelay = 2f,
                    });
            }
                    
            _conversation?.StartTyping();
        }

        void UI.Conversation.IListener.FinishTyping(int remainCnt)
        {
            if (remainCnt > 0)
                return;
            
            _conversation?.Deactivate();

            MainGameManager.Instance?.SetGameStateAsync(Type.EGameState.Game).Forget();
        }
    }
}


