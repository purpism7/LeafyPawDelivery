using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class Object
    {
        public int Id = 0;

        public List<EditObject> EditObjectList = new();
    }

    [System.Serializable]
    public class EditObject
    {
        public int UId = 0;
        public Vector3 Pos = Vector3.zero;
        public bool Arrangement = false;
    }
}
