using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

using Game;
using System.ComponentModel;

namespace Info
{
    public class ObjectHolder : Holder.Base
    {
        //private readonly string ObjectUIdKey = "Object_UId_Key";

        protected override string JsonFilePath
        {
            get
            {
                return Path.Combine(RootJsonFilePath, "Object_Place_{0}.txt");
            }
        }

        private const string _secretKey = "HankyuloBJect";

        // Key = Place Id
        private Dictionary<int, List<Info.Object>> _objectInfoDic = new();
        private List<int> _objectIdList = new();

        public override void LoadInfo()
        {
            RootJsonFilePath = Utility.GetInfoPath();

            _objectInfoDic?.Clear();

            int totalPlaceCount = 0;
            IPlaceData iPlaceData = MainGameManager.Get<PlaceManager>();
            if(iPlaceData != null)
            {
                totalPlaceCount = iPlaceData.TotalPlaceCount;
            }

            for (int i = 1; i <= totalPlaceCount; ++i)
            {
                List<Info.Object> objectInfoList = null;
                var fullPath = string.Format(JsonFilePath, i);

                if (System.IO.File.Exists(fullPath))
                {
                    var encodeStr = System.IO.File.ReadAllText(fullPath);
                    var jsonStr = encodeStr.Decrypt(_secretKey);
                    //Debug.Log("jsonStr = " + jsonStr);
                    objectInfoList = JsonHelper.FromJson<Info.Object>(jsonStr)?.ToList();
                }

                var objectIdList = Info.UserManager.Instance?.User?.ObjectIdList;
                if (objectIdList == null)
                    continue;

                foreach (int objectId in objectIdList)
                {
                    var objectData = ObjectContainer.Instance?.GetData(objectId);
                    if (objectData == null)
                        continue;

                    Info.Object objectInfo = null;

                    if (objectInfoList != null)
                    {
                        objectInfo = objectInfoList.Find(findObjectInfo => findObjectInfo != null && findObjectInfo.Id == objectId);
                    }

                    AddObject(objectData.PlaceId, objectId);

                    if (objectInfo != null)
                    {
                        if (_objectInfoDic.TryGetValue(objectData.PlaceId, out List<Info.Object> infoList))
                        {
                            var findObjectInfo = infoList.Find(info => info != null ? info.Id == objectInfo.Id : false);
                            if (findObjectInfo != null)
                            {
                                findObjectInfo.EditObjectList.AddRange(objectInfo.EditObjectList);
                            }
                        }
                    }
                }
            }
        }

        private void SaveInfo(int placeId)
        {
            if(_objectInfoDic == null)
                return;

            if(_objectInfoDic.TryGetValue(placeId, out List<Object> objectInfoList))
            {
                if (objectInfoList == null ||
                    objectInfoList.Count <= 0)
                    return;

                var jsonStr = JsonHelper.ToJson(objectInfoList.ToArray());
                var encodeStr = jsonStr.Encrypt(_secretKey);

                var fullPath = string.Format(JsonFilePath, placeId);
                
                System.IO.File.WriteAllText(fullPath, encodeStr);
            }
        }

        public bool AddObjectInfo(int id)
        {
            var objectData = ObjectContainer.Instance?.GetData(id);
            if (objectData == null)
                return false;

            //int objectUId = PlayerPrefs.GetInt(ObjectUIdKey, 0);
            //editObject.UId = ++objectUId;

            AddObject(objectData.PlaceId, id);

            //PlayerPrefs.SetInt(ObjectUIdKey, objectUId);

            SaveInfo(objectData.PlaceId);

            return true;
        }

        private void AddObject(int placeId, int id)
        {
            var objectInfo = new Object()
            {
                Id = id,
            };

            List<Object> objectInfoList = null;
            if (_objectInfoDic.TryGetValue(placeId, out objectInfoList))
            {
                var findObjectInfo = objectInfoList.Find(info => info != null ? info.Id == id : false);
                if(findObjectInfo == null)
                {
                    objectInfoList.Add(objectInfo);
                }
                else
                {
                    findObjectInfo.Count += 1;
                }
            }
            else
            {
                objectInfoList = new List<Object>();
                objectInfoList.Clear();
                objectInfoList.Add(objectInfo);

                _objectInfoDic.Add(placeId, objectInfoList);
            }

            _objectIdList.Add(id);
        }

        public EditObject GetAddEditObject(int id)
        {
            var objectInfo = GetObjectInfoById(id, GameUtils.ActivityPlaceId);
            if (objectInfo == null)
                return null;

            var objectData = ObjectContainer.Instance?.GetData(id);

            var editObjectList = objectInfo.EditObjectList;
            if (editObjectList != null)
            {
                foreach(var editObject in editObjectList)
                {
                    if (editObject == null)
                        continue;

                    if (editObject.Arrangement)
                        continue;

                    GenerateObjectUniqueID(editObject, objectData);

                    return editObject;
                }
            }
            else
            {
                objectInfo.EditObjectList = new();
                objectInfo.EditObjectList.Clear();
            }

            var addEditObject = new EditObject
            {
                UId = objectInfo.EditObjectList.Count + 1,
            };

            GenerateObjectUniqueID(addEditObject, objectData);
 
            //Debug.Log("ObjectHolder GetAddEditObject = " + addEditObject.UId);
            objectInfo.EditObjectList.Add(addEditObject);

            return addEditObject;
        }

        private void GenerateObjectUniqueID(EditObject editObject, Data.Object objectData)
        {
            if (editObject == null)
                return;

            if (objectData == null)
                return;

            if (objectData.ObjectType == Type.ObjectType.None)
                return;

            // if (string.IsNullOrEmpty(editObject.uniqueID))
            {
                editObject.uniqueID = GameUtils.GenerateUniqueID("O");
            }
        }

        public void Remove(int id, int objectUId)
        {
            var objectData = ObjectContainer.Instance.GetData(id);
            if (objectData == null)
                return;

            var editObject = GetEditObject(id, objectUId, objectData.PlaceId);
            if (editObject == null)
                return;

            editObject.Pos = Vector3.zero;
            editObject.Arrangement = false;

            SaveInfo(objectData.PlaceId);
        }

        public bool ArrangeObject(Game.Object obj, int placeId)
        {
            if (obj == null)
                return false;

            var objectInfo = GetObjectInfoById(obj.Id, placeId);
            if(objectInfo == null)
                return false;

            if (objectInfo.EditObjectList == null)
                return false;

            foreach(var editObject in objectInfo.EditObjectList)
            {
                if (editObject == null)
                    continue;

                if (editObject.UId != obj.UId)
                    continue;

                editObject.Pos = obj.LocalPos;
                editObject.Arrangement = true;

                break;
            }

            SaveInfo(placeId);

            return true;
        }

        public Info.EditObject GetEditObject(int id, int objectUId, int placeId)
        {
            if(_objectInfoDic == null)
                return null;

            var objectInfo = GetObjectInfoById(id, placeId);
            if (objectInfo == null)
                return null;

            var editObjectList = objectInfo.EditObjectList;
            if (editObjectList == null)
                return null;

            foreach(var editObject in editObjectList)
            {
                if (editObject == null)
                    continue;

                if (editObject.UId != objectUId)
                    continue;

                return editObject;
            }

            return null;
        }

        public Info.Object GetObjectInfoById(int objectId, int placeId)
        {
            if(_objectInfoDic == null)
                return null;

            var objectInfoList = GetObjectInfoList(placeId);
            if (objectInfoList == null)
                return null;
            
            return objectInfoList.Find(objectInfo => objectInfo.Id == objectId);
        }

        public List<Info.Object> GetObjectInfoList(int placeId)
        {
            if (_objectInfoDic == null)
                return null;

            if (_objectInfoDic.TryGetValue(placeId, out List<Object> objectInfoList))
            {
                return objectInfoList;
            }

            return null;
        }

        public int GetRemainCount(int id)
        {
            var objectInfo = GetObjectInfoById(id, GameUtils.ActivityPlaceId);
            if (objectInfo == null)
                return 0;

            if (objectInfo.EditObjectList == null)
                return 0;

            var objectData = ObjectContainer.Instance?.GetData(id);
            if (objectData == null)
                return 0;

            int count = 0;
            foreach(var editObject in objectInfo.EditObjectList)
            {
                if (editObject == null)
                    continue;

                if (!editObject.Arrangement)
                    continue;

                ++count;
            }

            int limitCount = objectData.Count;
            if (objectData.ObjectType == Type.ObjectType.Garden)
                limitCount = objectInfo.Count;

            return limitCount - count;
        }

        #region Firebase
        //public void Save()
        //{
        //    var firebaseMgr = GameSystem.FirebaseManager.Instance;
        //    if (firebaseMgr == null)
        //        return;

        //    var userId = firebaseMgr.Auth?.UserId;
        //    if (string.IsNullOrEmpty(userId))
        //        return;

        //    firebaseMgr?.Database?.Save(userId, JsonUtility.ToJson(_objectIdList));
        //}
        #endregion
    }
}

