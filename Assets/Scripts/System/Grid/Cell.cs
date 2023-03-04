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

        private Data _data = null;

        public void Init(Data data)
        {
            if (data == null)
            {
                return;
            }

            _data = data;

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

            boxCollider.size = new Vector3(_data.CellSize, _data.CellSize, 0);
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(_data.Row, _data.Column, 0) * _data.CellSize;
        }
    }
}

