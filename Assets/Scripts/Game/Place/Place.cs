using Cysharp.Threading.Tasks;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Common;
using UnityEngine;
using static Game.Type;

namespace Game
{
    public interface IPlace
    {
        List<Game.Creature.Animal> AnimalList { get; }
        List<Game.Object> ObjectList { get; }

        void RemoveObject(int id, int uId);
        void RemoveAll(System.Action endAction);

        void CreateDropItem(DropItem.Data dropItemData);
        void AllPickUpDropItem(System.Func<Vector3, Vector3> func);
        void SetWorldUIActive(bool isActive);

        Collider2D Collider { get; }
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
            public GardenManager GardenManager { get; private set; } = null;

            public Data WithGardenManager(GardenManager gardenManager)
            {
                GardenManager = gardenManager;
                return this;
            }
        }

        [SerializeField]
        private Transform objectRootTm;
        [SerializeField]
        private Transform itemRootTm = null;
        [SerializeField]
        private BGMPlayer bgmPlayer = null;
        [SerializeField]
        private Collider2D collider = null;

        public Transform animalRootTm;

        private List<Game.Object> _objectList = new();
        private List<Game.Creature.Animal> _animalList = new();
        private List<Game.DropItem> _dropItemList = new();
        private bool _initialize = false;

        private PlaceEventController _placeEventCtr = null;
        private float _objectPosZ = GameUtils.GetPosZOrder(Type.EPosZOrder.Object);

        public IPlaceState.EType State { get; private set; } = IPlaceState.EType.None;

        private string KeyObjectPosZ
        {
            get
            {
                return $"ObjectPosZ_{Id}";
            }
        }
        
        public Collider2D Collider { get { return collider; } } 

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
            else
            {
                // Debug.Log("here");
                SetWorldUIActive(false);
            }

            SetState(IPlaceState.EType.Active);

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
            
            SetState(IPlaceState.EType.Deactive);
            
            bgmPlayer?.Stop();
        }

        #region Animal
        public Creature.Animal SpawnAnimal(int id, int skinId, Vector3 pos, bool spwaned, out bool activate)
        {
            activate = false;
            
            if (_animalList == null)
                return null;

            pos.z = GameUtils.CalcPosZ(pos.y);  

            foreach (var animal in _animalList)
            {
                if (animal == null)
                    continue;

                if (animal.Id != id)
                    continue;

                if (animal.SkinId != skinId)
                    continue;

                activate = animal.IsActivate;
      
                animal.SetSpawned(spwaned);
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

            addAnimal?.SetSpawned(spwaned);

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
                SpawnAnimal(id, skinId, pos, false, out bool activate);
            }

            return existAnimal;
        }
        #endregion

        #region Object
        public Game.Object SpawnObject(int id, Vector3 pos, int uId, string uniqueID)
        {
            if (_objectList == null)
                return null;
            
            var objectType = ObjectType.None;
            var objectData = ObjectContainer.Instance?.GetData(id);
            if (objectData != null)
                objectType = objectData.ObjectType;

            foreach (var obj in _objectList)
            {
                if (obj == null)
                    continue;

                if (obj.IsActivate)
                    continue;

                if (obj.Id != id)
                    continue;

                obj.SetSpawned(true);
                obj.Reset(uId, pos);
                obj.Activate();

                return obj;
            }

            var objData = new Game.Object.Data
            {
                ObjectId = id,
                ObjectUId = uId,
                Pos = pos,
            }.WithObjectUniqueID(uniqueID)
             .WithObjectType(objectType);
            
            var addObject = CreateObject(objData, objectType);
            addObject?.SetSpawned(true);

            _objectList.Add(addObject);

            return addObject;
        }

        private Object CreateObject(Game.Object.Data objectData, ObjectType objectType)
        {
            Object obj = null;
            if (objectType == ObjectType.Garden)
            {
                var gardenPlot = CreateObject<Game.GardenPlot>(objectData);
                gardenPlot?.SetPlotListener(_data.GardenManager);
                gardenPlot?.SetPlotDataProvider(_data.GardenManager);

                obj = gardenPlot;
            }
            else
            {
                obj = CreateObject<Game.Object>(objectData);
            }

            return obj;
        }

        private T CreateObject<T>(Game.Object.Data objectData) where T : Game.Object
        {
            if (objectData == null)
                return null;
            
            return new GameSystem.ObjectCreator<T, Game.Object.Data>()
                .SetData(objectData)
                .SetId(objectData.ObjectId)
                .SetRootTm(objectRootTm)
                .Create();
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
        
        public float ObjectPosZ()
        {
            _objectPosZ -= ObjectPosZOffset;

            PlayerPrefs.SetFloat(KeyObjectPosZ, _objectPosZ);

            return _objectPosZ;
        }

        private float ObjectPosZOffset
        {
            get
            {
                return GameUtils.PosZOffset * 0.1f;
            }
        }
        #endregion
        
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

                var objectData = ObjectContainer.Instance?.GetData(objectInfo.Id);
                if(objectData == null)
                    continue;

                if (objectData.PlaceId != Id && objectData.PlaceId > 0)
                    continue;

                foreach (var editObject in editObjectList)
                {
                    if (editObject == null)
                        continue;

                    if (!editObject.Arrangement)
                        continue;

                    editObject.Pos.z += GameUtils.PosZOffset;

                    var objData = new Game.Object.Data()
                    {
                        ObjectId = objectInfo.Id,
                        ObjectUId = editObject.UId,
                        Pos = editObject.Pos,
                    }.WithObjectUniqueID(editObject.uniqueID)
                     .WithObjectType(objectData.ObjectType);

                    Game.Object resObject = null;
                    foreach (var obj in _objectList)
                    {
                        if (obj == null)
                            continue;

                        if (obj.IsActivate)
                            continue;

                        if (objectInfo.Id != obj.Id)
                            continue;

                        resObject = obj;
                        resObject.Initialize(objData);

                        break;
                    }

                    if (resObject == null)
                    {
                        resObject  = CreateObject(objData, objectData.ObjectType);
                        _objectList.Add(resObject);
                    }

                    resObject?.Activate();
                }
            }
        }
        
        public void Boom()
        {
            SetState(IPlaceState.EType.Active);
            
            if (_placeEventCtr == null)
            {
                _placeEventCtr = new();
                _placeEventCtr?.Initialize(this, _data.Id);
            }

            _placeEventCtr?.Start();

            itemRootTm.SetActive(true);
            SetWorldUIActive(true);
        }

        public void Bust()
        {
            SetState(IPlaceState.EType.Edit);
            
            _placeEventCtr?.End();

            itemRootTm.SetActive(false);
            SetWorldUIActive(false);
        }

        private void SetState(IPlaceState.EType state)
        {
            State = state;
        }
        
        public void SetWorldUIActive(bool isActive)
        {
            var objectList = _objectList;
            if (objectList == null)
                return;
            
            foreach (var obj in objectList)
            {
                if(obj == null)
                    continue;
                
                if(!obj.IsActivate)
                    continue;

                if (obj is IGardenPlot gardenPlot)
                {
                    gardenPlot.SetWorldUIActive(isActive);
                }
            }

            //if (isActive)
            //    UIManager.Instance?.SortWorldUIDepth();
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

        void IPlace.RemoveAll(System.Action endAction)
        {
            if(_animalList != null)
            {
                foreach (Creature.IAnimal iAnimal in _animalList)
                {
                    if (iAnimal == null)
                        continue;

                    iAnimal.Remove(false);
                }
            }

            if (_objectList != null)
            {
                foreach (IObject iObj in _objectList)
                {
                    if (iObj == null)
                        continue;

                    iObj.Remove(false);
                }

                _objectPosZ = GameUtils.GetPosZOrder(Type.EPosZOrder.Object);
                ObjectPosZ();
            }

            endAction?.Invoke();
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

        void IPlace.AllPickUpDropItem(System.Func<Vector3, Vector3> func)
        {
            AllPickUpDropItemAsync(func).Forget();
        }

        private async UniTask AllPickUpDropItemAsync(System.Func<Vector3, Vector3> func)
        {
            if (_dropItemList == null)
                return;

            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return;
            
            iGameCameraCtr.SetPositionUICamera(false, Vector3.zero);

            await UniTask.Yield();
            
            foreach (IDropItem iDropItem in _dropItemList)
            {
                if(iDropItem == null)
                    continue;
                
                if(iDropItem.EItemType != Type.EItem.Currency)
                    continue;
                
                iDropItem?.PickUp(func);

                await UniTask.WaitForSeconds(0.1f);
            }
            
            await UniTask.WaitForSeconds(2f);
            
            iGameCameraCtr.SetPositionUICamera(true, Vector3.zero);
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

