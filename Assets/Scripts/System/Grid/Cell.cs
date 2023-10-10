using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class Cell : MonoBehaviour
    {
        public enum EState
        {
            None,


        }

        public class Data
        {
            public IListener IListener = null;
            public int Row = 0;
            public int Column = 0;
            public float CellSize = 0;
        }

        public interface IListener
        {
            void Overlap(List<GameObject> list);
        }

        [SerializeField]
        private BoxCollider boxCollider = null;

        private List<GameObject> _overlapList = new List<GameObject>();

        public Data Data_ { get; private set; } = null;

        public void Init(Data data)
        {
            if (data == null)
                return;

            Data_ = data;

            name = "[" + data.Row + ", " + data.Column + "]"; 

            SetColliderSize();
            SetLocalPosition();
        }

        private void SetColliderSize()
        {
            if(boxCollider == null)
                return;

            boxCollider.size = new Vector3(Data_.CellSize, Data_.CellSize, 0);
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(Data_.Row, Data_.Column, 0) * Data_.CellSize;
        }

        private void Overlap()
        {
            _overlapList.Clear();

            var colliders = Physics.OverlapBox(boxCollider.center + transform.position, boxCollider.size / 2f);

            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

                var cell = collider.GetComponentInParent<Cell>();
                if (cell != null)
                    continue;

                //Debug.Log(obj.gameObject.name);

                _overlapList.Add(collider.gameObject);
            }

            Data_?.IListener?.Overlap(_overlapList);
        }

        public void ChainUpdate()
        {
            Overlap();
        }
    }
}

