using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [System.Serializable]
    public class User
    {
        public int Lv = 1;
        public long Leaf = 100;
        public long Berry = 10;

        public List<int> AnimalIdList = new();
        public List<int> ObjectIdList = new();
        public List<int> PlaceIdList = new();
    }
}

