using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            void Remove();
            void Arrange();
        }

        [SerializeField]
        private UnityEngine.UI.Button arrangeBtn = null;

        #region Inspector
        public RectTransform CanvasRectTm = null;
        #endregion

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void InteractableArrangeBtn(bool interactable)
        {
            if (arrangeBtn == null)
                return;

            arrangeBtn.interactable = interactable;
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
    }
}

