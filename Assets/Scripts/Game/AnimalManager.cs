using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class AnimalManager : Manager.Base<AnimalManager.Data>
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }
        
        private Data _data = null;
        private Info.AnimalHolder _animalHolder = new();

        public List<Info.Animal> AnimalInfoList => _animalHolder?.AnimalInfoList;
        public UnityEvent<Info.Animal> Listener { get; private set; } = new();

        protected override void Initialize()
        {
            Debug.Log("AnimalManager Initialize");

            Listener = new UnityEvent<Info.Animal>();
            Listener.RemoveAllListeners();

            var mainGameMgr = MainGameManager.Instance;
            mainGameMgr?.ObjectMgr.Listener.AddListener(OnChangedObjectInfo);
            mainGameMgr?.placeMgr?.Listener?.AddListener(OnChangedPlace);
        }

        public override IEnumerator CoInit(Data data)
        {
            _data = data;
            
            _animalHolder?.LoadInfo();

            Debug.Log("AnimalManager CoInit");
            
            yield break;
        }

        public void AddAnimal(Info.Animal animalInfo)
        {
            if (_animalHolder == null)
                return;
            
            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                Listener?.Invoke(animalInfo);
            }
        }

        // Add 되는 Animal 의 Skin Id Base 인 1 일 것.
        public void AddAnimal(int animalId)
        {
            if (_animalHolder == null)
                return;
            
            var animalInfo = new Info.Animal()
            {
                Id = animalId,
                SkinId = 1,
            };

            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                Listener?.Invoke(animalInfo);
            }
        }

        public void RemoveAnimal(int id)
        {
            _animalHolder?.RemoveAnimal(id);
        }

        public void ArrangeAnimal(int id, Vector3 pos, int placeId)
        {
            _animalHolder?.ArrangeAnimal(id, pos, placeId);
        }

        public bool CheckExist(int animalId)
        {
            if (AnimalInfoList == null)
                return false;
            
            return GetAnimalInfo(animalId) != null;
        }

        public Info.Animal GetAnimalInfo(int animalId)
        {
            return _animalHolder?.GetAnimalInfo(animalId);
        }

        public void Check()
        {
            if (_data == null)
                return;

            var animalOpenConidtionDatas = AnimalOpenConditionContainer.Instance?.Datas;
            if (animalOpenConidtionDatas == null)
                return;

            var animalContainer = AnimalContainer.Instance;

            foreach(var data in animalOpenConidtionDatas)
            {
                if (data == null)
                    continue;

                var animalData = animalContainer?.GetData(data.Id);
                if (animalData != null &&
                    animalData.PlaceId != _data.PlaceId)
                    continue;

                if (MainGameManager.Instance.CheckExist(Game.Type.EElement.Animal, data.Id))
                    continue;

                if (data.EType_ == OpenConditionData.EType.Starter)
                {
                    Sequencer.EnqueueTask(
                        () =>
                        {
                            var popup = new PopupCreator<Unlock, Unlock.Data>()
                                .SetData(new Unlock.Data()
                                {
                                    EElement = Type.EElement.Animal,
                                    Id = data.Id,
                                    ClickAction = () =>
                                    {

                                    },
                                })
                                .SetCoInit(true)
                                .SetReInitialize(true)
                                .Create();

                            return popup;
                        });

                    MainGameManager.Instance?.AddInfo(Type.EElement.Animal, data.Id);
                }
            }
        }

        #region Skin
        public bool CheckExistSkin(int id, int skinId)
        {
            if (_animalHolder == null)
                return false;

            return _animalHolder.CheckExist(id, skinId);
        }

        public int GetCurrenctSkinId(int id)
        {
            if (_animalHolder == null)
                return 0;

            return _animalHolder.GetCurrenctSkinId(id);
        }

        public void ApplySkin(int id, int skinId)
        {
            if (_animalHolder == null)
                return;

            _animalHolder.ApplySkin(id, skinId);
        }
        #endregion

        private void OnChangedPlace(int placeId)
        {
            
        }

        private void OnChangedObjectInfo(int id)
        {

        }
    }
}

