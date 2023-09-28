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
        [SerializeField]
        private Transform currencyRootTm = null;
        public Transform animalRootTm;

        //public Transform ObjectRootTm { get { return objectRootTm; } }

        private List<Game.Object> _objectList = new();
        private List<Game.Creature.Animal> _animalList = new();

        private Coroutine _speechBubbleCoroutine = null;
        private Coroutine _dropCurrencyCoroutine = null;
        private YieldInstruction _waitSecSpeechBubble = new WaitForSeconds(10f);
        private YieldInstruction _waitSecDropCurrency = new WaitForSeconds(30f);
        private float _initPosZ = 0;

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

            SetObjectList();
            SetAnimalList();
        }

        #region Animal
        public Creature.Animal AddAnimal(int id, int skinId, Vector3 pos)
        {
            if (_animalList == null)
                return null;

            foreach (var animal in _animalList)
            {
                if (animal == null)
                    continue;

                if (animal.Id != id)
                    continue;

                if (animal.SkinId != skinId)
                    continue;

                animal.SetPos(pos);
                animal.Activate();

                return animal;
            }

            var addAnimal = new GameSystem.AnimalCreator()
                 .SetAnimalId(id)
                 .SetSkinId(skinId)
                 .SetPos(pos)
                 .Create();

            _animalList.Add(addAnimal);

            return addAnimal;
        }

        public void RemoveAnimal(int id)
        {
            if (_animalList == null)
                return;

            foreach(var animal in _animalList)
            {
                if (animal == null)
                    continue;

                if (animal.Id != id)
                    continue;

                animal.Deactivate();
            }
        }

        public bool ChangeAnimalSkin(int id, int skinId, Vector3 pos, int currSkinId)
        {
            bool existAnimal = false;

            foreach (var animal in _animalList)
            {
                if (animal == null)
                    continue;

                if (!animal.IsActivate)
                    continue;

                if (animal.Id != id)
                    continue;

                if (animal.SkinId != currSkinId)
                    continue;

                pos = animal.transform.localPosition;

                animal.Deactivate();

                existAnimal = true;

                break;
            }

            if(existAnimal)
            {
                AddAnimal(id, skinId, pos);
            }

            return existAnimal;
        }
        #endregion

        public Game.Object AddObject(int id, Vector3 pos, int uId)
        {
            if (_objectList == null)
                return null;

            foreach(var obj in _objectList)
            {
                if (obj == null)
                    continue;

                if (obj.IsActivate)
                    continue;

                if (obj.Id != id)
                    continue;

                obj.UId = uId;
                obj.Activate();

                return obj;
            }

            var objData = new Game.Object.Data()
            {
                ObjectId = id,
                ObjectUId = uId,
                Pos = pos,
            };

            var addObj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetData(objData)
                .SetId(id)
                .SetRootTm(objectRootTm)
                .Create();

            _objectList.Add(addObj);

            return addObj;
        }

        public void RemoveObject(int id, int objectUId)
        {
            if(_objectList == null)
                return;

            foreach (var obj in _objectList)
            {
                if (obj == null)
                    continue;

                if (obj.Id != id)
                    continue;

                if (obj.UId != objectUId)
                    continue;

                obj.Deactivate();
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

                animalInfo.Pos.z = ++_initPosZ;

                var animalData = new Game.Creature.Animal.Data()
                {
                    //Id = data.Id,
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
                        .SetSkinId(animalInfo.SkinId)
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

                if (objectInfo.EditObjectList == null)
                    continue;

                var data = ObjectContainer.Instance.GetData(objectInfo.Id);
                if(data == null)
                    continue;
                
                if (data.PlaceId != Id)
                    continue;

                foreach(var editObject in objectInfo.EditObjectList)
                {
                    if (editObject == null)
                        continue;

                    if (!editObject.Arrangement)
                        continue;

                    editObject.Pos.z = ++_initPosZ;

                    var objectData = new Game.Object.Data()
                    {
                        ObjectId = objectInfo.Id,
                        ObjectUId = editObject.UId,
                        Pos = editObject.Pos,
                    };

                    Game.Object resObj = null;
                    foreach (var obj in _objectList)
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

                    if (resObj == null)
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
        }

        public void EnableCollider(bool enable)
        {
            if(_animalList != null)
            {
                foreach(var animal in _animalList)
                {
                    animal?.EnableCollider(enable);
                }
            }

            if(_objectList != null)
            {
                foreach (var obj in _objectList)
                {
                    obj?.EnableCollider(enable);
                }
            }
        }

        public void ProcessGame()
        {
            ActivateRandomSpeechBubble();

            StartDropCurrency();
            UIUtils.SetActive(currencyRootTm, true);
        }

        public void ProcessEdit()
        {
            DeactivateAllSpeechBubble();

            StopDropCurrency();
            UIUtils.SetActive(currencyRootTm, false);
        }

        #region SpeechBubble
        private void ActivateRandomSpeechBubble()
        {
            DeactivateAllSpeechBubble();
            _speechBubbleCoroutine = StartCoroutine(CoRandomSpeechBubble());
        }

        private void DeactivateAllSpeechBubble()
        {
            if(_speechBubbleCoroutine != null)
            {
                StopCoroutine(_speechBubbleCoroutine);
                _speechBubbleCoroutine = null;
            }

            foreach (var animal in _animalList)
            {
                animal?.DeactivateSpeechBubble();
            }
        }

        private IEnumerator CoRandomSpeechBubble()
        {
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
                yield break;

            if (_animalList == null)
                yield break;

            if (_animalList.Count <= 0)
                yield break;

            yield return _waitSecSpeechBubble;

            if (_speechBubbleCoroutine == null)
                yield break;

            var activateAnimalList = _animalList.FindAll(animal => animal != null ? animal.IsActivate : false);
            var randomIndex = UnityEngine.Random.Range(0, activateAnimalList.Count);
            var randomAnimal = activateAnimalList[randomIndex];
            if (randomAnimal == null)
                yield break;

            randomAnimal.ActivateSpeechBubble(
                () =>
                {
                    ActivateRandomSpeechBubble();
                });
        }
        #endregion

        #region Drop Currency

        private void StartDropCurrency()
        {
            StopDropCurrency();
            _dropCurrencyCoroutine = StartCoroutine(CoDropCurrency());
        }

        private void StopDropCurrency()
        {
            if (_dropCurrencyCoroutine != null)
            {
                StopCoroutine(_dropCurrencyCoroutine);
                _dropCurrencyCoroutine = null;
            }
        }

        private IEnumerator CoDropCurrency()
        {
            if (MainGameManager.Instance.GameState.CheckState<Game.State.Edit>())
                yield break;

            if (_animalList == null)
                yield break;

            if (_animalList.Count <= 0)
                yield break;

            yield return _waitSecDropCurrency;

            if (_dropCurrencyCoroutine == null)
                yield break;

            DropCurrency();

            yield return null;

            StartDropCurrency();
        }

        private void DropCurrency()
        {
            var activateAnimalList = _animalList.FindAll(animal => animal != null ? animal.IsActivate : false);
            var randomIndex = UnityEngine.Random.Range(0, activateAnimalList.Count);
            var randomAnimal = activateAnimalList[randomIndex];
            if (randomAnimal == null)
                return;

            if (randomAnimal.ElementData == null)
                return;

            new DropItemCreator()
                .SetRootTm(currencyRootTm)
                .SetDropItemData(new DropItem.Data()
                {
                    startRootTm = randomAnimal.transform,
                    EElement = Type.EElement.Animal,
                    Value = randomAnimal.ElementData.GetCurrency,
                })
                .Create();
        }
        #endregion
    }
}

