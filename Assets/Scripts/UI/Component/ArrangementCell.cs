using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

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
            public bool Lock = true;
        }
        
        public interface IListener
        {
            void Edit(Game.Type.EElement EElement, int id);
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] private TextMeshProUGUI descTMP = null;
        [SerializeField] private Button arrangementBtn = null;
        [SerializeField] private Image iconImg = null;

        [Header("Lock")]
        [SerializeField] private TextMeshProUGUI lockNameTMP = null;
        [SerializeField] private RectTransform lockRootRectTm = null;
        [SerializeField] private RectTransform openCondtionRootRectTm = null;
        #endregion

        private List<OpenCondition> _openConditionList = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _openConditionList?.Clear();

            SetNameTMP();
            SetDescTMP();
            SetIconImg();
            SetButtonState();
            SetLockData();

            UIUtils.SetActive(lockRootRectTm, _data.Lock);
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
            lockNameTMP?.SetText(localName);
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

        private void SetLockData()
        {
            if (_data == null)
                return;

            if (!_data.Lock)
                return;

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

            var currencyInfo = Game.Data.Const.GetCurrencyInfo(ActivityPlaceId);
            if (currencyInfo == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyInfo.AnimalSpriteName),
                Text = GetReqAnimalCurrency(openCondition.AnimalCurrency),
                PossibleFunc = () => animalOpenConditionContainer.CheckAnimalCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);

            openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyInfo.ObjectSpriteName),
                Text = GetReqObjectCurrency(openCondition.ObjectCurrency),
                PossibleFunc = () => animalOpenConditionContainer.CheckObjectCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);
        }

        private void SetObjectOpenCondition()
        {
            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var currencyInfo = Game.Data.Const.GetCurrencyInfo(ActivityPlaceId);
            if (currencyInfo == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyInfo.AnimalSpriteName),
                Text = GetReqAnimalCurrency(openCondition.AnimalCurrency),
                PossibleFunc = () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);

            openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyInfo.ObjectSpriteName),
                Text = GetReqObjectCurrency(openCondition.ObjectCurrency),
                PossibleFunc = () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);
        }

        private string GetReqAnimalCurrency(long currency)
        {
            return string.Format("x{0}", currency);
        }

        private string GetReqObjectCurrency(long currency)
        {
            return string.Format("x{0}", currency);
        }

        private UI.Component.OpenCondition CreateOpenCondition(OpenCondition.Data openConditionData)
        {
            var openCondition = new ComponentCreator<OpenCondition, OpenCondition.Data>()
                    .SetData(openConditionData)
                    .SetRootRectTm(openCondtionRootRectTm)
                    .Create();

            if(openCondition != null)
            {
                _openConditionList?.Add(openCondition);
            }

            return openCondition;
        }

        private void SetButtonState()
        {
            // UIUtils.SetActive(buyBtn?.gameObject, _data.Lock);
            UIUtils.SetActive(arrangementBtn?.gameObject, !_data.Lock);
        }

        private int ActivityPlaceId
        {
            get
            {
                var placeMgr = MainGameManager.Instance?.placeMgr;
                if (placeMgr == null)
                    return 0;

                return placeMgr.ActivityPlaceId;
            }
        }

        private void CreateUnlockPopup()
        {
            if (_data == null)
                return;

            bool isPossibleUnlock = false;
            OpenConditionData openConditionData = null;

            switch(_data.EElement)
            {
                case Game.Type.EElement.Animal:
                    {
                        isPossibleUnlock = AnimalOpenConditionContainer.Instance.Check(_data.Id);
                        openConditionData = AnimalOpenConditionContainer.Instance.GetData(_data.Id);

                        break;
                    }

                case Game.Type.EElement.Object:
                    {
                        isPossibleUnlock = ObjectOpenConditionContainer.Instance.Check(_data.Id);
                        openConditionData = ObjectOpenConditionContainer.Instance.GetData(_data.Id);

                        break;
                    }
            }

            if (openConditionData == null)
                return;

            if (!isPossibleUnlock)
                return;

            Sequencer.EnqueueTask(
                () =>
                {
                    var popup = new PopupCreator<Unlock, Unlock.Data>()
                        .SetData(new Unlock.Data()
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


                    Info.UserManager.Instance.SaveCurrency(new Info.User.Currency()
                    {
                        PlaceId = ActivityPlaceId,
                        Animal = -openConditionData.AnimalCurrency,
                        Object = -openConditionData.ObjectCurrency,
                    });

                    Game.UIManager.Instance?.Top?.SetCurrency();

                    return popup;
                });

            MainGameManager.Instance?.AddInfo(_data.EElement, _data.Id);
        }

        public void Unlock(Game.Type.EElement EElement, int id)
        {
            if (_data == null)
                return;

            if (_data.EElement != EElement)
                return;

            if (_data.Id != id)
                return;
            
            _data.Lock = false;
            
            SetIconImg();
            SetButtonState();
            
            UIUtils.SetActive(lockRootRectTm, _data.Lock);
        }
        
        public void OnClickUnlock()
        {
            CreateUnlockPopup();
        }

        // 배치 버튼 클릭 시,
        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.EElement, _data.Id);
        }
    }
}

