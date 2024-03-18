using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

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

        private Type.EScreenSaverType _eScreenSaverType = Type.EScreenSaverType.None;

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
        }

        public T Instantiate<T>(RectTransform rootRectTm)
        {
            return GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootRectTm);
        }

        public void EnalbeUIRoot(bool enable)
        {
            UIUtils.SetActive(uiRootRectTm, enable);
        }

        public void ActivateAnim(System.Action completeAction)
        {
            Top?.ActivateAnim(completeAction);
            Bottom?.ActivateAnim(null);
        }

        public void DeactivateAnim()
        {
            Top?.DeactivateAnim(null);
            Bottom?.DeactivateAnim(null);
        }

        public void ActivateSreenSaver(Type.EScreenSaverType eScreenSaverType)
        {
            _eScreenSaverType = eScreenSaverType;

            UIUtils.SetActive(screenSaverRectTm, true);
        }

        public void DeactivateScreenSaver()
        {
            _eScreenSaverType = Type.EScreenSaverType.None;

            UIUtils.SetActive(screenSaverRectTm, false);
        }

        public void SetInteractable(bool interactable, Game.Type.EBottomType[] exceptBottomTypes = null)
        {
            Top?.SetInteractable(interactable);
            Bottom?.SetInteractable(interactable, exceptBottomTypes);
        }

        public void OnClickScreenSaver()
        {
            switch(_eScreenSaverType)
            {
                case Type.EScreenSaverType.InappPurchase:
                    {
                        string localKey = "desc_inapp_purchase";
                        var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                        Game.Toast.Get?.Show(local, localKey);

                        break;
                    }

                case Type.EScreenSaverType.ShowAD:
                    {
                        string localKey = "desc_show_ad";
                        var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                        Game.Toast.Get?.Show(local, localKey);

                        break;
                    }
            }
        }

        #region IUpdater
        void IUpdater.ChainUpdate()
        {
            //Bottom?.ChainUpdate();
            Popup?.ChainUpdate();
        }
        #endregion
    }
}

