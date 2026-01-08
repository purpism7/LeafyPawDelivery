using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;

namespace UI.Component

{
    public class AnimalArrangementCell : ArrangementCell<AnimalArrangementCell.Data>
    {
        public new class Data : ArrangementCell<AnimalArrangementCell.Data>.Data
        {
            
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

        }

        public override void Activate()
        {
            base.Activate();
            
            SetCount();
        }

        protected override void SetNameTMP(string name)
        {
            base.SetNameTMP(name);

            openNameTMP?.SetText(name);
        }

        protected override void SetCount()
        {
            if (_data == null)
                return;

            descTMP?.SetText("x1");
        }

        #region Open Condition
        protected override void SetOpenConditionData()
        {
            base.SetOpenConditionData();

            SetAnimalOpenCondition();
        }

        private OpenConditionData AnimalOpenConditionData
        {
            get
            {
                var openConditionContainer = AnimalOpenConditionContainer.Instance;
                var openCondition = openConditionContainer?.GetData(_data.Id);

                return openCondition;
            }
        }

        private void SetAnimalOpenCondition()
        {
            if (_data == null)
                return;

            if (_data.Owned)
                return;

            var openCondition = AnimalOpenConditionData;
            if (openCondition == null)
                return;

            var placeData = MainGameManager.Get<PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            var animalOpenConditionContainer = AnimalOpenConditionContainer.Instance;

            AddOpenCondition(placeData.AnimalSpriteName, openCondition.AnimalCurrency, () => animalOpenConditionContainer.CheckAnimalCurrency(_data.Id));
            AddOpenCondition(placeData.ObjectSpriteName, openCondition.ObjectCurrency, () => animalOpenConditionContainer.CheckObjectCurrency(_data.Id));

            ActivateOpenConditionList();
        }
        #endregion

        #region Lock
        protected override void Unlock()
        {
            if (!IsLock)
                return;

            if (_data != null)
            {
                _data.Lock = !AnimalOpenConditionContainer.Instance.CheckReq(_data.Id);
                SetNotificationPossibleBuyAnimal();
            }

            ProcessUnlock();
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
        #endregion

        protected override bool CreateObtainPopup(int animalCurrency, int objectCurrency)
        {
            bool isPossibleObtain = isPossibleObtain = AnimalOpenConditionContainer.Instance.Check(_data.Id);
            if (!isPossibleObtain)
                return false;

            if(base.CreateObtainPopup(animalCurrency, objectCurrency))
            {
                MainGameManager.Get<Game.AnimalManager>()?.Add(_data.Id);
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

            var openCondition = AnimalOpenConditionData;
            if (openCondition != null)
                CreateObtainPopup(openCondition.AnimalCurrency, openCondition.ObjectCurrency);           
        }
    }
}
