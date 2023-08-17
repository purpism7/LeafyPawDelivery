using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class Place : Base<Place.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
        }
        
        [SerializeField]
        private Transform objectRootTm;
        public Transform animalRootTm;

        public Transform ObjectRootTm { get { return objectRootTm; } }

        private List<Game.Object> _objectList = new();
        private List<Game.Creature.Animal> _animalList = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            Deactivate();
        }

        public override void ChainUpdate()
        {
            foreach(var animal in _animalList)
            {
                animal?.ChainUpdate();
            }
        }

        public override void Activate()
        {
            base.Activate();

            SetAnimalList();
            SetObjectList();
        }

        public void AddAnimal(Game.Creature.Animal addAnimal)
        {
            if (_animalList == null)
                return;

            var findAnimal = _animalList.Find(animal => animal.Id == addAnimal.Id);
            if (findAnimal != null)
                return;

            _animalList.Add(addAnimal);
        }

        public void RemoveAnimal(int id)
        {
            if (_animalList == null)
                return;

            var findAnimal = _animalList.Find(animal => animal.Id == id);
            if (findAnimal == null)
                return;

            if (_animalList.Remove(findAnimal))
            {
                findAnimal.Deactivate();
            }
        }

        public void AddObject(Game.Object addObj)
        {
            if (_objectList == null)
                return;

            foreach(var obj in _objectList)
            {
                if (obj == null)
                    continue;

                if (obj.UId == addObj.ObjectUId)
                    return;
            }

            _objectList.Add(addObj);
        }

        public void RemoveObject(int objectUId)
        {
            if(_objectList == null)
                return;

            var findObject = _objectList.Find(obj => obj.UId == objectUId);
            if(findObject == null)
                return;

            if (_objectList.Remove(findObject))
            {
                findObject.Deactivate();
            }
        }

        private void SetAnimalList()
        {
            _animalList.Clear();

            var animalInfoList = MainGameManager.Instance?.AnimalMgr?.AnimalInfoList;
            if (animalInfoList == null)
                return;

            for (int i = 0; i < animalInfoList.Count; ++i)
            {
                var animalInfo = animalInfoList[i];
                if (animalInfo == null)
                    continue;

                var data = AnimalContainer.Instance.GetData(animalInfo.Id);
                if (data == null)
                    continue;

                if (data.PlaceId != _data.Id)
                    continue;

                if (!animalInfo.Arrangement)
                    continue;

                var animalData = new Game.Creature.Animal.Data()
                {
                    Id = data.Id,
                    Pos = animalInfo.Pos,
                };

                Game.Creature.Animal resAnimal = null;
                foreach (var animal in _animalList)
                {
                    if (animal == null)
                        continue;

                    if (animal.IsActivate)
                        continue;

                    if (animalInfo.Id != animal.Id)
                        continue;

                    resAnimal = animal;
                    resAnimal?.Initialize(animalData);

                    break;
                }

                if (resAnimal == null)
                {
                    resAnimal = new GameSystem.AnimalCreator()
                        .SetAnimalId(animalInfo.Id)
                        .SetPos(animalInfo.Pos)
                        .Create();

                    _animalList.Add(resAnimal);
                }

                resAnimal?.Activate();
            }
        }

        private void SetObjectList()
        {
            _objectList.Clear();

            var objectInfoList = MainGameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (objectInfoList == null)
                return;

            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                if(objectInfo == null)
                    continue;
                
                var data = ObjectContainer.Instance.GetData(objectInfo.Id);
                if(data == null)
                    continue;
                
                if (data.PlaceId != Id)
                    continue;
                
                if(!objectInfo.Arrangement)
                    continue;
                
                var objectData = new Game.Object.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                    Pos = objectInfo.Pos,
                };

                Game.Object resObj = null;
                foreach(var obj in _objectList)
                {
                    if (obj == null)
                        continue;

                    if (obj.IsActivate)
                        continue;

                    if (objectInfo.Id != obj.Id)
                        continue;

                    resObj = obj;
                    resObj?.Initialize(objectData);

                    break;
                }

                if(resObj == null)
                {
                    resObj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                        .SetData(objectData)
                        .SetId(objectInfo.Id)
                        .SetRootTm(objectRootTm)
                        .Create();

                    _objectList.Add(resObj);
                }

                resObj?.Activate();
            }
        }

        public void ActivateRandomSpeechBubble()
        {
            SetRandomSpeechBubble();
        }

        public void DeactivateAllSpeechBubble()
        {
            foreach(var animal in _animalList)
            {
                animal?.DeactivateSpeechBubble();
            }
        }

        private void SetRandomSpeechBubble()
        {
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
                return;

            if (_animalList == null)
                return;

            if (_animalList.Count <= 0)
                return;

            var randomIndex = UnityEngine.Random.Range(0, _animalList.Count);
            var randomAnimal = _animalList[randomIndex];
            if (randomAnimal == null)
                return;

            randomAnimal.ActivateSpeechBubble(
                () =>
                {
                    SetRandomSpeechBubble();
                });
        }
    }
}

