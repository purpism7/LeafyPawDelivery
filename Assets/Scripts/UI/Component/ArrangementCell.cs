using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;
using DG.Tweening;
using UnityEngine.Localization.Components;

using GameSystem;

namespace UI.Component
{
    public class ArrangementCell : UI.BaseComponent<ArrangementCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public int Id = 0;
            public Game.Type.EElement EElement = Game.Type.EElement.None; 
            public string Name = string.Empty;
            public bool Owned = false;
            public bool Lock = true;
            public bool isTutorial = false;

            public int index = -1;
        }
        
        public interface IListener
        {
            void Edit(Game.Type.EElement EElement, int id, int index);
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] private TextMeshProUGUI descTMP = null;
        [SerializeField] private Button arrangementBtn = null;
        [SerializeField] private Image iconImg = null;

        [SerializeField]
        private RectTransform openRootRectTm = null;
        [SerializeField]
        private RectTransform openConditionRootRectTm = null;
        [SerializeField]
        private TextMeshProUGUI openNameTMP = null;
        [SerializeField]
        private TextMeshProUGUI openDescTMP = null;
        //[SerializeField]
        //private TextMeshProUGUI openConditionDescTMP = null;

        [SerializeField]
        private Image hiddenIconImg = null;
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

        public int Id { get { return _data != null ? _data.Id : 0; } }
        public Game.Type.EElement EElement { get { return _data != null ? _data.EElement : Game.Type.EElement.None; } }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            UIUtils.SetActive(openRootRectTm, !_data.Owned);
            UIUtils.SetActive(lockRootRectTm, IsLock);
            UIUtils.SetActive(unLockImg?.gameObject, false);

            SetDescTMP();
            SetElementIconImg();
            SetButtonState();
            SetOpenConditionData();
            SetStoryIcon();

            UIUtils.SetActive(guideLineImg?.gameObject, data.isTutorial);
            if(data.isTutorial)
            {
                guideLineImg?.AnimBlink();
            }
        }

        public override void Activate()
        {
            base.Activate();

            SetNameTMP();

            ActivateOpenConditionList();

            openDescTMP?.GetComponent<LocalizeStringEvent>()?.RefreshString();
        }

        public override void Deactivate()
        {
            DeactivateOpenConditionList();

            base.Deactivate();
        }

        private void DeactiveAllOpenConditionList()
        {
            foreach(var openCondition in _openConditionList)
            {
                if (openCondition == null)
                    continue;

                openCondition.gameObject.SetActive(false);
            }
        }

        private void ActivateOpenConditionList()
        {
            if (_openConditionList == null)
                return;

            foreach(var openCondition in _openConditionList)
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

        private void SetNameTMP()
        {
            if (_data == null)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id, Game.Data.Const.AnimalBaseSkinId);

            nameTMP?.SetText(localName);

            if (_data.EElement == Game.Type.EElement.Object)
            {
                var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
                var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
                if (openCondition != null)
                {
                    if(openCondition.eType == OpenConditionData.EType.Hidden)
                    {
                        localName = string.Empty;
                    }
                }
            }

            openNameTMP?.SetText(localName);
        }

        private void SetDescTMP()
        {
            if (_data == null)
                return;

            var text = string.Empty;
            if(_data.EElement == Game.Type.EElement.Animal)
            {
                //text = LocalizationSettings.StringDatabase.GetLocalizedString(_data.EElement.ToString(), "description_" + _data.Id, LocalizationSettings.SelectedLocale);
                text = String.Format("x{0}", 1);
            }
            else if(_data.EElement == Game.Type.EElement.Object)
            {
                var objectData = ObjectContainer.Instance.GetData(_data.Id);
                if(objectData != null)
                {
                    text = String.Format("x{0}", objectData.Count);
                }
            }

            descTMP?.SetText(text);
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

        private void SetOpenConditionData()
        {
            DeactiveAllOpenConditionList();

            UIUtils.SetActive(hiddenIconImg?.gameObject, false);

            openDescTMP?.SetText(string.Empty);
            //openConditionDescTMP?.SetText(string.Empty);

            if (_data == null)
                return;

            if (_data.Owned)
                return;

            if (_data.EElement == Game.Type.EElement.Animal)
            {
                SetAnimalOpenCondition();
            }
            else
            {
                SetObjectOpenCondition();
            }
        }

        private void SetAnimalOpenCondition()
        {
            var animalOpenConditionContainer = AnimalOpenConditionContainer.Instance;
            var openCondition = animalOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var placeData = Game.Data.Const.ActivityPlaceData;
            if (placeData == null)
                return;

            AddOpenCondition(placeData.AnimalSpriteName, openCondition.AnimalCurrency, () => animalOpenConditionContainer.CheckAnimalCurrency(_data.Id));
            AddOpenCondition(placeData.ObjectSpriteName, openCondition.ObjectCurrency, () => animalOpenConditionContainer.CheckObjectCurrency(_data.Id));
        }

        private void SetObjectOpenCondition()
        {
            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var placeData = Game.Data.Const.ActivityPlaceData;
            if (placeData == null)
                return;

            if(openCondition.eType == OpenConditionData.EType.Hidden)
            {
                UIUtils.SetActive(hiddenIconImg?.gameObject, true);

                openNameTMP?.SetText(string.Empty);

                var localName = GameUtils.GetName(_data.EElement, _data.Id);
                var desc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_hidden_object", LocalizationSettings.SelectedLocale);
                
                openDescTMP?.SetText(string.Format(desc, localName));

                return;
            }

            AddOpenCondition(placeData.AnimalSpriteName, openCondition.AnimalCurrency, () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id));
            AddOpenCondition(placeData.ObjectSpriteName, openCondition.ObjectCurrency, () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id));
        }

        private string GetRequireCurrency(long currency)
        {
            return string.Format("{0}", currency);
        }

        private void AddOpenCondition(string spriteName, int currency, Func<bool> possibleFunc)
        {
            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(spriteName),
                Text = GetRequireCurrency(currency),
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

            if(openCondition != null)
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

        private void SetStoryIcon()
        {
            UIUtils.SetActive(storyIconImg?.gameObject, false);

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
                    UIUtils.SetActive(storyIconImg?.gameObject, true);

                    break;
                }
            }
        }

        private void SetButtonState()
        {
            // UIUtils.SetActive(buyBtn?.gameObject, _data.Lock);
            UIUtils.SetActive(arrangementBtn?.gameObject, !IsLock);
        }

        private void CreateObtainPopup()
        {
            if (_data == null)
                return;

            bool isPossibleObtain = false;
            OpenConditionData openConditionData = null;

            switch(_data.EElement)
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

                    int animalCurrency = openConditionData.AnimalCurrency;
                    int objectCurrency = openConditionData.ObjectCurrency;

                    Info.UserManager.Instance?.SetCurrency(new Info.User.Currency()
                    {
                        PlaceId = GameUtils.ActivityPlaceId,
                        Animal = -animalCurrency,
                        Object = -objectCurrency,
                    });

                    ITop iTop = Game.UIManager.Instance?.Top;
                    iTop?.SetCurrency();

                    mainGameMgr?.AddAcquireCurrency(Game.Type.EElement.Animal, Game.Type.EAcquireAction.Use, animalCurrency);
                    mainGameMgr?.AddAcquireCurrency(Game.Type.EElement.Object, Game.Type.EAcquireAction.Use, objectCurrency);

                    return obtain;
                });

            mainGameMgr?.Add(_data.EElement, _data.Id);
        }

        public void SetIndex(int index)
        {
            if (_data == null)
                return;

            _data.index = index;
        }

        public void SetIsTutorial(bool isTutorial)
        {
            if (_data == null)
                return;

            _data.isTutorial = isTutorial;

            UIUtils.SetActive(guideLineImg?.gameObject, isTutorial);
        }

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

            UIUtils.SetActive(openRootRectTm, !_data.Owned);

            return true;
        }

        private void Unlock()
        {
            if (!IsLock)
                return;

            if (_data.EElement == Game.Type.EElement.Object)
            {
                _data.Lock = !ObjectOpenConditionContainer.Instance.CheckReq(_data.Id);
            }
            else if(_data.EElement == Game.Type.EElement.Animal)
            {
                _data.Lock = !AnimalOpenConditionContainer.Instance.CheckReq(_data.Id);
            }
            
            if (IsLock)
                return;

            _openConditionList?.ForEach(
                openCondition =>
                {
                    openCondition?.Activate();
                });

            AnimUnlock();
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
                       .AppendCallback(() => UIUtils.SetActive(lockImg?.gameObject, false))
                       .AppendCallback(() => UIUtils.SetActive(unLockImg?.gameObject, true))
                       .AppendInterval(0.4f)
                       .Append(lockBgImg.DOFade(0, 0.3f))
                       .Join(unLockImg.DOFade(0, 0.3f))
                       .OnComplete(() =>
                       {
                           UIUtils.SetActive(lockRootRectTm, IsLock);

                           _endTask = true;
                       });
                     sequence.Restart();

                    return this;
                });
        }

        private bool IsLock
        {
            get
            {
                if (_data == null)
                    return true;

                //if(_data.EElement == Game.Type.EElement.Object)
                //{
                //    var openConditionData = ObjectOpenConditionData;
                //    if (openConditionData == null)
                //        return true;

                //    if (openConditionData.eType == OpenConditionData.EType.Hidden)
                //        return false;
                //}

                return _data.Lock;
            }
        }

        private OpenConditionData ObjectOpenConditionData
        {
            get
            {
                var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
                var openCondition = objectOpenConditionContainer?.GetData(_data.Id);

                return openCondition;
            }
        }
        
        public void OnClickObtain()
        {
            if (_data == null)
                return;

            if (_data.isTutorial)
                return;

            if(_data.EElement == Game.Type.EElement.Object)
            {
                var openCondition = ObjectOpenConditionData;
                if (openCondition == null)
                    return;

                if (openCondition.eType == OpenConditionData.EType.Hidden)
                    return;
            }

            CreateObtainPopup();
        }

        // 배치 버튼 클릭 시,
        public void OnClick()
        {
            if (_data == null)
                return;

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            _data.IListener?.Edit(_data.EElement, _data.Id, _data.index);
        }
    }
}

