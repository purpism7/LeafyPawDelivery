using Game;
using Game.Event;
using GameSystem;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI.Component

{
    public class ObjectArrangementCell : ArrangementCell<ObjectArrangementCell.Data>
    {
        public new class Data : ArrangementCell<ObjectArrangementCell.Data>.Data
        {
            public int Count { get; private set; } = 0;
            public Data WithCount(int count)
            {
                Count = count;
                return this;
            }

        }

        [Header("")]
        [SerializeField] private Button buyBtn = null;
        [SerializeField] private OpenCondition animalCurrencyCost = null;
        [SerializeField] private OpenCondition objectCurrencyCost = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            if (data.Id == 142)
            {
                animalCurrencyCost?.Activate();
                objectCurrencyCost?.Activate();
                // GameUtils.SetActive(openRootRectTm, false);

                buyBtn?.SetActive(true);

                //GameUtils.SetActive(openRootRectTm, false);

                buyBtn?.onClick?.RemoveAllListeners();
                //buyBtn?.onClick?.AddListener(OnClickBuy);
            }
            else
            {
                animalCurrencyCost?.Deactivate();
                objectCurrencyCost?.Deactivate();

                buyBtn?.SetActive(false);

                //GameUtils.SetActive(openRootRectTm, true);
            }
                
        }
        public override void Activate()
        {
            base.Activate();

            SetHiddenOpenDescTMP();
            SetSpecialObjectDescTMP();

            if (_data.Id == 142)
            {
                descTMP?.SetText($"{_data.Count}");

                SetPurchaseCost();
            }
        }

        protected override void SetNameTMP(string name)
        {
            base.SetNameTMP(name);

            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition != null)
            {
                if (openCondition.eType == OpenConditionData.EType.Hidden ||
                   openCondition.eType == OpenConditionData.EType.Special)
                    name = string.Empty;
            }

            openNameTMP?.SetText(name);
        }

        protected override void SetDescTMP()
        {
            if (_data == null)
                return;

            var text = string.Empty;
            var objectData = ObjectContainer.Instance.GetData(_data.Id);
            if (objectData != null)
            {
                text = $"x{objectData.Count}";
            }
            
            descTMP?.SetText(text);
        }

        private void SetHiddenOpenDescTMP()
        {
            if (!CheckHiddenObject)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id);
            var desc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_hidden_object", LocalizationSettings.SelectedLocale);

            openDescTMP?.SetText(string.Format(desc, localName));
        }

        private void SetSpecialObjectDescTMP()
        {
            var animalData = AnimalContainer.Instance?.GetDataByInteractionId(_data.Id);
            if (animalData == null)
                return;

            openNameTMP?.SetText(string.Empty);

            var localName = GameUtils.GetName(Type.EElement.Animal, animalData.Id, Games.Data.Const.AnimalBaseSkinId);
            var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_try_to_get_closer", LocalizationSettings.SelectedLocale);

            openDescTMP?.SetText(string.Format(localDesc, localName));
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

        protected override void SetOpenConditionData()
        {
            base.SetOpenConditionData();

            SetObjectOpenCondition();
        }

        private void SetObjectOpenCondition()
        {
            if (_data == null)
                return;

            if (_data.Owned)
                return;

            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            GameUtils.SetActive(openConditionRootRectTm, openCondition.eType != OpenConditionData.EType.Hidden || openCondition.eType != OpenConditionData.EType.Special);
            //GameUtils.SetActive(openConditionRootRectTm, _data.Id == 142);

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

        private void SetPurchaseCost()
        {
            if (_data == null)
                return;

            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            var animalCurrency = Mathf.CeilToInt(openCondition.AnimalCurrency * _data.Count * 1.25f);
            var openConditionData = CreateOpenConditionData(placeData.AnimalSpriteName, animalCurrency, () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id));

            animalCurrencyCost?.Initialize(openConditionData);
            animalCurrencyCost?.Activate();

            var objectCurrency = Mathf.CeilToInt(openCondition.ObjectCurrency * _data.Count * 1.5f);
            openConditionData = CreateOpenConditionData(placeData.ObjectSpriteName, objectCurrency, () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id));

            objectCurrencyCost?.Initialize(openConditionData);
            objectCurrencyCost?.Activate();
        }

        #region Lock
        protected override void Unlock()
        {
            if (!IsLock)
                return;

            if (_data != null)
            {
                _data.Lock = !ObjectOpenConditionContainer.Instance.CheckReq(_data.Id);
                SetNotificationPossibleBuyObject();
            }

            ProcessUnlock();
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
        #endregion

        private OpenConditionData ObjectOpenConditionData
        {
            get
            {
                var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
                var openCondition = objectOpenConditionContainer?.GetData(_data.Id);

                return openCondition;
            }
        }

        protected override void OnClickObtain()
        {
            if (_data == null)
                return;

            if (_data.isTutorial)
                return;

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            var openCondition = ObjectOpenConditionData;
            if (openCondition == null)
                return;

            if (openCondition.eType == OpenConditionData.EType.Hidden)
                return;

            if (openCondition.eType == OpenConditionData.EType.Special)
            {
                var animalData = AnimalContainer.Instance?.GetDataByInteractionId(_data.Id);
                if (animalData == null)
                    return;

                var popup = new PopupCreator<Profile, Profile.Data>()
                    .SetReInitialize(true)
                    .SetData(
                        new Profile.Data()
                        {
                            EElement = Type.EElement.Animal,
                            Id = animalData.Id,
                            ETab = Type.ETab.Friendship,
                            RefreshAction = () =>
                            {

                            },

                        })
                    .Create();

                return;
            }

            CreateObtainPopup();
        }
    }
}
