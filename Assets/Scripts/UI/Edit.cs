using System.Collections;
using System.Collections.Generic;
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
            UIUtils.SetActive(editRootRectTm, true);
            DeactivateTop();
        }

        private void DeactivateBotom()
        {
            UIUtils.SetActive(editRootRectTm, false);  
        }

        public void ActivateTop()
        {
            UIUtils.SetActive(topRootRectTm, true);
            DeactivateBotom();
        }
        
        private void DeactivateTop()
        {
            UIUtils.SetActive(topRootRectTm, false);
        }

        public void DeactivateEdit()
        {
            DeactivateBotom();
            DeactivateTop();
        }

        public async UniTask ActivateTopAsync(System.Action action)
        {
            DeactivateBotom();
            UIUtils.SetActive(topRootRectTm, true);

            await UniTask.WaitForSeconds(3f);
            
            action?.Invoke();
            
            UIUtils.SetActive(topRootRectTm, false);
        }

        public void OnPressDownMove()
        {
            if(!_isMoving)
            {
                GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

                MainGameManager.Instance?.IGameCameraCtr.SetStopUpdate(true);

                UIUtils.SetActive(bottomBtnsRootRectTm, false);

                _data?.IListener?.Move(true);
            }

            _isMoving = true;
        }

        public void OnPressUpMove()
        {
            UIUtils.SetActive(bottomBtnsRootRectTm, true);

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
    }
}

