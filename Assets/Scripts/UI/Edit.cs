using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace UI
{
    public class Edit : Base<Edit.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void Move(bool isMoving);
            void Return();
            void Remove();
            void Arrange();

            void Conversation();
            void Special();
        }
        
        #region Inspector
        [SerializeField]
        private UnityEngine.UI.Button returnBtn = null;
        [SerializeField]
        private UnityEngine.UI.Button arrangeBtn = null;
        [SerializeField]
        private RectTransform bottomBtnsRootRectTm = null;
        [SerializeField]
        private RectTransform editRootRectTm = null;

        [SerializeField] 
        private RectTransform topRootRectTm = null;
        [SerializeField] 
        private RectTransform interactionRectTm = null;
        
        public RectTransform FriendshipPointRootRectTm = null;
        public RectTransform CanvasRectTm = null;
        #endregion

        private bool _isMoving = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _isMoving = false;
        }

        public void InteractableReturnBtn(bool interactable)
        {
            if (returnBtn == null)
                return;

            returnBtn.interactable = interactable;
        }

        public void InteractableArrangeBtn(bool interactable)
        {
            if (arrangeBtn == null)
                return;

            arrangeBtn.interactable = interactable;
        }

        public void ActivateBottom()
        {
            GameUtils.SetActive(editRootRectTm, true);
            DeactivateTop();
        }

        private void DeactivateBotom()
        {
            GameUtils.SetActive(editRootRectTm, false);  
        }

        private void ActivateTop()
        {
            GameUtils.SetActive(topRootRectTm, true);
        }
        
        private void DeactivateTop()
        {
            GameUtils.SetActive(topRootRectTm, false);
        }

        public void DeactivateEdit()
        {
            DeactivateBotom();
            DeactivateTop();
        }

        public async UniTask ActivateTopAsync(bool isInteraction, System.Action action)
        {
            DeactivateBotom();
            GameUtils.SetActive(interactionRectTm, isInteraction);
            ActivateTop();

            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            
            action?.Invoke();
            
            DeactivateTop();
        }

        public void OnPressDownMove()
        {
            if(!_isMoving)
            {
                GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

                MainGameManager.Instance?.IGameCameraCtr.SetStopUpdate(true);

                GameUtils.SetActive(bottomBtnsRootRectTm, false);

                _data?.IListener?.Move(true);
            }

            _isMoving = true;
        }

        public void OnPressUpMove()
        {
            GameUtils.SetActive(bottomBtnsRootRectTm, true);

            MainGameManager.Instance?.IGameCameraCtr.SetStopUpdate(false);
           
            _data?.IListener?.Move(false);

            _isMoving = false;
        }

        public void OnClickReturn()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

            _data?.IListener?.Return();
        }

        public void OnClickRemove()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

            _data?.IListener?.Remove();
        }

        public void OnClickArrange()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

            _data?.IListener?.Arrange();
        }

        public void OnClickConversation()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
            
            _data?.IListener?.Conversation();
            
            DeactivateEdit();
        }

        public void OnClickSpecial()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
            
            _data?.IListener?.Special();
            
            DeactivateEdit();
        }
    }
}

