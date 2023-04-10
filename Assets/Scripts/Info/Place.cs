using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class Place
    {
        public int Id { get; private set; } = 0;
        public List<Object> ObjectList { get; private set; } = new();

        public Place(int id, List<Object> objList)
        {
            Id = id;
            ObjectList = objList;
        }

        public Object GetObject(int objectId)
        {
            if(ObjectList == null)
            {
                return null;
            }

            return ObjectList.Find(obj => obj.Id == objectId);
        }

        public void AddObject(Object addObj)
        {
            if(addObj == null)
            {
                return;
            }

            if(ObjectList == null)
            {
                ObjectList = new();
            }

            var findIndex = ObjectList.FindIndex(obj => obj.Id == addObj.Id);
            if (findIndex >= 0)
            {
                ObjectList[findIndex] = addObj;
            }
            else
            {
                ObjectList.Add(addObj);
            }
        }
    }
}

