using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;
using DG.Tweening;

using GameSystem;
using Game;
using Info;
using Type = Game.Type;

namespace UI.Component
{
    public interface IArrangementCell 
    {
        int Id { get; }
        Game.Type.EElement ElementType { get; }

        GameObject GameObj { get; }
        Transform Transform { get; }

        void Activate();
        void Deactivate();

        void SetAcitve(bool isActive);

        void SetIndex(int index);
        void SetIsTutorial(bool isTutorial);

        bool Obtain(Game.Type.EElement eElement, int id);
    }

    public abstract class ArrangementCell<TData> : UI.BaseComponent<TData>, IArrangementCell where TData : ArrangementCell<TData>.Data
        
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public int Id = 0;
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public string Name = string.Empty;
            public bool Owned = false;
            public bool Lock = true;
            public bool IsSpecialObject = false;
            public bool isTutorial = false;

            public int index = -1;
        }

        public interface IListener
        {
            void Edit(int id, int index);
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] protected TextMeshProUGUI descTMP = null;
        [SerializeField] private Button arrangementBtn = null;
        [SerializeField] private Image iconImg = null;

        [SerializeField]
        private RectTransform openRootRectTm = null;
        [SerializeField] protected RectTransform openConditionRootRectTm = null;
        [SerializeField] protected TextMeshProUGUI openNameTMP = null;
        [SerializeField] protected TextMeshProUGUI openDescTMP = null;

        [SerializeField] protected Image hiddenIconImg = null;
        [SerializeField]
        private Image storyIconImg = null;

        [SerializeField]
        private Image guideLineImg = null;

        [Header("Lock")]
        [SerializeField]
        private RectTransform lockRootRectTm = null;
        [SerializeField]
        private Image lockImg = null;
        [SerializeField]
        private Image unLockImg = null;
        [SerializeField]
        private Image lockBgImg = null;
        #endregion

        private List<OpenCondition> _openConditionList = new();

        public Transform Transform => transform;

        public override void Initialize(TData data)
        {
            base.Initialize(data);

            GameUtils.SetActive(openRootRectTm, !_data.Owned);
            GameUtils.SetActive(lockRootRectTm, IsLock);
            GameUtils.SetActive(lockImg, IsLock);
            GameUtils.SetActive(lockBgImg, IsLock);

            if (IsLock)
            {
                lockBgImg.DOFade(1f, 0);
                unLockImg.DOFade(1f, 0);
            }

            GameUtils.SetActive(unLockImg, false);

            SetDescTMP();
            SetElementIconImg();
            SetButtonState();
            SetOpenConditionData();
            SetStoryIcon();
            SetTutorial();
        }

        public override void Activate()
        {
            base.Activate();

            var localName = GameUtils.GetName(_data.EElement, _data.Id, Games.Data.Const.AnimalBaseSkinId);
            SetNameTMP(localName);

            ActivateOpenConditionList();

            if (_data != null)
            {
                // GameUtils.SetActive(specialObjectRectTm, _data.IsSpecialObject);
                GameUtils.SetActive(openConditionRootRectTm, !_data.IsSpecialObject);
            }
        }

        public override void Deactivate()
        {
            DeactivateOpenConditionList();

            base.Deactivate();
        }

        public void SetIndex(int index)
        {
            if (_data == null)
                return;

            _data.index = index;
        }

        protected virtual void SetNameTMP(string name)
        {
            if (_data == null)
                return;

            nameTMP?.SetText(name);
        }

        protected abstract void SetDescTMP();

        public bool Obtain(Game.Type.EElement eElement, int id)
        {
            if (_data == null)
                return false;

            Unlock();

            if (_data.EElement != eElement)
                return false;

            if (_data.Id != id)
                return false;

            _data.Owned = true;

            SetElementIconImg();
            SetButtonState();
            SetOpenConditionData();
            SetStoryIcon();

            GameUtils.SetActive(openRootRectTm, !_data.Owned);

            return true;
        }

        private void SetElementIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(_data.EElement, _data.Id);

            if (!_data.Owned)
            {
                UIUtils.SetSilhouetteColorImg(iconImg);
            }
            else
            {
                UIUtils.SetOriginColorImg(iconImg);
            }
        }

        private void SetButtonState()
        {
            arrangementBtn?.SetActive(!IsLock);
        }

        private void SetStoryIcon()
        {
            GameUtils.SetActive(storyIconImg, false);

            if (_data == null)
                return;

            if (_data.Owned)
                return;

            int placeId = GameUtils.ActivityPlaceId;

            var storyList = StoryContainer.Instance?.GetStoryList(placeId);
            if (storyList == null)
                return;

            var storyOpenConditionContainer = StoryOpenConditionContainer.Instance;
            if (storyOpenConditionContainer == null)
                return;

            foreach (var story in storyList)
            {
                if (story == null)
                    continue;

                var storyOpenCondition = storyOpenConditionContainer.GetData(story.Id);
                if (storyOpenCondition == null)
                    continue;

                if (storyOpenConditionContainer.CheckExistReqId(story.Id, placeId, _data.EElement, _data.Id))
                {
                    GameUtils.SetActive(storyIconImg, true);

                    break;
                }
            }
        }

        private void SetTutorial()
        {
            if (_data == null)
                return;

            guideLineImg?.SetActive(_data.isTutorial);
            if (_data.isTutorial)
            {
                guideLineImg?.StartBlink();
            }
        }

        #region Open Condition
        private void ActivateOpenConditionList()
        {
            if (_openConditionList == null)
                return;

            foreach (var openCondition in _openConditionList)
            {
                openCondition?.Activate();
            }
        }

        private void DeactivateOpenConditionList()
        {
            if (_openConditionList == null)
                return;

            foreach (var openCondition in _openConditionList)
            {
                openCondition?.Deactivate();
            }
        }

        protected virtual void SetOpenConditionData()
        {
            DeactiveAllOpenConditionList();

            GameUtils.SetActive(hiddenIconImg, false);

            openDescTMP?.SetText(string.Empty);
        }

        private void DeactiveAllOpenConditionList()
        {
            foreach (var openCondition in _openConditionList)
            {
                if (openCondition == null)
                    continue;

                openCondition.gameObject.SetActive(false);
            }
        }

        protected void AddOpenCondition(string spriteName, int currency, Func<bool> possibleFunc)
        {
            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(spriteName),
                Text = string.Format("{0}", currency),
                PossibleFunc = possibleFunc,
            };

            CreateOpenCondition(openConditionData);
        }

        private UI.Component.OpenCondition CreateOpenCondition(OpenCondition.Data openConditionData)
        {
            _openConditionList?.Reverse();

            UI.Component.OpenCondition openCondition = null;
            foreach (var component in _openConditionList)
            {
                if (component == null)
                    continue;

                if (component.gameObject.IsActive())
                    continue;

                openCondition = component;
            }

            if (openCondition != null)
            {
                openCondition.Initialize(openConditionData);
                openCondition.gameObject.SetActive(true);
            }
            else
            {
                openCondition = new ComponentCreator<OpenCondition, OpenCondition.Data>()
                    .SetData(openConditionData)
                    .SetRootRectTm(openConditionRootRectTm)
                    .Create();

                _openConditionList?.Add(openCondition);
            }

            return openCondition;
        }
        #endregion

        #region Lock

        protected abstract void Unlock();

        protected void ProcessUnlock()
        {
            if (IsLock)
                return;

            _openConditionList?.ForEach(
                openCondition =>
                {
                    openCondition?.Activate();
                });

            AnimUnlock();
        }

        protected bool IsLock
        {
            get
            {
                if (_data == null)
                    return true;

                return _data.Lock;
            }
        }

        private void AnimUnlock()
        {
            Sequencer.EnqueueTask(
                () =>
                {
                    Sequence sequence = DOTween.Sequence()
                        .SetAutoKill(false)
                        .OnStart(() => { _endTask = false; })
                        .AppendInterval(0.5f)
                        .AppendCallback(() => lockImg?.SetActive(false))
                        .AppendCallback(() => unLockImg?.SetActive(true))
                        .AppendInterval(0.4f)
                        .Append(lockBgImg.DOFade(0, 0.3f))
                        .Join(unLockImg.DOFade(0, 0.3f))
                        .OnComplete(() =>
                        {
                            GameUtils.SetActive(lockRootRectTm, IsLock);

                            _endTask = true;
                        });
                    sequence.Restart();

                    return this;
                });
        }
        #endregion

        protected void CreateObtainPopup()
        {
            if (_data == null)
                return;

            bool isPossibleObtain = false;
            OpenConditionData openConditionData = null;

            switch (_data.EElement)
            {
                case Game.Type.EElement.Animal:
                    {
                        isPossibleObtain = AnimalOpenConditionContainer.Instance.Check(_data.Id);
                        openConditionData = AnimalOpenConditionContainer.Instance.GetData(_data.Id);

                        break;
                    }

                case Game.Type.EElement.Object:
                    {
                        isPossibleObtain = ObjectOpenConditionContainer.Instance.Check(_data.Id);
                        openConditionData = ObjectOpenConditionContainer.Instance.GetData(_data.Id);

                        break;
                    }
            }

            if (openConditionData == null)
                return;

            if (!isPossibleObtain)
                return;

            var mainGameMgr = MainGameManager.Instance;

            int animalCurrency = openConditionData.AnimalCurrency;
            int objectCurrency = openConditionData.ObjectCurrency;

            var currency = new Info.User.Currency
            {
                PlaceId = GameUtils.ActivityPlaceId,
                Animal = -animalCurrency,
                Object = -objectCurrency,
            };

            if (!UserManager.Instance.CheckCurrency(currency))
                return;

            Sequencer.EnqueueTask(
                () =>
                {
                    var obtain = new PopupCreator<Obtain, Obtain.Data>()
                        .SetData(new Obtain.Data()
                        {
                            EElement = _data.EElement,
                            Id = _data.Id,
                            ClickAction = () =>
                            {

                            },
                        })
                        .SetCoInit(true)
                        .SetReInitialize(true)
                        .Create();

                    Info.UserManager.Instance?.SetCurrency(currency);

                    ITop iTop = Game.UIManager.Instance?.Top;
                    iTop?.SetCurrency();

                    mainGameMgr?.AddAcquireCurrency(Game.Type.EElement.Animal, Game.Type.EAcquireAction.Use, animalCurrency);
                    mainGameMgr?.AddAcquireCurrency(Game.Type.EElement.Object, Game.Type.EAcquireAction.Use, objectCurrency);

                    return obtain;
                });

            mainGameMgr?.Add(_data.EElement, _data.Id);
        }

        #region IArrangementCell
        int IArrangementCell.Id => _data?.Id ?? 0;
        Type.EElement IArrangementCell.ElementType => _data?.EElement ?? Type.EElement.None;

        GameObject IArrangementCell.GameObj => gameObject;

        void IArrangementCell.SetAcitve(bool isActive)
        {
            
        }

        public void SetIsTutorial(bool isTutorial)
        {
            if (_data == null)
                return;

            _data.isTutorial = isTutorial;
            guideLineImg?.SetActive(isTutorial);
        }
        #endregion

        protected abstract void OnClickObtain();

        public void OnClick()
        {
            if (_data == null)
                return;

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            _data.IListener?.Edit(_data.Id, _data.index);
        }
    }
}

