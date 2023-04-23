using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class Cell : MonoBehaviour
    {
        public class Data
        {
            public int Row = 0;
            public int Column = 0;
            public float CellSize = 0;
        }

        public BoxCollider boxCollider;

        public Data Data_ { get; private set; } = null;

        public void Init(Data data)
        {
            if (data == null)
            {
                return;
            }

            Data_ = data;

            name = "[" + data.Row + ", " + data.Column + "]"; 

            SetColliderSize();
            SetLocalPosition();
        }

        private void SetColliderSize()
        {
            if(boxCollider == null)
            {
                return;
            }

            boxCollider.size = new Vector3(Data_.CellSize, Data_.CellSize, 0);
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(Data_.Row, Data_.Column, 0) * Data_.CellSize;
        }
    }
}

