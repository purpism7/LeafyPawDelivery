using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class Cell : MonoBehaviour
    {
        public class Data
        {
            public int Id = 0;
            public int Row = 0;
            public int Column = 0;
            public float CellSize = 0;
        }

        [SerializeField]
        private BoxCollider boxCollider = null;

        private Vector3 _halfExtnents = Vector3.one * 0.5f;
        private Data _data = null;
        
        public bool IsOverlap { get; private set; } = false;

        public int Id { get { return _data != null ? _data.Id : 0; } }
        public int Row { get { return _data != null ? _data.Row : 0; } }
        public int Column { get { return _data != null ? _data.Column : 0; } }

        //public bool Path = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = IsOverlap ? new Color(1, 0, 0, 1f) : new Color(0, 0, 1, 1f);
            Gizmos.DrawWireCube(transform.position, boxCollider.size);

            //if(Path)
            //{
            //    Gizmos.color = Color.black;
            //    Gizmos.DrawCube(transform.position, boxCollider.size);
            //}
        }

        public void Init(Data data)
        {
            if (data == null)
                return;

            _data = data;

            name = "[" + data.Row + ", " + data.Column + " = " + _data.Id + "]"; 

            SetColliderSize();
            SetLocalPosition();
        }

        private void SetColliderSize()
        {
            if(boxCollider == null)
                return;

            boxCollider.size = new Vector3(_data.CellSize, _data.CellSize, 50f);

            _halfExtnents = boxCollider.size * 0.5f;
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(_data.Row, _data.Column, 0) * _data.CellSize;
        }

        public void SetOverlap()
        {
            if (boxCollider == null)
                return;

            IsOverlap = false;

            var colliders = Physics.OverlapBox(boxCollider.center + transform.position, _halfExtnents, Quaternion.identity);

            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

                // cell 들은 제외.
                var cell = collider.GetComponentInParent<Cell>();
                if (cell != null)
                    continue;

                var gameBaseElement = collider.GetComponentInParent<Game.BaseElement>();
                if (gameBaseElement != null)
                {
                    if (!gameBaseElement.IsOverlap)
                        continue;
                }

                IsOverlap = true;

                break;
            }
        }

        public void ChainUpdate()
        {
            //Overlap();
        }
    }
}

