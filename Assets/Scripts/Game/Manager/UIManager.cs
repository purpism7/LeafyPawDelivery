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

        public RectTransform UIRootRectTm
        {
            get
            {
                return uiRootRectTm;
            }
        }

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

        public T Instantiate<T>(Transform rootTm)
        {
            return GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootTm);
        }

        // public void EnalbeUIRoot(bool enable)
        // {
        //     GameUtils.SetActive(uiRootRectTm, enable);
        // }

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

        public void ActivateSreenSaver(Type.EScreenSaverType eScreenSaverType = Type.EScreenSaverType.None)
        {
            _eScreenSaverType = eScreenSaverType;

            GameUtils.SetActive(screenSaverRectTm, true);
        }

        public void DeactivateScreenSaver()
        {
            _eScreenSaverType = Type.EScreenSaverType.None;

            GameUtils.SetActive(screenSaverRectTm, false);
        }

        public void SetInteractable(bool interactable, Game.Type.EBottomType[] exceptBottomTypes = null)
        {
            Top?.SetInteractable(interactable);
            Bottom?.SetInteractable(interactable, exceptBottomTypes);
        }

        public void OnClickScreenSaver()
        {
            switch (_eScreenSaverType)
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
                        var localKey = "check_internet_connection";
                        var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            Game.Toast.Get?.Show(local, localKey);

                            DeactivateScreenSaver();

                            return;
                        }

                        localKey = "desc_show_ad";
                        local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

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

