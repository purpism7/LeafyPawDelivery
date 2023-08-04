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
            mainGameMgr?.ObjectMgr.Listener.AddListener(OnChangedObject);
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

        public void AddAnimal(int animalId)
        {
            if (_animalHolder == null)
                return;
            
            var animalInfo = new Info.Animal()
            {
                Id = animalId,
            };

            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                
            }

            Listener?.Invoke(animalInfo);
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

                if(data.EType_ == OpenConditionData.EType.Starter)
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

                    
                }
            }
        }

        private void OnChangedPlace(int placeId)
        {
            
        }

        private void OnChangedObject(Info.Object obj)
        {

        }
    }
}

