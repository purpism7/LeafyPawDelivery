using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class AnimalManager : Manager.BaseElement<AnimalManager.Data>, IStarter
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }

        public static UnityEvent<Info.Animal> Listener { get; private set; } = null;

        private Data _data = null;
        private Info.AnimalHolder _animalHolder = new();

        public List<Info.Animal> AnimalInfoList => _animalHolder?.AnimalInfoList;

        protected override void Initialize()
        {
            Debug.Log("AnimalManager Initialize");

            Listener = new UnityEvent<Info.Animal>();
            Listener.RemoveAllListeners();

            var mainGameMgr = MainGameManager.Instance;
            Game.ObjectManager.Listener.AddListener(OnChangedObjectInfo);
            Game.PlaceManager.Listener?.AddListener(OnChangedPlace);

            _animalHolder?.LoadInfo();
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _data = data;

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
        public override void Add(int id)
        {
            if (_animalHolder == null)
                return;
            
            var animalInfo = new Info.Animal()
            {
                Id = id,
                SkinId = Game.Data.Const.AnimalBaseSkinId,
            };

            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                Listener?.Invoke(animalInfo);
            }
        }

        public override void Remove(int id, int uId = 0)
        {
            _animalHolder?.RemoveAnimal(id);
        }

        public override bool CheckExist(int id)
        {
            if (AnimalInfoList == null)
                return false;

            return GetAnimalInfo(id) != null;
        }

        public void ArrangeAnimal(int id, Vector3 pos)
        {
            _animalHolder?.ArrangeAnimal(id, pos);
        }

        

        public Info.Animal GetAnimalInfo(int animalId)
        {
            return _animalHolder?.GetAnimalInfo(animalId);
        }

        void IStarter.Check()
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

                if (CheckExist(data.Id))
                    continue;

                if (data.EType_ == OpenConditionData.EType.Starter)
                {
                    Sequencer.EnqueueTask(
                        () =>
                        {
                            var popup = new PopupCreator<Obtain, Obtain.Data>()
                                .SetData(new Obtain.Data()
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

                    Add(data.Id);
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

        public void AddSkin(int id, int skinId)
        {
            if (_animalHolder == null)
                return;

            _animalHolder.AddASkin(id, skinId);
        }
        #endregion

        private void OnChangedPlace(int placeId)
        {
            
        }

        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {

        }
    }
}

