using Game.Creature;
using Game.Element.State;
using GameData;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Common;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Game
{
    [ExecuteAlways]
    public class UIManager : Singleton<UIManager>, IUpdater
    {
        [SerializeField]
        private RectTransform uiRootRectTm = null;
        [SerializeField] private RectTransform worldUIRootRectTr = null;
        [SerializeField] private RectTransform worldUIGameRootRectTr = null;
        [SerializeField] private RectTransform worldUISpeechBubbleRootRectTr = null;

        public UI.Top Top;
        public UI.Bottom Bottom = null;
        public UI.Popup Popup;

        [SerializeField]
        private RectTransform screenSaverRectTm = null;

        private Type.EScreenSaverType _eScreenSaverType = Type.EScreenSaverType.None;
        private List<IWorldUI> _sortedWorldUIList = new List<IWorldUI>();

        public RectTransform WorldUIRootRectTr => worldUIRootRectTr;
        public RectTransform WorldUIGameRootRectTr => worldUIGameRootRectTr;
        public RectTransform WorldUISpeechBubbleRootRectTr => worldUISpeechBubbleRootRectTr;

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
            var poolable = ObjectPooler.Instance.Get<T>();
            if(poolable != null)
                return poolable;

            var t = GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootTm);
            ObjectPooler.Instance.Add(t as IPoolable);

            return t;
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
                
                case Type.EScreenSaverType.InteractionAnimal:
                {
                    IPlace iPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
                    var findAnimal = iPlace?.AnimalList?.Find(animal => animal?.State != null && animal.State.CheckState(typeof(Interaction)));
                    if (findAnimal == null)
                        return;
                    
                    var localKey = "desc_interaction_animal";
                    var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                    Game.Toast.Get?.Show(string.Format(local, GameUtils.GetName(Type.EElement.Animal, findAnimal.Id, findAnimal.SkinId)), localKey);

                    break;
                }
            }
        }

        public void SortWorldUIDepth()
        {
            if (!worldUIGameRootRectTr)
                return;

            if (_sortedWorldUIList == null)
                return;

            _sortedWorldUIList.Clear();

            // 1. 대상 수집
            for (int i = 0; i < worldUIGameRootRectTr.childCount; i++)
            {
                var worldUI = worldUIGameRootRectTr.GetChild(i).GetComponent<IWorldUI>();
                if (worldUI != null)
                {
                    _sortedWorldUIList.Add(worldUI);
                }
            }

            // 2. Z값 기준으로 정렬 (멀리 있는 것을 먼저 그리려면 OrderByDescending)
            // 일반적으로 Z가 클수록 멀다면 -> OrderByDescending 사용 시 먼 것이 앞 인덱스(뒤쪽 렌더링)
            _sortedWorldUIList.Sort((a, b) => a.Order.CompareTo(b.Order));

            // 3. 인덱스 적용
            for (int i = 0; i < _sortedWorldUIList.Count; i++)
            {
                _sortedWorldUIList[i].Transform.SetSiblingIndex(i);
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

