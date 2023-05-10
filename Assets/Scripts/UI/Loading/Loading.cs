using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem.Loader;

namespace GameSystem
{
    public class Loading : MonoBehaviour
    {
        #region Inspector
        public RectTransform loadingTypeRootRectTm = null;
        #endregion

        public void Init()
        {
            Debug.Log("Loading Init");
        }
    }
}


