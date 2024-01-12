using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

namespace Info
{
    public partial class User
    {
        [SerializeField]
        private List<int> objectIdList = new();

        public List<int> ObjectIdList { get { return objectIdList; } }

        #region Object
        public void AddObject(int id)
        {
            if (CheckExistObject(id))
                return;

            objectIdList.Add(id);
        }

        public bool CheckExistObject(int id)
        {
            if (objectIdList == null)
                return false;

            return objectIdList.Contains(id);
        }
        #endregion
    }
}
