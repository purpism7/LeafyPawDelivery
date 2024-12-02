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
using Type = Game.Type;

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
            public bool IsSpecialObject = false;
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

        [SerializeField]
        private Image hiddenIconImg = null;
        [SerializeField]
        private Image storyIconImg = null;

        [SerializeField]
        private Image guideLineImg = null;
        
        [SerializeField]
        private RectTransform specialObjectRectTm = null;

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

            GameUtils.SetActive(openRootRectTm, !_data.Owned);
            GameUtils.SetActive(lockRootRectTm, IsLock);
            GameUtils.SetActive(lockImg, IsLock);
            GameUtils.SetActive(lockBgImg, IsLock);
            if(IsLock)
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

            SetNameTMP();
            ActivateOpenConditionList();
            SetHiddenOpenDescTMP();
            
            if(_data != null)
                GameUtils.SetActive(specialObjectRectTm, _data.IsSpecialObject);
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

            var localName = GameUtils.GetName(_data.EElement, _data.Id, Games.Data.Const.AnimalBaseSkinId);

            nameTMP?.SetText(localName);

            if (_data.EElement == Game.Type.EElement.Object)
            {
                var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
                var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
                if (openCondition != null)
                {
                    if(openCondition.eType == OpenConditionData.EType.Hidden ||
                       openCondition.eType == OpenConditionData.EType.Special)
                        localName = string.Empty;
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
                text = "x1";
            }
            else if(_data.EElement == Game.Type.EElement.Object)
            {
                var objectData = ObjectContainer.Instance.GetData(_data.Id);
                if(objectData != null)
                {
                    text = $"x{objectData.Count}";
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

            GameUtils.SetActive(hiddenIconImg, false);

            openDescTMP?.SetText(string.Empty);

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

        private void SetHiddenOpenDescTMP()
        {
            if (_data.EElement != Game.Type.EElement.Object)
                return;

            if (!CheckHiddenObject)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id);
            var desc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_hidden_object", LocalizationSettings.SelectedLocale);

            openDescTMP?.SetText(string.Format(desc, localName));
        }
        
        private void SetSpecialObjectDescTMP()
        {
            if (_data.EElement != Game.Type.EElement.Object)
                return;
         
            openNameTMP?.SetText(string.Empty);
            
            var animalData = AnimalContainer.Instance?.GetDataByInteractionId(_data.Id);
            if (animalData == null)
                return;
                    
            var localName = GameUtils.GetName(Type.EElement.Animal, animalData.Id, Games.Data.Const.AnimalBaseSkinId);
            var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_try_to_get_closer", LocalizationSettings.SelectedLocale);

            openDescTMP?.SetText(string.Format(localDesc, localName));
        }

        private void SetAnimalOpenCondition()
        {
            var animalOpenConditionContainer = AnimalOpenConditionContainer.Instance;
            var openCondition = animalOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
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

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            GameUtils.SetActive(openConditionRootRectTm, openCondition.eType != OpenConditionData.EType.Hidden);
            
            switch (openCondition.eType)
            {
                case OpenConditionData.EType.Hidden:
                {
                    GameUtils.SetActive(hiddenIconImg, true);

                    openNameTMP?.SetText(string.Empty);

                    SetHiddenOpenDescTMP();

                    return;
                }

                case OpenConditionData.EType.Special:
                {
                    SetSpecialObjectDescTMP();
                    
                    return;
                }
            }

            AddOpenCondition(placeData.AnimalSpriteName, openCondition.AnimalCurrency, () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id));
            AddOpenCondition(placeData.ObjectSpriteName, openCondition.ObjectCurrency, () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id));
        }

        private bool CheckHiddenObject
        {
            get
            {
                if (_data == null)
                    return false;

                var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
                var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
                if (openCondition == null)
                    return false;

                return openCondition.eType == OpenConditionData.EType.Hidden;
            }
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

        private void SetButtonState()
        {
            arrangementBtn?.SetActive(!IsLock);
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

                    Info.UserManager.Instance?.SetCurrency(
                        new Info.User.Currency()
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
            guideLineImg?.SetActive(isTutorial);
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

        private void Unlock()
        {
            if (!IsLock)
                return;

            if (_data.EElement == Game.Type.EElement.Object)
            {
                _data.Lock = !ObjectOpenConditionContainer.Instance.CheckReq(_data.Id);

                SetNotificationPossibleBuyObject();
            }
            else if(_data.EElement == Game.Type.EElement.Animal)
            {
                _data.Lock = !AnimalOpenConditionContainer.Instance.CheckReq(_data.Id);

                SetNotificationPossibleBuyAnimal();
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

        private bool IsLock
        {
            get
            {
                if (_data == null)
                    return true;

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

        private void SetNotificationPossibleBuyAnimal()
        {
            var connector = Info.Connector.Get;
            if (!_data.Lock &&
                connector != null &&
                connector.PossibleBuyAnimal <= 0)
            {
                connector.SetPossibleBuyAnimal(_data.Id);
            }
        }

        private void SetNotificationPossibleBuyObject()
        {
            var connector = Info.Connector.Get;
            if (!_data.Lock &&
                connector != null &&
                connector.PossibleBuyObject <= 0)
            {
                connector.SetPossibleBuyObject(_data.Id);
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

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

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

