using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class AnimalManager : Manager.BaseElement<AnimalManager.Data>, IEvent
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }

        public static UnityEvent<Event.AnimalData> Event { get; private set; } = null;

        private Data _data = null;
        private Info.AnimalHolder _animalHolder = new();
        private Game.Event.Animal _animalEvent = new();

        public Creature.Animal Conversation { get; private set; } = null;

        public List<Info.Animal> AnimalInfoList => _animalHolder?.AnimalInfoList;

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                SaveAnimalPos();
            }
        }

        private void OnApplicationQuit()
        {
            SaveAnimalPos();
        }

        public override MonoBehaviour Initialize()
        {
            Event = new UnityEvent<Event.AnimalData>();
            Event?.RemoveAllListeners();

            Game.ObjectManager.Event.AddListener(OnChangedObject);

            return this;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _data = data;

            _animalHolder?.LoadInfo();

            yield break;
        }

        private void SaveAnimalPos()
        {
            var iPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace as IPlace;
            var animalList = iPlace?.AnimalList;
            if (animalList == null)
                return;

            foreach (var animal in animalList)
            {
                if (animal == null)
                    continue;

                _animalHolder?.SetPos(animal.Id, animal.LocalPos);
            }

            _animalHolder?.SaveInfo();
        }

        public bool CheckIsAll(int placeId)
        {
            var animalDataList = AnimalContainer.Instance?.GetDataListByPlaceId(placeId);
            if (animalDataList == null)
                return false;

            var openConditionDataList = AnimalOpenConditionContainer.Instance?.GetDataList(new[] { OpenConditionData.EType.Starter, OpenConditionData.EType.Buy });

            foreach (var animalData in animalDataList)
            {
                if (animalData == null)
                    continue;

                if (openConditionDataList != null &&
                    openConditionDataList.Find(openConditionData => openConditionData.Id == animalData.Id) != null)
                {
                    if (!CheckExist(animalData.Id))
                        return false;
                }
            }

            return true;
        }

        public bool CheckGetStarter
        {
            get
            {
                int placeId = GameUtils.ActivityPlaceId;
                if (placeId != Games.Data.Const.StartPlaceId)
                    return true;

                var animalDataList = AnimalContainer.Instance?.GetDataListByPlaceId(placeId);
                if (animalDataList == null)
                    return false;

                var openConditionDataList = AnimalOpenConditionContainer.Instance?.GetDataList(new[] { OpenConditionData.EType.Starter });
                foreach (var animalData in animalDataList)
                {
                    if (animalData == null)
                        continue;

                    if (openConditionDataList != null &&
                        openConditionDataList.Find(openConditionData => openConditionData.Id == animalData.Id) != null)
                    {
                        if (!CheckExist(animalData.Id))
                            return false;
                    }
                }

                return true;
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
                SkinId = Games.Data.Const.AnimalBaseSkinId,
            };

            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                var data = new Game.Event.AddAnimalData()
                {
                    id = animalInfo.Id,
                };

                Event?.Invoke(data);

                _animalEvent?.Emit(data);

                Info.UserManager.Instance?.AddAnimal(animalInfo);

                Info.Connector.Get?.SetAddAnimal(id);

                Notification.Get?.Notify(Notification.EType.OpenPlace);
                Notification.Get?.Notify(Notification.EType.AddAnimal);
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

            Event?.Invoke(new Game.Event.ArrangeAnimalData()
            {
                id = id,
            });
        }

        public void SetConverationAnimal(Creature.Animal animal)
        {
            Conversation = animal;
        }

        public Info.Animal GetAnimalInfo(int animalId)
        {
            return _animalHolder?.GetAnimalInfo(animalId);
        }

        void IEvent.Starter(System.Action endAction)
        {
            if (_data == null)
                return;

            _animalEvent?.Starter(endAction);
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

            Info.UserManager.Instance?.AddAnimalSkin(id, skinId);
        }
        #endregion
        
        #region Friendship

        public void AddFriendshipPoint(int id, int point, int cash = 0)
        {
            if (_animalHolder == null)
                return;

            if (_animalHolder.AddFriendshipPoint(id, point))
                Info.UserManager.Instance?.AddFriendshipPoint(id, point, cash);
        }

        public bool CheckGetFriendshipReward(int id, int index)
        {
            if (_animalHolder == null)
                return true;

            return _animalHolder.GetFriendshipReward(id, index);
        }
        
        public bool CheckMaxFriendshipPoint(int id)
        {
            if (_animalHolder == null)
                return true;
    
            var animalInfo = _animalHolder.GetAnimalInfo(id);
            if (animalInfo == null)
                return true;

            return animalInfo.FriendshipPoint >= Games.Data.Const.MaxFriendshipPoint;
        }
        #endregion

        private void OnChangedObject(Game.Event.ObjectData objectData)
        {

        }
    }
}

