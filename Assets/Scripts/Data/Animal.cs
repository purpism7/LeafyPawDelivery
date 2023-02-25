using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class Animal : ScriptableObject
    {
        public enum EGrade
        {
            Normal,
            Rare,
        }

        public int Id = 0;
        public EGrade EGrade_ = EGrade.Normal;

        public OpenCondition OpenCondition = new();
        public DropItem DropItem = new();
    }
}

