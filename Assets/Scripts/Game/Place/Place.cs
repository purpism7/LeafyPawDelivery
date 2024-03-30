using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public interface IPlace
    {
        List<Game.Creature.Animal> AnimalList { get; }
        List<Game.Object> ObjectList { get; }
        
        void CreateDropItem(DropItem.Data dropItemData);
    }

    public interface IPlaceState
    {
        public enum EType
        {
            None,

            Active,
            Deactive,
            Edit,
        }

        EType State { get; }
    }

    public class Place : Base<Place.Data>, IPlace, IPlaceState
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public bool onBGM = true;
        }

        [SerializeField]
        private Transform objectRootTm;
        [SerializeField]
        private Transform itemRootTm = null;
        [SerializeField]
        private BGMPlayer bgmPlayer = null;

        public Transform animalRootTm;

        private List<Game.Object> _objectList = new();
        private List<Game.Creature.Animal> _animalList = new();
        private List<Game.DropItem> _dropItemList = new();
        private bool _initialize = false;

        private PlaceEventController _placeEventCtr = null;
        private IPlaceState.EType _state = IPlaceState.EType.None;
        private float _objectPosZ = GameUtils.GetPosZOrder(Type.EPosZOrder.Object);

        public string KeyObjectPosZ
        {
            get
            {
                return string.Format("ObjectPosZ_{0}", Id);
            }
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _initialize = true;

            _dropItemList?.Clear();

            Info.Setting.Event?.AddListener(OnChangedSetting);

            Deactivate();
        }

        private void Initialize()
        {
            if (!_initialize)
                return;

            SetObjectList();
            SetAnimalList();
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

            Initialize();

            GameUtils.SetActive(animalRootTm, true);

            if(!_initialize)
            {
                Boom();
            }

            _state = IPlaceState.EType.Active;

            if(_data != null &&
               _data.onBGM)
            {
                bgmPlayer?.Play();
            }

            if(_initialize)
            {
                _initialize = false;
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            GameUtils.SetActive(animalRootTm, false);

            Bust();

            _state = IPlaceState.EType.Deactive;

            bgmPlayer?.Stop();
        }

        #region Animal
        public Creature.Animal SpwanAnimal(int id, int skinId, Vector3 pos, bool spwaned)
        {
            if (_animalList == null)
                return null;

            pos.z = GameUtils.CalcPosZ(pos.y);  //GetAnimalPosZ(id);

            foreach (var animal in _animalList)
            {
                if (animal == null)
                    continue;

                if (animal.Id != id)
                    continue;

                if (animal.SkinId != skinId)
                    continue;

                animal.SetSpwaned(spwaned);
                animal.SetLocalPos(pos);
                animal.Activate();

                return animal;
            }

            var addAnimal = new GameSystem.AnimalCreator()
                 .SetAnimalId(id)
                 .SetSkinId(skinId)
                 .SetPos(pos)
                 .SetIPlaceState(this)
                 .Create();

            addAnimal?.SetSpwaned(spwaned);

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
                SpwanAnimal(id, skinId, pos, false);
            }

            return existAnimal;
        }

        //private float GetAnimalPosZ(int id)
        //{
        //    var posZ = id * GetPosZOffset(id);
           
        //    return -posZ;
        //}
        #endregion

        #region Object
        public Game.Object SpwanObject(int id, Vector3 pos, int uId)
        {
            if (_objectList == null)
                return null;

            foreach (var obj in _objectList)
            {
                if (obj == null)
                    continue;

                if (obj.IsActivate)
                    continue;

                if (obj.Id != id)
                    continue;

                obj.SetSpwaned(true);
                obj.Reset(uId, pos);
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

            addObj?.SetSpwaned(true);

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

        //private float GetObjectPosZ(int id, int uId)
        //{
        //    float posZ = (id * 10 + uId) * GetPosZOffset(id);

        //    return -posZ;
        //}

        public float ObjectPosZ
        {
            get
            {
                _objectPosZ -= ObjectPosZOffset;

                PlayerPrefs.SetFloat(KeyObjectPosZ, _objectPosZ);

                return _objectPosZ;
            }
        }

        private float ObjectPosZOffset
        {
            get
            {
                return GameUtils.PosZOffset * 0.1f;
            }
        }

        //private int GetCalcPos(float value, out int offset)
        //{
        //    int floorInt = Mathf.FloorToInt(value);
        //    int len = Mathf.FloorToInt(Mathf.Log10(floorInt)) + 1;
        //    offset = 10000 / (int)Mathf.Pow(10, len);
        //    Debug.Log("resOffset = " + offset);
        //    //if(floorY < 100)
        //    //{
        //    //    offset = 
        //    //}
        //    //else if(floorY)
            
        //    return floorInt * offset;
            
        //}
        #endregion

        //private float GetPosZOffset(int id)
        //{
        //    float offset = 0.1f;
        //    int length = (int)(Mathf.Log10(id) + 1);

        //    for (int i = 0; i < length; ++i)
        //    {
        //        offset *= 0.1f;
        //    }

        //    return offset;
        //}

        private void SetAnimalList()
        {
            _animalList.Clear();

            var animalInfoList = MainGameManager.Get<AnimalManager>()?.AnimalInfoList;
            if (animalInfoList == null)
                return;

            for (int i = 0; i < animalInfoList.Count; ++i)
            {
                var animalInfo = animalInfoList[i];
                if (animalInfo == null)
                    continue;

                var data = AnimalContainer.Instance?.GetData(animalInfo.Id);
                if (data == null)
                    continue;

                if (data.PlaceId != _data.Id)
                    continue;

                if (!animalInfo.Arrangement)
                    continue;

                animalInfo.Pos.z = animalInfo.Pos.y * GameUtils.PosZOffset;// GetAnimalPosZ(animalInfo.Id);

                var animalData = new Game.Creature.Animal.Data()
                {
                    Pos = animalInfo.Pos,
                    IPlaceState = this,   
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
                        .SetIPlaceState(this)
                        .Create();

                    _animalList.Add(resAnimal);
                }

                resAnimal?.Activate();
            }
        }

        private void SetObjectList()
        {
            _objectList.Clear();
            _objectPosZ = PlayerPrefs.GetFloat(KeyObjectPosZ, GameUtils.GetPosZOrder(Type.EPosZOrder.Object));
            
            var objectInfoList = MainGameManager.Get<ObjectManager>()?.ObjectInfoList;
            if (objectInfoList == null)
                return;

            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                if(objectInfo == null)
                    continue;

                var editObjectList = objectInfo.EditObjectList;
                if (editObjectList == null)
                    continue;

                var data = ObjectContainer.Instance?.GetData(objectInfo.Id);
                if(data == null)
                    continue;
                
                if (data.PlaceId != Id)
                    continue;

                foreach(var editObject in editObjectList)
                {
                    if (editObject == null)
                        continue;

                    if (!editObject.Arrangement)
                        continue;

                    editObject.Pos.z += GameUtils.PosZOffset;

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

        public void Boom()
        {
            _state = IPlaceState.EType.Active;

            if (_placeEventCtr == null)
            {
                _placeEventCtr = new();
                _placeEventCtr?.Initialize(this, _data.Id);
            }

            _placeEventCtr?.Start();

            itemRootTm.SetActive(true);
        }

        public void Bust()
        {
            _state = IPlaceState.EType.Edit;

            _placeEventCtr?.End();

            itemRootTm.SetActive(false);
        }

        #region IPlace
        List<Creature.Animal> IPlace.AnimalList
        {
            get
            {
                return _animalList;
            }
        }

        List<Game.Object> IPlace.ObjectList
        {
            get
            {
                return _objectList;
            }
        }

        void IPlace.CreateDropItem(DropItem.Data dropItemData)
        {
            if (dropItemData != null)
            {
                dropItemData.startPos.z = GameUtils.GetPosZOrder(Type.EPosZOrder.DropItem);
            }

            if (_dropItemList != null)
            {
                for(int i = 0; i < _dropItemList.Count; ++i)
                {
                    var dropItem = _dropItemList[i];
                    if (dropItem == null)
                        continue;

                    if (dropItem.IsActivate)
                        continue;

                    if(dropItemData != null)
                    {
                        dropItemData.startPos.z -= i;
                    }

                    dropItem.Initialize(dropItemData);
                    dropItem.Activate();

                    return;
                }
            }

            var addDropItem = DropItemCreator.Get
               .SetRootTm(itemRootTm)
               .SetDropItemData(dropItemData)
               .Create();

            _dropItemList?.Add(addDropItem);
        }
        #endregion

        #region IPlaceState
        IPlaceState.EType IPlaceState.State
        {
            get
            {
                return _state;
            }
        }
        #endregion       

        #region Setting.Event
        private void OnChangedSetting(Game.Event.SettingData settingData)
        {
            if (_data == null)
                return;

            if (settingData == null)
                return;

            switch(settingData)
            {
                case Game.Event.BGMData bgmData:
                    {
                        if (bgmData.on == _data.onBGM)
                            return;

                        _data.onBGM = bgmData.on;

                        if (GameUtils.ActivityPlaceId != _data.Id)
                            return;

                        if(bgmData.on)
                        {
                            bgmPlayer?.Play();
                        }
                        else
                        {
                            bgmPlayer?.Stop();
                        }

                        return;
                    }
            }
        }
        #endregion
    }
}

