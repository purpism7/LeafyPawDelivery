using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;
using DG.Tweening;

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

            public int index = 0;
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

            SetNameTMP();
            SetDescTMP();
            SetIconImg();
            SetButtonState();
            SetOpenConditionData();

            UIUtils.SetActive(openRootRectTm, !_data.Owned);
            UIUtils.SetActive(lockRootRectTm, _data.Lock);
            UIUtils.SetActive(unLockImg?.gameObject, false);
        }

        public override void Activate()
        {
            base.Activate();

            ActivateOpenConditionList();
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

            var localName = GameUtils.GetName(_data.EElement, _data.Id);

            nameTMP?.SetText(localName);
            openNameTMP?.SetText(localName);
        }

        private void SetDescTMP()
        {
            if (_data == null)
                return;

            var text = string.Empty;
            if(_data.EElement == Game.Type.EElement.Animal)
            {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(_data.EElement.ToString(), "description_" + _data.Id, LocalizationSettings.SelectedLocale);
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

        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(_data.EElement, _data.Id);

            if (_data.Lock)
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

            if (_data == null)
                return;

            //if (!_data.Lock)
            //    return;

            if(_data.EElement == Game.Type.EElement.Animal)
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

        private void SetButtonState()
        {
            // UIUtils.SetActive(buyBtn?.gameObject, _data.Lock);
            UIUtils.SetActive(arrangementBtn?.gameObject, !_data.Lock);
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

                    Info.UserManager.Instance?.User?.SetCurrency(new Info.User.Currency()
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

        public void Obtain(Game.Type.EElement eElement, int id)
        {
            if (_data == null)
                return;

            Unlock(eElement, id);

            if (_data.EElement != eElement)
                return;

            if (_data.Id != id)
                return;

            _data.Owned = true;

            SetIconImg();
            SetButtonState();

            UIUtils.SetActive(openRootRectTm, !_data.Owned);
        }

        private void Unlock(Game.Type.EElement eElement, int id)
        {
            if (_data == null)
                return;

            if (!_data.Lock)
                return;

            if (eElement != Game.Type.EElement.Object)
                return;

            _data.Lock = !ObjectOpenConditionContainer.Instance.CheckReq(_data.Id);
            if (_data.Lock)
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
                           UIUtils.SetActive(lockRootRectTm, _data.Lock);

                           _endTask = true;
                       });
                     sequence.Restart();

                    return this;
                });
        }
        
        public void OnClickObtain()
        {
            CreateObtainPopup();
        }

        // 배치 버튼 클릭 시,
        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.EElement, _data.Id, _data.index);
        }
    }
}

