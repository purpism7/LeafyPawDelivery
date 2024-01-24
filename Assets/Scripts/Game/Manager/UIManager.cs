using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;
using GameData;
using UI;

namespace Game
{
    [ExecuteAlways]
    public class UIManager : Singleton<UIManager>, IUpdater
    {
        [SerializeField]
        private RectTransform uiRootRectTm = null;
        public UI.Top Top;
        public UI.Bottom Bottom = null;
        public UI.Popup Popup;

        [SerializeField]
        private RectTransform screenSaverRectTm = null;

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(base.CoInit(iProvider));

            Top?.Initialize(null);

            Bottom?.Initialize(new UI.Bottom.Data()
            {
                PopupRootRectTm = Popup.popupRootRectTm,
            });

            yield return null;

            Debug.Log("End UIManager Initialize");
        }

        public T Instantiate<T>(RectTransform rootRectTm)
        {
            return GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootRectTm);
        }

        public void EnalbeUIRoot(bool enable)
        {
            UIUtils.SetActive(uiRootRectTm, enable);
        }

        public void ActivateAnim()
        {
            Top?.ActivateAnim(null);
            Bottom?.ActivateAnim(null);
        }

        public void DeactivateAnim()
        {
            Top?.DeactivateAnim(null);
            Bottom?.DeactivateAnim(null);
        }

        public void ActivateSreenSaver()
        {
            UIUtils.SetActive(screenSaverRectTm, true);
        }

        public void DeactivateScreenSaver()
        {
            UIUtils.SetActive(screenSaverRectTm, false);
        }

        #region IUpdater
        void IUpdater.ChainUpdate()
        {
            Top?.ChainUpdate();
            Bottom?.ChainUpdate();
            Popup?.ChainUpdate();

            return;
        }
        #endregion
    }
}

