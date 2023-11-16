using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

using Game;

namespace Info
{
    public class ObjectHolder : Holder.Base
    {
        private readonly string ObjectUIdKey = "Object_UId_Key";
        
        protected override string JsonFilePath => RootJsonFilePath + "/Info/Object_Place_{0}.json";

        // Key = Place Id
        private Dictionary<int, List<Info.Object>> _objectInfoDic = new();
        private List<int> _objectIdList = new();

        public override void LoadInfo()
        {
            _objectInfoDic?.Clear();

            for(int i = 0; i < Game.Data.Const.TotalPlaceCount; ++i)
            {
                var jsonfilePath = string.Format(JsonFilePath, i);
                if (!System.IO.File.Exists(jsonfilePath))
                    break;

                var jsonString = System.IO.File.ReadAllText(jsonfilePath);
                var objectInfos = JsonHelper.FromJson<Info.Object>(jsonString);
                if (objectInfos == null)
                    break;

                foreach (var objectInfo in objectInfos)
                {
                    if (objectInfo == null)
                        continue;

                    var objectData = ObjectContainer.Instance.GetData(objectInfo.Id);
                    if (objectData == null)
                        continue;

                    AddObject(objectData.PlaceId, objectInfo.Id);

                    //objectInfo.EditObjectList.Add(objectInfo.EditObjectList)

                    if (_objectInfoDic.TryGetValue(objectData.PlaceId, out List<Info.Object> objectInfoList))
                    {
                        var findObjectInfo = objectInfoList.Find(info => info != null ? info.Id == objectInfo.Id : false);
                        if (findObjectInfo != null)
                        {
                            findObjectInfo.EditObjectList.AddRange(objectInfo.EditObjectList);
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

                var jsonString = JsonHelper.ToJson(objectInfoList.ToArray());
                var jsonFilePath = string.Format(JsonFilePath, placeId);
                
                System.IO.File.WriteAllText(jsonFilePath, jsonString);
            }
        }

        public bool AddObjectInfo(int id)
        {
            var objectData = ObjectContainer.Instance.GetData(id);
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

            var editObjectList = objectInfo.EditObjectList;
            if (editObjectList != null)
            {
                //for(int i = 0; i < editObjectList.Count; ++i)
                //{
                //    for(int j = 0; j < editObjectList.Count; ++j)
                //    {
                //        if (i == j)
                //            continue;

                //        if(editObjectList[i].UId == editObjectList[j].UId)
                //        {
                //            editObjectList[j].UId = j;
                //        }
                //    }
                //}

                foreach(var editObject in editObjectList)
                {
                    if (editObject == null)
                        continue;

                    if (editObject.Arrangement)
                        continue;

                    return editObject;
                }
            }
            else
            {
                objectInfo.EditObjectList = new();
                objectInfo.EditObjectList.Clear();
            }

            var addEditObject = new EditObject()
            {
                UId = objectInfo.EditObjectList.Count + 1,
            };
            //Debug.Log("ObjectHolder GetAddEditObject = " + addEditObject.UId);
            objectInfo.EditObjectList.Add(addEditObject);

            return addEditObject;
        }

        public void RemoveObject(int id, int objectUId)
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

        public void ArrangeObject(int id, int objectUId, Vector3 pos, int placeId)
        {
            var objectInfo = GetObjectInfoById(id, placeId);
            if(objectInfo == null)
                return;

            if (objectInfo.EditObjectList == null)
                return;

            foreach(var editObject in objectInfo.EditObjectList)
            {
                if (editObject == null)
                    continue;

                if (editObject.UId != objectUId)
                    continue;

                editObject.Pos = pos;
                editObject.Arrangement = true;

                break;
            }

            SaveInfo(placeId);
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
            int placeId = MainGameManager.Instance.placeMgr.ActivityPlaceId;

            var objectInfo = GetObjectInfoById(id, placeId);
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

            return objectData.Count - count;
        }

        #region Firebase
        public void Save()
        {
            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            firebaseMgr?.Database?.Save(userId, JsonUtility.ToJson(_objectIdList));
        }
        #endregion
    }
}

