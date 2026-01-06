using Game;
using Game.Event;
using GameSystem;
using Info;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static Game.Type;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

namespace UI.Component

{
    public class ObjectArrangementCell : ArrangementCell<ObjectArrangementCell.Data>
    {
        public new class Data : ArrangementCell<ObjectArrangementCell.Data>.Data
        {
            public ObjectType ObjectType { get; private set; } = ObjectType.None;
            public int Count { get; private set; } = 0;

            public Data WithObjectType(ObjectType type)
            {
                ObjectType = type;
                return this;
            }

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

            if (data.ObjectType == ObjectType.Garden)
            {
                animalCurrencyCost?.Activate();
                objectCurrencyCost?.Activate();

                buyBtn?.SetActive(true);

                buyBtn?.onClick?.RemoveAllListeners();
                buyBtn?.onClick?.AddListener(OnClickBuy);
            }
            else
            {
                animalCurrencyCost?.Deactivate();
                objectCurrencyCost?.Deactivate();

                buyBtn?.SetActive(false);
            }
                
        }
        public override void Activate()
        {
            base.Activate();

            SetHiddenOpenDescTMP();
            SetSpecialObjectDescTMP();

            if (_data.ObjectType == ObjectType.Garden)
            {
                SetPurchaseCost();
            }
        }

        protected override void SetNameTMP(string name)
        {
            base.SetNameTMP(name);

            var openCondition = ObjectOpenConditionData;
            if (openCondition != null)
            {
                if (openCondition.eType == OpenConditionData.EType.Hidden ||
                   openCondition.eType == OpenConditionData.EType.Special)
                    name = string.Empty;
            }

            openNameTMP?.SetText(name);
        }

        protected override void SetCount()
        {
            if (_data == null)
                return;
            
            var objectMgr = MainGameManager.Get<Game.ObjectManager>();
            if (objectMgr == null)
                return;

            int count = _data.Count;
            int remainCount = objectMgr.GetRemainCount(_data.Id);

            var objectInfo = objectMgr.GetObjectInfoById(_data.Id);
            if (_data.ObjectType == ObjectType.Garden)
            {
                count = UserManager.Instance.GardenPlotCount;
            }

            descTMP?.SetText($"{remainCount}/{count}");
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

            var openCondition = ObjectOpenConditionData;
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            GameUtils.SetActive(openConditionRootRectTm, openCondition.eType != OpenConditionData.EType.Hidden || openCondition.eType != OpenConditionData.EType.Special);

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
            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;

            AddOpenCondition(placeData.AnimalSpriteName, openCondition.AnimalCurrency, () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id));
            AddOpenCondition(placeData.ObjectSpriteName, openCondition.ObjectCurrency, () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id));
        }

        private void SetPurchaseCost()
        {
            if (_data == null)
                return;

            var openCondition = ObjectOpenConditionData;
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;

            var animalCurrency = RequiredAnimalCurrency;
            var openConditionData = CreateOpenConditionData(placeData.AnimalSpriteName, animalCurrency, () => objectOpenConditionContainer.CheckAnimalCurrency(_data.Id));

            animalCurrencyCost?.Initialize(openConditionData);
            animalCurrencyCost?.Activate();

            var objectCurrency = RequiredObjectCurrency;
            openConditionData = CreateOpenConditionData(placeData.ObjectSpriteName, objectCurrency, () => objectOpenConditionContainer.CheckObjectCurrency(_data.Id));

            objectCurrencyCost?.Initialize(openConditionData);
            objectCurrencyCost?.Activate();
        }

        private int RequiredAnimalCurrency
        {
            get
            {
                if (_data == null)
                    return 0;

                var openCondition = ObjectOpenConditionData;
                if (openCondition == null)
                    return 0;

                return Mathf.CeilToInt(openCondition.AnimalCurrency * _data.Count * 1.25f);
            }
        }

        private int RequiredObjectCurrency
        {
            get
            {
                if (_data == null)
                    return 0;

                var openCondition = ObjectOpenConditionData;
                if (openCondition == null)
                    return 0;

                return Mathf.CeilToInt(openCondition.ObjectCurrency * _data.Count * 1.5f);
            }
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

        protected override bool CreateObtainPopup(int animalCurrency, int objectCurrency)
        {
            bool isPossibleObtain = ObjectOpenConditionContainer.Instance.Check(_data.Id);
            if (!isPossibleObtain)
                return false;

            if(base.CreateObtainPopup(animalCurrency, objectCurrency))
            {
                MainGameManager.Get<Game.ObjectManager>()?.Add(_data.Id);
                SetCount();

                return true;
            }

            return false;
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

            var openConditionData = ObjectOpenConditionData;
            if(openConditionData != null)
                CreateObtainPopup(openConditionData.AnimalCurrency, openConditionData.ObjectCurrency);
        }

        private void OnClickBuy()
        {
            var mainGameMgr = MainGameManager.Instance;
            var userManager = UserManager.Instance;

            var currency = new Info.User.Currency
            {
                PlaceId = GameUtils.ActivityPlaceId,
                Animal = -RequiredAnimalCurrency,
                Object = -RequiredObjectCurrency,
            };

            if (!userManager.CheckCurrency(currency))
            {
                var localDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_currency", LocalizationSettings.SelectedLocale);
                Game.Toast.Get?.Show(localDesc);

                return;
            }

            var openConditionData = ObjectOpenConditionData;
            if (openConditionData != null)
                CreateObtainPopup(openConditionData.AnimalCurrency, openConditionData.ObjectCurrency);
        }
    }
}
