using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [CreateAssetMenu(menuName = "Animals/ScriptableObject/User")]
    public class User : ScriptableObject
    {
        public int Lv = 1;
        public long Leaf = 0;
        public long Berry = 0;

        public List<int> AnimalIdList = new();
        public List<int> ObjectIdList = new();
        public List<int> PlaceIdList = new();
    }
}

