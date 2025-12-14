using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    [System.Serializable]
    public class GridData : MonoBehaviour
    {
        public int Row = 0;
        public int Column = 0;
        [HideInInspector]
        public float CellSize = 100f;
    }
}

